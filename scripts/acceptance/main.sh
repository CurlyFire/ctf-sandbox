#!/bin/bash
set -euo pipefail

indent() {
  sed 's/^/    /'
}

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
VERSION="$1"
REGION="$2"
PROJECT="$3"
ADMIN_PASSWORD="$4"

IMAGE_PATH="us-central1-docker.pkg.dev/$PROJECT/ctf-sandbox-repo/ctf-sandbox"
BUCKET_NAME="ctf-sandbox"
LATEST_SHA_PATH="acceptance/latest_sha.txt"
LATEST_EXIT_CODE_PATH="acceptance/latest_exit_code.txt"
TMP_FILE="/tmp/tmp.txt"

echo "Fetching latest image $IMAGE_PATH SHA..."
LATEST_SHA=$(gcloud artifacts docker images list "$IMAGE_PATH" --format="value(version)" --sort-by="~CREATE_TIME" | head -n 1)
echo "Latest SHA: $LATEST_SHA"

if gsutil cp "gs://$BUCKET_NAME/$LATEST_SHA_PATH" "$TMP_FILE" 2>/dev/null; then
    PREVIOUS_SHA=$(cat "$TMP_FILE")
    rm -f "$TMP_FILE"
    echo "Previous SHA: $PREVIOUS_SHA"
else
    echo "No previous SHA found"
    PREVIOUS_SHA=""
fi

if [[ "$LATEST_SHA" == "$PREVIOUS_SHA" ]]; then
    if gsutil cp "gs://$BUCKET_NAME/$LATEST_EXIT_CODE_PATH" "$TMP_FILE" 2>/dev/null; then
        PREVIOUS_EXIT_CODE=$(cat "$TMP_FILE")
        rm -f "$TMP_FILE"
        echo "Previous Exit code: $PREVIOUS_EXIT_CODE"
        exit "$PREVIOUS_EXIT_CODE"
    else
        echo "No previous Exit code found"
        exit 0
    fi
fi

# Temp files for capturing stdout and stderr
TMP_DOCKER_COMPOSE_TESTS=$(mktemp)
TMP_ACCEPTANCE_TESTS=$(mktemp)
TMP_E2E_TESTS=$(mktemp)

# Start scripts in parallel, redirecting output to temp files
# Use & to run in background and capture PID
echo "Starting tests in parallel for docker-compose, acceptance, and e2e environments..."
$SCRIPT_DIR/docker-compose-tests.sh > "$TMP_DOCKER_COMPOSE_TESTS" 2>&1 &
PID_DOCKER_COMPOSE_TESTS=$!

$SCRIPT_DIR/acceptance-environment-tests.sh "$VERSION" "$REGION" "$PROJECT" "$ADMIN_PASSWORD" > "$TMP_ACCEPTANCE_TESTS" 2>&1 &
PID_ACCEPTANCE_TESTS=$!

$SCRIPT_DIR/e2e-environment-tests.sh  "$VERSION" "$REGION" "$PROJECT" "$ADMIN_PASSWORD" > "$TMP_E2E_TESTS" 2>&1 &
PID_E2E_TESTS=$!

# Track exit codes
EXIT_DOCKER_COMPOSE_TESTS=0
EXIT_ACCEPTANCE_TESTS=0
EXIT_E2E_TESTS=0

# Wait for all scripts to finish
wait $PID_DOCKER_COMPOSE_TESTS || EXIT_DOCKER_COMPOSE_TESTS=$?
wait $PID_ACCEPTANCE_TESTS || EXIT_ACCEPTANCE_TESTS=$?
wait $PID_E2E_TESTS || EXIT_E2E_TESTS=$?

# Output results in clean blocks
if [ $EXIT_DOCKER_COMPOSE_TESTS -ne 0 ]; then
  echo -e "\n❌ Script docker-compose-tests.sh failed (exit $EXIT_DOCKER_COMPOSE_TESTS):"
else
  echo -e "\n✅ Script docker-compose-tests.sh succeeded:"
fi
cat "$TMP_DOCKER_COMPOSE_TESTS" | indent

if [ $EXIT_ACCEPTANCE_TESTS -ne 0 ]; then
  echo -e "\n❌ Script acceptance-environment-tests.sh failed (exit $EXIT_ACCEPTANCE_TESTS):"
else
  echo -e "\n✅ Script acceptance-environment-tests.sh succeeded:"
fi
cat "$TMP_ACCEPTANCE_TESTS" | indent

if [ $EXIT_E2E_TESTS -ne 0 ]; then
  echo -e "\n❌ Script e2e-environment-tests.sh failed (exit $EXIT_E2E_TESTS):"
else
  echo -e "\n✅ Script e2e-environment-tests.sh succeeded:"
fi
cat "$TMP_E2E_TESTS" | indent

# Clean up
rm -f "$TMP_DOCKER_COMPOSE_TESTS"
rm -f "$TMP_ACCEPTANCE_TESTS"
rm -f "$TMP_E2E_TESTS"

# Final status
TOTAL_EXIT=$(( EXIT_DOCKER_COMPOSE_TESTS ))

echo "$LATEST_SHA" > "$TMP_FILE"
gsutil cp "$TMP_FILE" "gs://$BUCKET_NAME/$LATEST_SHA_PATH"
echo "$TOTAL_EXIT" > "$TMP_FILE"
gsutil cp "$TMP_FILE" "gs://$BUCKET_NAME/$LATEST_EXIT_CODE_PATH"
rm -f "$TMP_FILE"
if [ $TOTAL_EXIT -ne 0 ]; then
  echo "One or more scripts failed."
  exit $TOTAL_EXIT
else
  echo "✅ All scripts completed successfully."
fi

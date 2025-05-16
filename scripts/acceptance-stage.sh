#!/bin/bash
set -euo pipefail

REGION="$1"
PROJECT="$2"

IMAGE_PATH="us-central1-docker.pkg.dev/$PROJECT/ctf-sandbox-repo/ctf-sandbox"
BUCKET_NAME="ctf-sandbox"
OBJECT_PATH="acceptance/latest_sha.txt"
TMP_FILE="/tmp/previous_sha.txt"

echo "Fetching latest image $IMAGE_PATH SHA..."
LATEST_SHA=$(gcloud artifacts docker images list "$IMAGE_PATH" --format="value(version)" --sort-by="~CREATE_TIME" | head -n 1)
echo "Latest SHA: $LATEST_SHA"

if gsutil cp "gs://$BUCKET_NAME/$OBJECT_PATH" "$TMP_FILE" 2>/dev/null; then
    PREVIOUS_SHA=$(cat "$TMP_FILE")
    echo "Previous SHA: $PREVIOUS_SHA"
else
    echo "No previous SHA found"
    PREVIOUS_SHA=""
fi

if [[ "$LATEST_SHA" == "$PREVIOUS_SHA" ]]; then
    echo "No new image to deploy."
    #exit 0
fi

# Start Docker Compose environment
echo "Starting Docker Compose environment..."
docker compose up -d

# Ensure cleanup on exit
cleanup() {
  echo "Cleaning up Docker Compose environment..."
  docker compose down
}
trap cleanup EXIT

# Wait for dev and mailpit containers to be healthy (with timeout)
declare -a services=("dev" "mailpit")
for service in "${services[@]}"; do
  echo "Waiting for $service to be healthy..."
  CONTAINER_ID=$(docker compose ps -q "$service")
  if docker inspect "$CONTAINER_ID" | grep -q '"Health":'; then
    ATTEMPTS=0
    MAX_ATTEMPTS=30
    while true; do
      STATUS=$(docker inspect -f '{{.State.Health.Status}}' "$CONTAINER_ID")
      echo "[$(date)] [$service] Health status: $STATUS (attempt $ATTEMPTS/$MAX_ATTEMPTS)"
      if [ "$STATUS" == "healthy" ]; then
        echo "$service is healthy."
        break
      fi
      if [ $ATTEMPTS -ge $MAX_ATTEMPTS ]; then
        echo "Timed out waiting for $service to become healthy."
        docker compose logs "$service"
        exit 1
      fi
      sleep 2
      ATTEMPTS=$((ATTEMPTS + 1))
    done
  else
    echo "$service has no healthcheck defined; skipping."
  fi
  echo "Finished waiting for $service."
done

# Run tests inside dev container
echo "Running acceptance tests in dev container..."
docker compose exec dev /workspace/scripts/acceptance-stage-tests.sh

# Update SHA in GCS
echo "$LATEST_SHA" > "$TMP_FILE"
gsutil cp "$TMP_FILE" "gs://$BUCKET_NAME/$OBJECT_PATH"
echo "Deployment complete and SHA updated."

#!/bin/bash
set -euo pipefail

VERSION="$1"
REGION="$2"
PROJECT="$3"
ENV="e2e"
ADMIN_PASSWORD="$4"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
EXIT_CODE=0
{
  $SCRIPT_DIR/../release.sh "$VERSION" "$REGION" "$PROJECT" "$ENV" "$ADMIN_PASSWORD"
  # Release External System Test Instances
  # TODO : Complete this section

  # Run e2e tests
  echo "Running acceptance tests on e2e environment..."
  # TODO : Pass parameters to point to e2e environment
  $SCRIPT_DIR/tests.sh Smoke Acceptance ExternalSystemContract E2E

} || EXIT_CODE=$?
echo "Cleaning up e2e environment..."
$SCRIPT_DIR/../teardown.sh $REGION $PROJECT $ENV

exit $EXIT_CODE
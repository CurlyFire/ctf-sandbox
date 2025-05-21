#!/bin/bash
set -euo pipefail

VERSION="$1"
REGION="$2"
PROJECT="$3"
ENV="acc"
ADMIN_PASSWORD="$4"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
EXIT_CODE=0
{
  $SCRIPT_DIR/../release.sh "$VERSION" "$REGION" "$PROJECT" "$ENV" "$ADMIN_PASSWORD"
  # Release external system stubs
  # TODO : Complete this section

  # Run acceptance tests
  echo "Running acceptance tests on acceptance environment..."
  # TODO : Pass parameters to point to acceptance environment
  $SCRIPT_DIR/tests.sh

} || EXIT_CODE=$?
echo "Cleaning up acceptance environment..."
$SCRIPT_DIR/../teardown.sh $REGION $PROJECT $ENV

exit $EXIT_CODE
#!/bin/bash
set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
EXIT_CODE=0
{
  # Start Docker Compose environment
  echo "Starting Docker Compose environment..."
  docker compose up -d

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
          EXIT_CODE=1
          break
        fi
        sleep 2
        ATTEMPTS=$((ATTEMPTS + 1))
      done
    else
      echo "$service has no healthcheck defined; skipping."
    fi
    echo "Finished waiting for $service."
  done

  # Run acceptance tests inside dev container
  echo "Running acceptance tests in dev container..."
  docker compose exec -T dev $SCRIPT_DIR/tests.sh || EXIT_CODE=1
} || EXIT_CODE=$?

echo "Cleaning up Docker Compose environment..."
docker compose down
exit $EXIT_CODE
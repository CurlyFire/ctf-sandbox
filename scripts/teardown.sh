#!/bin/bash
set -euo pipefail

REGION="$1"
PROJECT="$2"
ENV="$3"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "📡 Retrieving project number"
PROJECT_NUMBER=$(gcloud projects describe "$PROJECT" --format='value(projectNumber)')

echo "🧹 Deleting MVC app"
gcloud run services delete mvc-app-$ENV --region=$REGION --quiet || true

echo "🧹 Deleting mailpit-ui"
gcloud run services delete mailpit-ui-$ENV --region=$REGION --quiet || true

echo "🧹 Deleting rqlite"
gcloud run services delete rqlite-$ENV --region=$REGION --quiet || true

echo "🌐 Getting GKE credentials"
CLUSTER_NAME="sandbox-cluster"
gcloud container clusters get-credentials "$CLUSTER_NAME" --region="$REGION"

echo "🧹 Deleting mailpit-smtp service from GKE"
kubectl delete service mailpit-smtp-$ENV || true

echo "🧹 Deleting mailpit-smtp deployment from GKE"
kubectl delete deployment mailpit-smtp-$ENV || true

echo "✅ Teardown complete for environment: $ENV"

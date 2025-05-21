#!/bin/bash
set -e

VERSION="$1"
REGION="$2"
PROJECT="$3"
ENV="$4"
ADMIN_PASSWORD="$5"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
echo "📡 Retrieving project number"
PROJECT_NUMBER=$(gcloud projects describe $PROJECT --format='value(projectNumber)')

### Deploy rqlite

echo "✅ Deploying rqlite (initial)"
gcloud run deploy rqlite-$ENV \
  --image=rqlite/rqlite \
  --port=4001 \
  --region=$REGION \
  --ingress=internal \
  --min-instances=1 \
  --max-instances=1 \
  --allow-unauthenticated \
  --args="--http-addr=0.0.0.0:4001"

RQLITE_URL="rqlite-$ENV-${PROJECT_NUMBER}.$REGION.run.app"
echo "🔁 Updating rqlite HTTP_ADV_ADDR=$RQLITE_URL"
echo "Resolved advertised address: $RQLITE_URL:443"
gcloud run services update rqlite-$ENV \
  --region=$REGION \
  --update-env-vars=HTTP_ADV_ADDR=$RQLITE_URL:443 \
  --args="--http-addr=0.0.0.0:4001"

### Deploy mailpit-ui

echo "✅ Deploying mailpit-ui"
gcloud run deploy mailpit-ui-$ENV \
  --image=axllent/mailpit:latest \
  --port=8025 \
  --region=$REGION \
  --allow-unauthenticated \
  --ingress=all \
  --network=default \
  --subnet=default \
  --vpc-egress=all-traffic \
  --args="--database=https://$RQLITE_URL" \
  --set-env-vars=MP_UI_AUTH="admin:$ADMIN_PASSWORD"


MAILPIT_SA=$(gcloud run services describe mailpit-ui-$ENV \
  --region=$REGION \
  --format='value(spec.template.spec.serviceAccountName)')

gcloud run services add-iam-policy-binding rqlite-$ENV \
  --region=$REGION \
  --member=serviceAccount:$MAILPIT_SA \
  --role=roles/run.invoker

### Ensure GKE cluster

CLUSTER_NAME="sandbox-cluster"
echo "📦 Ensuring GKE cluster exists"
if ! gcloud container clusters describe $CLUSTER_NAME --region=$REGION >/dev/null 2>&1; then
  gcloud container clusters create $CLUSTER_NAME \
    --region=$REGION \
    --num-nodes=1 \
    --disk-type=pd-standard \
    --disk-size=30GB \
    --quiet
fi

echo "🌐 Getting GKE credentials"
gcloud container clusters get-credentials $CLUSTER_NAME --region=$REGION

### Deploy mailpit-smtp to GKE

echo "📦 Rendering and deploying mailpit-smtp to GKE"
export ENV RQLITE_URL
envsubst < $SCRIPT_DIR/templates/release-stage/mailpit-smtp.yaml.tpl > mailpit-smtp.yaml
envsubst < $SCRIPT_DIR/templates/release-stage/mailpit-smtp-service.yaml.tpl > mailpit-smtp-service.yaml

kubectl apply -f mailpit-smtp.yaml
kubectl apply -f mailpit-smtp-service.yaml

# Cleanup
rm mailpit-smtp.yaml
rm mailpit-smtp-service.yaml

MAILPIT_URL="mailpit-ui-$ENV-${PROJECT_NUMBER}.$REGION.run.app"
TIMEOUT=300
INTERVAL=5
ELAPSED=0
until SMTP_IP=$(kubectl get svc mailpit-smtp-$ENV -o jsonpath='{.status.loadBalancer.ingress[0].ip}' 2>/dev/null); do
  echo "Waiting for LoadBalancer IP for mailpit-smtp-$ENV... ($ELAPSED / $TIMEOUT seconds)"
  sleep $INTERVAL
  ELAPSED=$((ELAPSED + INTERVAL))
  if [ $ELAPSED -ge $TIMEOUT ]; then
    echo "❌ Timed out waiting for LoadBalancer IP for mailpit-smtp-$ENV"
    exit 1
  fi
done

### Deploy MVC app

echo "✅ Deploying .NET 9 MVC App"
gcloud run deploy mvc-app-$ENV \
  --image=us-central1-docker.pkg.dev/$PROJECT/ctf-sandbox-repo/ctf-sandbox:$VERSION \
  --region=$REGION \
  --ingress=all \
  --allow-unauthenticated \
  --network=default \
  --subnet=default \
  --vpc-egress=all-traffic \
  --set-env-vars=EmailSettings__SmtpServer="$SMTP_IP",AdminAccount__Password="$ADMIN_PASSWORD",EmailSettings__MailpitUrl="https://$MAILPIT_URL"

echo "✅ Deployment complete"

[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string]$Version,
    
    [Parameter(Mandatory = $true)]
    [string]$Region,
    
    [Parameter(Mandatory = $true)]
    [string]$Project,
    
    [Parameter(Mandatory = $true)]
    [string]$Env,
    
    [Parameter(Mandatory = $true)]
    [string]$AdminPassword
)

$ErrorActionPreference = 'Stop'
$ScriptDir = $PSScriptRoot

Write-Host "📡 Retrieving project number"
$ProjectNumber = (gcloud projects describe $Project --format='value(projectNumber)')

### Deploy rqlite

Write-Host "✅ Deploying rqlite (initial)"
gcloud run deploy "rqlite-$Env" `
    --image=rqlite/rqlite `
    --port=4001 `
    --region=$Region `
    --ingress=internal `
    --min-instances=1 `
    --max-instances=1 `
    --allow-unauthenticated `
    --args="--http-addr=0.0.0.0:4001"

$RqliteUrl = "rqlite-$Env-${ProjectNumber}.$Region.run.app"
Write-Host "🔁 Updating rqlite HTTP_ADV_ADDR=$RqliteUrl"
Write-Host "Resolved advertised address: $RqliteUrl`:443"
gcloud run services update "rqlite-$Env" `
    --region=$Region `
    --update-env-vars=HTTP_ADV_ADDR="$RqliteUrl`:443" `
    --args="--http-addr=0.0.0.0:4001"

### Deploy mailpit-ui

Write-Host "✅ Deploying mailpit-ui"
gcloud run deploy "mailpit-ui-$Env" `
    --image=axllent/mailpit:latest `
    --port=8025 `
    --region=$Region `
    --allow-unauthenticated `
    --ingress=all `
    --network=default `
    --subnet=default `
    --vpc-egress=all-traffic `
    --args="--database=https://$RqliteUrl" `
    --set-env-vars="MP_UI_AUTH=admin:$AdminPassword"

$MailpitSA = (gcloud run services describe "mailpit-ui-$Env" `
    --region=$Region `
    --format='value(spec.template.spec.serviceAccountName)')

gcloud run services add-iam-policy-binding "rqlite-$Env" `
    --region=$Region `
    --member="serviceAccount:$MailpitSA" `
    --role=roles/run.invoker

### Ensure GKE cluster

$ClusterName = "sandbox-cluster"
Write-Host "📦 Ensuring GKE cluster exists"
$ClusterExists = $null
try {
    $null = gcloud container clusters describe $ClusterName --region=$Region 2>$null
    $ClusterExists = $true
} catch {
    $ClusterExists = $false
}

if (-not $ClusterExists) {
    gcloud container clusters create $ClusterName `
        --region=$Region `
        --num-nodes=1 `
        --disk-type=pd-standard `
        --disk-size=30GB `
        --quiet
}

Write-Host "🌐 Getting GKE credentials"
gcloud container clusters get-credentials $ClusterName --region=$Region

### Deploy mailpit-smtp to GKE

Write-Host "📦 Rendering and deploying mailpit-smtp to GKE"
$env:ENV = $Env
$env:RQLITE_URL = $RqliteUrl

Get-Content "$ScriptDir/templates/release-stage/mailpit-smtp.yaml.tpl" | ForEach-Object {
    $ExecutionContext.InvokeCommand.ExpandString($_)
} | Set-Content -Path "mailpit-smtp.yaml"

Get-Content "$ScriptDir/templates/release-stage/mailpit-smtp-service.yaml.tpl" | ForEach-Object {
    $ExecutionContext.InvokeCommand.ExpandString($_)
} | Set-Content -Path "mailpit-smtp-service.yaml"

kubectl apply -f mailpit-smtp.yaml
kubectl apply -f mailpit-smtp-service.yaml

# Cleanup
Remove-Item -Path mailpit-smtp.yaml
Remove-Item -Path mailpit-smtp-service.yaml

$MailpitUrl = "mailpit-ui-$Env-${ProjectNumber}.$Region.run.app"
$Timeout = 300
$Interval = 5
$Elapsed = 0
$SmtpIp = $null

while ($true) {
    try {
        $SmtpIp = kubectl get svc "mailpit-smtp-$Env" -o jsonpath='{.status.loadBalancer.ingress[0].ip}' 2>$null
        if ($SmtpIp) { break }
    } catch {
        # Continue if the command fails
    }

    Write-Host "Waiting for LoadBalancer IP for mailpit-smtp-$Env... ($Elapsed / $Timeout seconds)"
    Start-Sleep -Seconds $Interval
    $Elapsed += $Interval
    if ($Elapsed -ge $Timeout) {
        Write-Host "❌ Timed out waiting for LoadBalancer IP for mailpit-smtp-$Env"
        exit 1
    }
}

### Deploy MVC app

Write-Host "✅ Deploying .NET 9 MVC App"
gcloud run deploy "mvc-app-$Env" `
    --image="us-central1-docker.pkg.dev/$Project/ctf-sandbox-repo/ctf-sandbox:$Version" `
    --region=$Region `
    --ingress=all `
    --allow-unauthenticated `
    --network=default `
    --subnet=default `
    --vpc-egress=all-traffic `
    --set-env-vars="EmailSettings__SmtpServer=$SmtpIp,AdminAccount__Password=$AdminPassword,EmailSettings__MailpitUrl=https://$MailpitUrl"

Write-Host "✅ Deployment complete"
#!/usr/bin/env pwsh
param(
    [Parameter(Mandatory=$true)]
    [string]$Region,
    
    [Parameter(Mandatory=$true)]
    [string]$Project,
    
    [Parameter(Mandatory=$true)]
    [string]$Env
)

# Ensure script fails on errors
$ErrorActionPreference = "Stop"

Write-Host "🧹 Deleting MVC app"
gcloud run services delete "mvc-app-$Env" --region=$Region --quiet
if ($LASTEXITCODE -ne 0) { Write-Host "Note: MVC app service deletion failed or service didn't exist" }

Write-Host "🧹 Deleting mailpit-ui"
gcloud run services delete "mailpit-ui-$Env" --region=$Region --quiet
if ($LASTEXITCODE -ne 0) { Write-Host "Note: Mailpit UI service deletion failed or service didn't exist" }

Write-Host "🧹 Deleting rqlite"
gcloud run services delete "rqlite-$Env" --region=$Region --quiet
if ($LASTEXITCODE -ne 0) { Write-Host "Note: rqlite service deletion failed or service didn't exist" }

Write-Host "🌐 Getting GKE credentials"
$ClusterName = "sandbox-cluster"
gcloud container clusters get-credentials $ClusterName --region=$Region

Write-Host "🧹 Deleting mailpit-smtp service from GKE"
kubectl delete service "mailpit-smtp-$Env"
if ($LASTEXITCODE -ne 0) { Write-Host "Note: Mailpit SMTP service deletion failed or service didn't exist" }

Write-Host "🧹 Deleting mailpit-smtp deployment from GKE"
kubectl delete deployment "mailpit-smtp-$Env"
if ($LASTEXITCODE -ne 0) { Write-Host "Note: Mailpit SMTP deployment deletion failed or deployment didn't exist" }

Write-Host "✅ Teardown complete for environment: $Env"

$ErrorActionPreference = "Stop"

# Import shared module
Import-Module (Join-Path $env:WORKSPACE_ROOT "pipelines/shared/CICD.psm1") -Force

Write-Log "🚀 Starting tear down all gcloud"

Remove-AllGCloudRessources
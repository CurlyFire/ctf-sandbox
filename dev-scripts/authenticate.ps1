Import-Module (Join-Path $env:WORKSPACE_ROOT "pipelines/shared/CICD.psm1") -Force

# Check if user is authenticated with gcloud
$authStatus = gcloud auth list --filter=status:ACTIVE --format="value(account)" 2>$null

if ([string]::IsNullOrWhiteSpace($authStatus)) {
    Write-Host "Not authenticated with gcloud. Launching authentication..." -ForegroundColor Yellow
    gcloud auth login
    $config = Get-CICDConfig
    gcloud config set project $config.GoogleCloud.ProjectId
    $dockerRepository = $config.App.DockerImageName.Split("/")[0]
    gcloud auth configure-docker "$dockerRepository" --quiet

    gh auth login --web
} else {
    Write-Host "Already authenticated with gcloud as: $authStatus" -ForegroundColor Green
}
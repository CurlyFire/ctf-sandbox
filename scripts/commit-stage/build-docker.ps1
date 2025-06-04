[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectId,
    
    [Parameter(Mandatory=$true)]
    [string]$ImageTag
)

$imageName = "us-central1-docker.pkg.dev/$ProjectId/ctf-sandbox-repo/ctf-sandbox:$ImageTag"
Write-Host "Building Docker image: $imageName"
docker build -t $imageName .

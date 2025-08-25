#!/usr/bin/env pwsh
param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$AdminPassword,

    [Parameter(Mandatory = $true)]
    [string]$IpInfoToken,

    [switch]$PushImage
)

$ErrorActionPreference = "Stop"

# Import shared module
Import-Module (Join-Path $env:WORKSPACE_ROOT "pipelines/shared/CICD.psm1") -Force

Write-Log "🚀 Starting commit stage"

# 2. Run tests
Invoke-LocalTests -AdminPassword $AdminPassword -IpInfoToken $IpInfoToken -Stage "commit"

# 3. Publish .NET app
Publish-DotNetApp

# 4. Build Docker image
Build-DockerImage -Version $Version

# 5. Optionally push Docker image
if ($PushImage.IsPresent) {
    Push-DockerImage -Version $Version
}

Write-Log "✅ Commit stage completed successfully"

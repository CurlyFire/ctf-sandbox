#!/usr/bin/env pwsh
param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [switch]$PushImage
)

$ErrorActionPreference = "Stop"

# 1. Import shared module
Import-Module (Join-Path $env:WORKSPACE_ROOT "pipelines/shared/CICD.psm1") -Force

Write-Log "🚀 Starting commit stage"

# 2. Run tests
Invoke-Tests -Stage "commit"

# 3. Publish .NET app
Publish-DotNetApp

# 4. Build Docker image
Build-DockerImage -Version $Version

# 5. Optionally push Docker image
if ($PushImage.IsPresent) {
    Push-DockerImage -Version $Version
}

Write-Log "✅ Commit stage completed successfully"

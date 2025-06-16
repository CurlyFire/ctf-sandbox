#!/usr/bin/env pwsh
param(
    [Parameter(Mandatory = $true)]
    [string]$Version,
    [switch]$PushImage
)

$ErrorActionPreference = "Stop"

# Import shared module
Import-Module (Join-Path $env:WORKSPACE_ROOT "scripts/shared/CICD.psm1") -Force

# Get config
$config = Get-CICDConfig
$workspaceRoot = $env:WORKSPACE_ROOT

Write-Log "🚀 Starting commit stage"

# 1. Build the solution
Build-DotNetSolution

# 2. Run tests for commit stage categories
Invoke-Tests -Stage "commit"

# 3. Publish .NET app
Publish-DotNetApp



Build-DockerImage -Version $Version

# 5. Optionally push Docker image
if ($PushImage.IsPresent) {
    Push-DockerImage -Version $Version
}

Write-Log "✅ Commit stage completed successfully"

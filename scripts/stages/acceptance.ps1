#!/usr/bin/env pwsh
param(
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = "Stop"

$workspace = $env:WORKSPACE_ROOT
Import-Module (Join-Path $workspace "scripts/shared/CICD.psm1") -Force

Write-Log "🚀 Starting acceptance stage for version $Version"

if (Test-IsShaAlreadyProcessed -Version $Version) {
    Write-Log "⚠️ SHA $Version already tested. Skipping."
    return
}

$environments = @(
    New-GCloudEnvironment -Name "acceptance" -Version $Version -AdminPassword New-RandomPassword
    New-DockerEnvironment -Name "docker" -Version $Version -AdminPassword New-RandomPassword
)

Build-DotNetSolution
try {
    foreach ($env in $environments) {
        $env.Deploy()
        Invoke-Tests -Stage "acceptance" -Env $env.Name
    }

    Write-Log "✅ Acceptance stage completed successfully"
}
finally {
    Write-Log "🧹 Tearing down environments..."
    foreach ($env in $environments) {
        $env.Teardown()
    }

    try {
        Register-TestedSha -Version $Version
        Write-Log "📌 SHA $Version has been registered as tested (regardless of outcome)"
    } catch {
        Write-Log "⚠️ Failed to register tested SHA: $_"
    }
}

#!/usr/bin/env pwsh
param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$AdminPassword,

    [Parameter(Mandatory = $true)]
    [string]$IpInfoToken
)

$ErrorActionPreference = "Stop"

$workspace = $env:WORKSPACE_ROOT
Import-Module (Join-Path $workspace "pipelines/shared/CICD.psm1") -Force

Write-Log "🚀 Starting acceptance stage for version $Version"

if (Test-IsShaAlreadyProcessed -Version $Version) {
    Write-Log "⚠️ SHA $Version already tested. Skipping."
    #return
}
Build-DotNetSolution

$environments = @(
    New-DockerEnvironment -Name "docker" -Version $Version -AdminPassword $AdminPassword -IpInfoToken $IpInfoToken
    New-GCloudEnvironment -Name "acceptance" -Version $Version -AdminPassword $AdminPassword -IpInfoToken $IpInfoToken
    New-GCloudEnvironment -Name "e2e" -Version $Version -AdminPassword $AdminPassword -IpInfoToken $IpInfoToken
)

try {
    foreach ($env in $environments) {
        $envConfig = $env.Deploy()
        Invoke-Tests -Stage "acceptance" -Env $env.Name -EnvConfig $envConfig
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

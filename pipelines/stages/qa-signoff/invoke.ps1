#!/usr/bin/env pwsh
[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string]$Version,
    [switch]$Success
)

$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $env:WORKSPACE_ROOT "pipelines/shared/CICD.psm1") -Force

Write-Log "🚀 Starting QA-Signoff stage"
$validSuffix = "-rc"
# Validate version suffix
if (-not $Version.EndsWith($validSuffix)) {
    throw "Version '$Version' must end with the valid suffix '$validSuffix'"
}

if ($Success)
{
    if (Test-IsVersionAlreadyDeployed -Version $Version -Env "qa")
    {
        Publish-StableRelease -Version $Version
        Write-Log "✅ QA Signoff successful for version $Version"
    }
    else
    {
        throw "Version '$Version' has not been deployed to QA environment, cannot sign off"
    }
}
else
{
    Write-Log "❌ QA Signoff failed for version $Version"
}
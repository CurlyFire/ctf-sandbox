#!/usr/bin/env pwsh
[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string]$Version,
    [Parameter(Mandatory = $true)]
    [bool]$Success
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
    Write-Log "✅ QA Signoff successful for version $Version"
    Publish-PreRelease -CommitSha "d2bde12a1aebe3db2838fa952e7a2674c3272eac"
    #Publish-StableRelease -Version $Version
}
else
{
    Write-Log "❌ QA Signoff failed for version $Version"
}
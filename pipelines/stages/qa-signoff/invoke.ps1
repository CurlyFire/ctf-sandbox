#!/usr/bin/env pwsh
[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string]$Version
)

$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $env:WORKSPACE_ROOT "pipelines/shared/CICD.psm1") -Force

Write-Log "🚀 Starting QA-Signoff stage"
$validSuffix = "-rc"
# Validate version suffix
if (-not $Version.EndsWith($validSuffix)) {
    throw "Version '$Version' must end with the valid suffix '$validSuffix'"
}

Publish-StableRelease -Version $Version
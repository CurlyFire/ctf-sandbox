#!/usr/bin/env pwsh
[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$Env,

    [Parameter(Mandatory = $true)]
    [string]$WebAdminAccount,
    
    [Parameter(Mandatory = $true)]
    [string]$MailpitAdminAccount,

    [Parameter(Mandatory = $true)]
    [string]$AdminPassword,

    [Parameter(Mandatory = $true)]
    [string]$IpInfoToken,

    [Parameter(Mandatory = $true)]
    [string]$ValidSuffix
)

$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $env:WORKSPACE_ROOT "pipelines/shared/CICD.psm1") -Force

# Validate version suffix
if ($ValidSuffix -eq "") {
    # When valid suffix is empty, ensure version has no suffix like -rc, -beta, -alpha, etc.
    if ($Version -match '-') {
        throw "Version '$Version' must not contain a suffix when ValidSuffix is empty"
    }
} elseif (-not $Version.EndsWith($ValidSuffix)) {
    throw "Version '$Version' must end with the valid suffix '$ValidSuffix'"
}

Write-Log "🚀 Starting release stage"

$environment = Deploy-GCloudEnvironment -Name $Env -Version $Version -WebAdminAccount $WebAdminAccount -MailpitAdminAccount $MailpitAdminAccount -AdminPassword $AdminPassword -IpInfoToken $IpInfoToken
Invoke-Tests -Stage "release" -EnvironmentName $Env -GCloudEnvironment $environment
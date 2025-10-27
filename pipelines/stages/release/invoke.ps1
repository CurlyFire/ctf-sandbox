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
    [AllowEmptyString()]
    [string]$ValidSuffix
)

$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $env:WORKSPACE_ROOT "pipelines/shared/CICD.psm1") -Force

# Validate version and its suffix
Test-VersionSuffix -Version $Version -ValidSuffix $ValidSuffix

Write-Log "🚀 Starting release stage"

$environment = Deploy-GCloudEnvironment -Name $Env -Version $Version -WebAdminAccount $WebAdminAccount -MailpitAdminAccount $MailpitAdminAccount -AdminPassword $AdminPassword -IpInfoToken $IpInfoToken
Register-DeployedVersion -Version $Version -Env $Env
Invoke-Tests -Stage "release" -EnvironmentName $Env -GCloudEnvironment $environment
#!/usr/bin/env pwsh
[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$Env,

    [Parameter(Mandatory = $true)]
    [string]$WebServerAdminAccount,
    
    [Parameter(Mandatory = $true)]
    [string]$MailpitAdminAccount,

    [Parameter(Mandatory = $true)]
    [string]$AdminPassword,

    [Parameter(Mandatory = $true)]
    [string]$IpInfoToken
)

$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $env:WORKSPACE_ROOT "pipelines/shared/CICD.psm1") -Force

Write-Log "🚀 Starting release stage"

$environment = Deploy-GCloudEnvironment -Name $Env -Version $Version -WebServerAdminAccount $WebServerAdminAccount -MailpitAdminAccount $MailpitAdminAccount -AdminPassword $AdminPassword -IpInfoToken $IpInfoToken
Invoke-Tests -Stage "release" -EnvironmentName $Env -GCloudEnvironment $environment
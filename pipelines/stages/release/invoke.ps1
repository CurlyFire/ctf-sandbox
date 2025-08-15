#!/usr/bin/env pwsh
[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$Env,
    
    [Parameter(Mandatory = $true)]
    [string]$AdminPassword,

    [Parameter(Mandatory = $true)]
    [string]$IpInfoToken
)

$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $env:WORKSPACE_ROOT "pipelines/shared/CICD.psm1") -Force

Write-Log "🚀 Starting release stage"

$envConfig = Deploy-GCloudEnvironment -Name $Env -Version $Version -AdminPassword $AdminPassword -IpInfoToken $IpInfoToken

Invoke-Tests -Stage "release" -Env $Env -EnvConfig $envConfig
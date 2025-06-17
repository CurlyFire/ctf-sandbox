#!/usr/bin/env pwsh
[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$Env,
    
    [Parameter(Mandatory = $true)]
    [string]$AdminPassword
)

$ErrorActionPreference = 'Stop'

Import-Module (Join-Path $env:WORKSPACE_ROOT "scripts/shared/CICD.psm1") -Force

$environment = New-GCloudEnvironment -Name $Env -Version $Version -AdminPassword $AdminPassword
$environment.Deploy()

Invoke-Tests -Stage "release" -Env $Env
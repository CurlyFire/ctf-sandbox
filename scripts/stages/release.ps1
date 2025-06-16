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
$config = Get-CICDConfig

Deploy-GCloudEnv -Env $Env -Version $Version -AdminPassword $AdminPassword
Invoke-Tests -Env $Env -Categories $config.TestCategoriesPerStage.Release
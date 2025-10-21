#!/usr/bin/env pwsh
param(
    [Parameter(Mandatory = $true)]
    [string]$CommitSha,

    [Parameter(Mandatory = $true)]
    [string]$WebAdminAccount,
    
    [Parameter(Mandatory = $true)]
    [string]$MailpitAdminAccount,    

    [Parameter(Mandatory = $true)]
    [string]$AdminPassword,

    [Parameter(Mandatory = $true)]
    [string]$IpInfoToken,

    [Parameter(Mandatory = $false)]
    [switch]$Force
)

$ErrorActionPreference = "Stop"

$workspace = $env:WORKSPACE_ROOT
Import-Module (Join-Path $workspace "pipelines/shared/CICD.psm1") -Force

Write-Log "🚀 Starting acceptance stage for version $CommitSha"

if ($Force) {
    Write-Log "⚠️ Execution is forced, ignoring previous test results."
}
elseif (Test-IsShaAlreadyProcessed -Version $CommitSha) {
    Write-Log "⚠️ SHA $CommitSha already tested. Skipping."
    return
}

$environmentNamesToDeploy = @("acceptance","e2e")
$deployedEnvironmentNames = @()
try {
    foreach ($name in $environmentNamesToDeploy) {
        $deployedEnvironmentNames += $name
        $environment = Deploy-GCloudEnvironment -Name $name -Version $CommitSha -WebAdminAccount $WebAdminAccount -MailpitAdminAccount $MailpitAdminAccount -AdminPassword $AdminPassword -IpInfoToken $IpInfoToken
        Invoke-Tests -Stage "acceptance" -EnvironmentName $environment.Name -GCloudEnvironment $environment
    }
    Publish-PreRelease -CommitSha $CommitSha
    Write-Log "✅ Acceptance stage completed successfully"
}
finally {
    Write-Log "🧹 Tearing down environments..."
    foreach ($name in $deployedEnvironmentNames) {
        try {
            Remove-GCloudEnvironment -Name $name
        } catch {
            Write-Log "⚠️ Failed to remove environment $name): $_"
        }
    }


    try {
        Register-TestedSha -Version $CommitSha
        Write-Log "📌 SHA $CommitSha has been registered as tested (regardless of outcome)"
    } catch {
        Write-Log "⚠️ Failed to register tested SHA: $_"
    }
}

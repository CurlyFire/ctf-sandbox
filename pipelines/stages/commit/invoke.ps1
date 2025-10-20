#!/usr/bin/env pwsh
param(
    [switch]$TagAndPush
)

$ErrorActionPreference = "Stop"

# 1. Import shared module
Import-Module (Join-Path $env:WORKSPACE_ROOT "pipelines/shared/CICD.psm1") -Force

Write-Log "🚀 Starting commit stage"

# 2. Run tests
Invoke-Tests -Stage "commit"

# 3. Calculate semantic version
$version = Get-SemanticVersion -PreReleaseTag "beta"

# 4. Publish .NET app
Publish-DotNetApp

# 5. Build Docker image
Build-DockerImage -Version $version

# 6. Optionally create Git tag and push to remote
if ($TagAndPush.IsPresent) {
    Invoke-TagAndPush -Version $version
}

Write-Log "✅ Commit stage completed successfully"

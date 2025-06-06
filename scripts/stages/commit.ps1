#!/usr/bin/env pwsh
param(
    [Parameter(Mandatory = $true)]
    [string]$Version,
    [switch]$PushImage
)

$ErrorActionPreference = "Stop"

# Import shared module
Import-Module (Join-Path $env:WORKSPACE_ROOT "scripts/shared/CICD.psm1") -Force

# Get config
$config = Get-CICDConfig
$workspaceRoot = $env:WORKSPACE_ROOT

Write-Log "🚀 Starting commit stage"

# 1. Build the solution
Build-DotNetSolution -SolutionPath (Join-Path $workspaceRoot $Config.DotnetSolution)

# 2. Run tests for commit stage categories
Invoke-Tests -Categories $Config.TestCategoriesPerStage.Commit -SolutionPath (Join-Path $workspaceRoot $Config.DotnetSolution)

# 3. Publish .NET app
$projectPath = Join-Path $WorkspaceRoot $config.DotnetProject
$publishDir  = Join-Path $WorkspaceRoot $config.PublishDir
Publish-DotNetApp -ProjectPath $projectPath -OutputPath $publishDir

# 4. Build Docker image
$versionedTag = "$($config.DockerImageName):$Version"

Build-DockerImage -Tag $versionedTag -ContextPath $workspaceRoot

# 5. Optionally push Docker image
if ($PushImage.IsPresent) {
    Push-DockerImage -Tag $versionedTag
}

Write-Log "✅ Commit stage completed successfully"

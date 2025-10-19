#!/usr/bin/env pwsh
param(
    [switch]$PushImage
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

# 6. Optionally push Docker image with Git tagging
if ($PushImage.IsPresent) {
    $tagName = $version
    $tagCreated = $false
    
    try {
        # Create and push Git tag
        Write-Log "🏷️  Creating Git tag: $tagName"
        git tag $tagName
        $tagCreated = $true
        
        Write-Log "📤 Pushing tag to origin..."
        git push origin $tagName
        
        # Push Docker image
        Push-DockerImage -Version $version
        
        Write-Log "✅ Successfully tagged and pushed version $version"
    }
    catch {
        Write-Log "❌ Failed to push Docker image. Rolling back Git tag..." -Level "Error"
        
        if ($tagCreated) {
            # Delete remote tag if it was pushed
            Write-Log "🔄 Deleting remote tag: $tagName"
            git push origin --delete $tagName 2>$null
            
            # Delete local tag
            Write-Log "🔄 Deleting local tag: $tagName"
            git tag -d $tagName
        }
        
        throw
    }
}

Write-Log "✅ Commit stage completed successfully"

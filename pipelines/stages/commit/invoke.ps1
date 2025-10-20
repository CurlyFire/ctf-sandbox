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
    $tagName = $version
    
    try {
        Write-Log "🏷️  Creating local Git tag: $tagName"
        git tag $tagName
        
        Write-Log "📤 Pushing tag to origin..."
        git push origin $tagName

        Write-Log "📤 Pushing Docker image to registry..."
        Push-DockerImage -Version $version
        
        Write-Log "✅ Successfully tagged and pushed version $version"
    }
    catch {
        Write-Log "❌ Failed to push. Rolling back operations..." -Level "Error"
        
        # Delete remote tag if it was pushed
        Write-Log "🔄 Deleting remote tag: $tagName"
        git push origin --delete $tagName 2>$null
        
        # Delete local tag
        Write-Log "🔄 Deleting local tag: $tagName"
        git tag -d $tagName
        
        throw
    }
}

Write-Log "✅ Commit stage completed successfully"

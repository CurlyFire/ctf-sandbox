if (-not $env:WORKSPACE_ROOT) {
    Write-Error "❌ WORKSPACE_ROOT environment variable is not set. Aborting setup."
    exit 1
}
$workspaceRoot = $env:WORKSPACE_ROOT

Write-Host "🔐 Marking workspace as safe for Git..."
git config --global --add safe.directory $workspaceRoot

Write-Host "🔧 Installing dotnet-ef tool..."
dotnet tool install --global dotnet-ef --version 9.0.11

Write-Host "✅ postCreateCommand script complete."
if (-not $env:WORKSPACE_ROOT) {
    Write-Error "❌ WORKSPACE_ROOT environment variable is not set. Aborting setup."
    exit 1
}
$workspaceRoot = $env:WORKSPACE_ROOT

Write-Host "🔐 Marking workspace as safe for Git..."
git config --global --add safe.directory $workspaceRoot

Write-Host "🔧 Installing dotnet-ef tool..."
dotnet tool install --global dotnet-ef

Write-Host "Installing Playwright browsers..."
Set-Location tests/ctf-sandbox.tests && dotnet build && ./bin/Debug/net9.0/playwright.ps1 install-deps && ./bin/Debug/net9.0/playwright.ps1 install

Write-Host "✅ postCreateCommand script complete."
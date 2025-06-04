$scriptPath = "/workspaces/ctf-sandbox/scripts/repo-tools.ps1"

if (Test-Path $scriptPath) {
    . $scriptPath
    Write-Host "✅ repo-tools.ps1 loaded (DevContainer)"
}

git config --global --add safe.directory /workspaces/ctf-sandbox
dotnet tool install --global dotnet-ef
mkdir -p ~/.config/powershell
cp /workspaces/ctf-sandbox/.devcontainer/pwsh-profile.ps1  ~/.config/powershell/Microsoft.PowerShell_profile.ps1
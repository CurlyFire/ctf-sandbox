#!/bin/bash
set -euo pipefail

echo "✅ Setting Git safe directory for workspace..."
git config --global --add safe.directory /workspace

echo "✅ Installing dotnet-ef tool globally..."
dotnet tool install --global dotnet-ef

echo "✅ Installing Playwright browsers..."
cd tests/ctf-sandbox.tests && dotnet build && pwsh ./bin/Debug/net9.0/playwright.ps1 install-deps && pwsh ./bin/Debug/net9.0/playwright.ps1 install

echo "🎉 Devcontainer setup complete."

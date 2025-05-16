#!/usr/bin/env bash
cd /workspace
# Run acceptance stage tests
dotnet build ./src --configuration Release
dotnet test ./src --no-build --configuration Release --filter "Category=Smoke"
dotnet test ./src --no-build --configuration Release --filter "Category=Acceptance"
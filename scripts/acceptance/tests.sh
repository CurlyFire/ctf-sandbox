#!/usr/bin/env bash
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# Run acceptance stage tests
dotnet build $SCRIPT_DIR/../../src --configuration Release
dotnet test $SCRIPT_DIR/../../src --no-build --configuration Release --filter "Category=Smoke"
dotnet test $SCRIPT_DIR/../../src --no-build --configuration Release --filter "Category=Acceptance"
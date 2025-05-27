#!/usr/bin/env bash
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Default categories if none provided
if [ $# -eq 0 ]; then
    CATEGORIES=("Smoke" "Acceptance")
else
    CATEGORIES=("$@")
fi

# Build the project first
dotnet build $SCRIPT_DIR/../../src --configuration Release

# Run tests for each category
for category in "${CATEGORIES[@]}"; do
    echo "Running tests for category: $category"
    dotnet test $SCRIPT_DIR/../../src --no-build --configuration Release --filter "Category=$category"
done
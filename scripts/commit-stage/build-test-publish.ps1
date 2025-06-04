# Define test categories
$testCategories = @(
    @{
        Name = "Unit"
        DisplayName = "unit tests"
    },
    @{
        Name = "NarrowIntegration"
        DisplayName = "narrow integration tests"
    },
    @{
        Name = "Component"
        DisplayName = "component tests"
    },
    @{
        Name = "ContractVerification"
        DisplayName = "contract verification tests"
    }
)

$sourceRoot = Join-Path (Get-RepoRoot) "src"

Write-Host "Building solution..."
dotnet build -c Release $sourceRoot

# Run all test categories
foreach ($category in $testCategories) {
    Write-Host "Running $($category.DisplayName)..."
    dotnet test --no-build -c Release --logger "trx;LogFilePath=$($category.Name.ToLower())-test-results.trx" --filter "Category=$($category.Name)" $sourceRoot
}

Write-Host "Publishing UI..."
$projectPath = Join-Path $sourceRoot "ctf-sandbox.csproj"

dotnet publish --no-build -c Release -o ./publish $projectPath
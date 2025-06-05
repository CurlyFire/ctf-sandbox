[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$Name,

    [Parameter(Mandatory = $true)]
    [string]$Tag,

    [switch]$Push
)

if (-not $env:WORKSPACE_ROOT) {
    Write-Error "❌ WORKSPACE_ROOT environment variable is not set. Aborting..."
    exit 1
}
$workspaceRoot = $env:WORKSPACE_ROOT

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

$sourceRoot = Join-Path $workspaceRoot "src"

Write-Host "Building solution..."
dotnet build -c Release $sourceRoot

# Run all test categories
foreach ($category in $testCategories) {
    Write-Host "Running $($category.DisplayName)..."
    dotnet test --no-build -c Release --logger "trx;LogFilePath=$($category.Name.ToLower())-test-results.trx" --filter "Category=$($category.Name)" $sourceRoot
}

Write-Host "Publishing UI..."
$projectPath = Join-Path $sourceRoot "ctf-sandbox.csproj"
$publishPath = Join-Path $workspaceRoot "publish"

dotnet publish --no-build -c Release -o $publishPath $projectPath

$fullImage = "${Name}:${Tag}"

Write-Host "🐳 Building Docker image: $fullImage"
docker build -t $fullImage $workspaceRoot

if ($Push) {
    Write-Host "📤 Pushing Docker image to registry..."
    docker push $fullImage
} else {
    Write-Host "🛑 Push skipped (use -Push to enable)"
}
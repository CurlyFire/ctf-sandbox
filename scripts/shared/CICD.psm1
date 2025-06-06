# scripts/shared/CICD.psm1

function Write-Log {
    param([string]$Message)
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "[$timestamp] $Message"
}

function Build-DotNetSolution {
    param(
        [string]$SolutionPath
    )
    Write-Log "Building .NET solution at $SolutionPath"
    dotnet build $SolutionPath -c Release
}

function Invoke-Tests {
    param(
        [string[]]$Categories,
        [string]$SolutionPath
    )

    foreach ($category in $Categories) {
        Write-Log "Running tests for category: $category"
        dotnet test $SolutionPath --no-build -c Release --filter "Category=$category"
    }
}

function Publish-DotNetApp {
    param(
        [string]$ProjectPath,
        [string]$OutputPath
    )
    Write-Log "Publishing .NET project $ProjectPath to $OutputPath"
    dotnet publish $ProjectPath -c Release -o $OutputPath
}

function Build-DockerImage {
    param(
        [string]$Tag,
        [string]$ContextPath = "."
    )
    Write-Log "Building Docker image: $Tag in folder $ContextPath"
    docker build -t $Tag $ContextPath
}

function Push-DockerImage {
    param([string]$Tag)
    Write-Log "Pushing Docker image: $Tag"
    docker push $Tag
}

function Get-CICDConfig {
    $configPath = Join-Path $env:WORKSPACE_ROOT "scripts/shared/config.psd1"
    return Import-PowerShellDataFile $configPath
}

Export-ModuleMember -Function *
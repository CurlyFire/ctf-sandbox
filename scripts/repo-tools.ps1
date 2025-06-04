function Get-GitRoot {
    $gitRoot = git rev-parse --show-toplevel 2>$null
    if (-not $gitRoot) {
        throw "Not inside a Git repository"
    }
    return $gitRoot
}

function Get-RepoRootFromPath {
    param([string]$StartPath)

    $dir = Resolve-Path $StartPath
    while (-not (Test-Path (Join-Path $dir ".git"))) {
        $parent = Split-Path -Parent $dir
        if ($parent -eq $dir) {
            throw "Cannot find .git directory upward from $StartPath"
        }
        $dir = $parent
    }
    return $dir
}

function Get-RepoRoot {
    return Get-RepoRootFromPath $PSScriptRoot
}

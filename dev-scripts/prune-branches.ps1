Write-Host "Pruning remote-tracking branches..." -ForegroundColor Cyan
# Delete remote-tracking branches that no longer exist on the remote
git fetch --prune

# Delete local Git branches that don't have a corresponding remote branch

# Get the current branch to avoid deleting it
$currentBranch = git rev-parse --abbrev-ref HEAD

# Fetch and prune to update remote references
Write-Host "Fetching and pruning remote references..." -ForegroundColor Cyan
git fetch --prune

# Get all local branches
$localBranches = git branch --format='%(refname:short)'

# Get all remote branches (without the remote name prefix)
$remoteBranches = git branch -r | ForEach-Object { 
    $_.Trim() -replace '^origin/', '' 
} | Where-Object { $_ -notlike '*HEAD*' }

Write-Host "`nAnalyzing branches..." -ForegroundColor Cyan

$branchesToDelete = @()

foreach ($branch in $localBranches) {
    # Skip the current branch
    if ($branch -eq $currentBranch) {
        Write-Host "  Skipping current branch: $branch" -ForegroundColor Yellow
        continue
    }
    
    # Check if branch exists on remote
    if ($remoteBranches -notcontains $branch) {
        $branchesToDelete += $branch
        Write-Host "  Found branch without remote: $branch" -ForegroundColor Red
    }
}

if ($branchesToDelete.Count -eq 0) {
    Write-Host "`nNo local branches without remotes found." -ForegroundColor Green
    exit 0
}

Write-Host "`nFound $($branchesToDelete.Count) branch(es) without remotes:" -ForegroundColor Yellow
$branchesToDelete | ForEach-Object { Write-Host "  - $_" }

# Ask for confirmation
$confirmation = Read-Host "`nDo you want to delete these branches? (yes/no)"

if ($confirmation -eq 'yes' -or $confirmation -eq 'y') {
    Write-Host "`nDeleting branches..." -ForegroundColor Cyan
    foreach ($branch in $branchesToDelete) {
        git branch -D $branch
        Write-Host "  Deleted: $branch" -ForegroundColor Green
    }
    Write-Host "`nDone! Deleted $($branchesToDelete.Count) branch(es)." -ForegroundColor Green
} else {
    Write-Host "`nOperation cancelled." -ForegroundColor Yellow
}
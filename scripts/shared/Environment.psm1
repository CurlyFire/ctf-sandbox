class Environment {
    [string]$Name
    [string]$Version
    [string]$AdminPassword

    Environment([string]$name, [string]$version, [string]$adminPassword) {
        $this.Name = $name
        $this.Version = $version
        $this.AdminPassword = $adminPassword
    }

    [void] Deploy() {
        Write-Log "🚀 Deploying environment $($this.Name) with version $($this.Version)"
        Deploy-GCloudEnv -Env $this.Name -Version $this.Version -AdminPassword $this.AdminPassword
    }

    [void] RunTests([string[]]$categories) {
        Write-Log "🧪 Running tests for environment $($this.Name)"
        Invoke-Tests -Env $this.Name -Categories $categories
    }

    [void] Teardown() {
        Write-Log "🧹 Tearing down environment $($this.Name)"
        try {
            Remove-GCloudEnv -Env $this.Name
        }
        catch {
            Write-Log "⚠️ Failed to teardown GCloud $($this.Name) env: $_"
        }
    }
}

Export-ModuleMember -Function * -Variable * -Alias *

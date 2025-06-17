class CICDConfig {
    [string]$ProjectId
    [string]$Region
    [string]$BucketName
    [string]$DockerImageName
    [string]$DotnetSolution
    [string]$DotnetProject
    [string]$DockerComposeFile
    [string]$Dockerfile
    [string]$PublishDir
    [hashtable]$TestCategories

    CICDConfig([hashtable]$rawConfig) {
        $this.ProjectId         = $rawConfig.ProjectId
        $this.Region            = $rawConfig.Region
        $this.BucketName        = $rawConfig.BucketName
        $this.DockerImageName   = $rawConfig.DockerImageName
        $this.DotnetSolution    = $rawConfig.DotnetSolution
        $this.DotnetProject     = $rawConfig.DotnetProject
        $this.DockerComposeFile = $rawConfig.DockerComposeFile
        $this.PublishDir        = $rawConfig.PublishDir
        $this.TestCategories    = $rawConfig.TestCategories
        $this.Dockerfile        = $rawConfig.Dockerfile
    }

    [string[]] GetTestCategories([string]$stage, [string]$env = "Default") {
    if (-not $this.TestCategories.ContainsKey($stage)) {
        throw "❌ Stage '$stage' is not defined in the configuration."
    }

    $stageTable = $this.TestCategories[$stage]
    if (-not $stageTable.ContainsKey($env)) {
        throw "❌ Environment '$env' is not defined under Stage '$stage' in the configuration."
    }

    return $stageTable[$env]
}

    [string] GetSolutionPath() {
        return $this.JoinToWorkspacePath($this.DotnetSolution)
    }

    [string] GetProjectPath() {
        return $this.JoinToWorkspacePath($this.DotnetProject)
    }

    [string] GetPublishPath() {
        return $this.JoinToWorkspacePath($this.PublishDir)
    }

    [string] GetDockerComposePath() {
        return $this.JoinToWorkspacePath($this.DockerComposeFile)
    }

    [string] GetDockerfilePath() {
        return $this.JoinToWorkspacePath($this.Dockerfile)
    }

    hidden [string] JoinToWorkspacePath([string]$relativePath) {
        $root = $env:WORKSPACE_ROOT
        if (-not $root) {
            throw "WORKSPACE_ROOT environment variable is not set."
        }
        return Join-Path $root $relativePath
    }
}



# Abstract base class for deployment environments
class Environment {
    [string]$Name
    [string]$Version
    [string]$AdminPassword
    [CICDConfig]$Config

    Environment([string]$name, [string]$version, [string]$adminPassword, [CICDConfig]$config) {
        if ($this.GetType() -eq [Environment]) {
            throw "Cannot instantiate abstract class Environment"
        }
        $this.Name = $name
        $this.Version = $version
        $this.AdminPassword = $adminPassword
        $this.Config = $config
    }

    # Abstract methods that must be implemented by derived classes
    [void] Deploy() {
        throw "Deploy method must be implemented by derived class"
    }

    [void] Teardown() {
        throw "Teardown method must be implemented by derived class"
    }
}

class DockerEnvironment : Environment {
    DockerEnvironment([string]$name, [string]$version, [string]$adminPassword, [CICDConfig]$config) 
        : base($name, $version, $adminPassword, $config) 
        { 

        }

    hidden [void] WaitForHealthyContainers([string]$composeFile, [int]$timeoutSeconds = 300) {
        Write-Log "⏳ Waiting for containers to be healthy..."
        $startTime = Get-Date
        $timeout = $startTime.AddSeconds($timeoutSeconds)

        while ($true) {
            $containers = docker-compose -f $composeFile ps --format json | ConvertFrom-Json
            $unhealthyContainers = $containers | Where-Object { $_.Health -and $_.Health -ne "healthy" }
            
            if (-not $unhealthyContainers) {
                Write-Log "✅ All containers are healthy!"
                return
            }

            if ((Get-Date) -gt $timeout) {
                $unhealthyNames = ($unhealthyContainers | ForEach-Object { $_.Name }) -join ", "
                throw "Timeout waiting for containers to be healthy. Unhealthy containers: $unhealthyNames"
            }

            Start-Sleep -Seconds 5
        }
    }

    [void] Deploy() {
        Write-Log "🐳 Deploying Docker environment $($this.Name) with version $($this.Version)"
        $dockerComposePath = $this.Config.GetDockerComposePath()
        docker compose -f $dockerComposePath up -d
        
        # Wait for containers to be healthy
        $this.WaitForHealthyContainers($dockerComposePath, 300)
    }

    [void] Teardown() {
        Write-Log "🧹 Tearing down Docker environment $($this.Name)"
        $dockerComposePath = $this.Config.GetDockerComposePath()
        docker compose -f $dockerComposePath down
    }
}

class GCloudEnvironment : Environment {

    GCloudEnvironment([string]$name, [string]$version, [string]$adminPassword,[CICDConfig]$config) 
        : base($name, $version, $adminPassword, $config) 
        {
        }

    [void] Deploy() {
        Write-Log "🚀 Deploying GCloud environment $($this.Name) with version $($this.Version)"

        $workspaceRoot = $env:WORKSPACE_ROOT
        Write-Host "📡 Retrieving project number"
        $projectNumber = (gcloud projects describe $this.Config.ProjectId --format='value(projectNumber)')
        ### Deploy rqlite

        Write-Host "✅ Deploying rqlite (initial)"
        gcloud run deploy "rqlite-$($this.Name)" `
            --image=rqlite/rqlite `
            --port=4001 `
            --region=$($this.Config.Region) `
            --ingress=internal `
            --min-instances=1 `
            --max-instances=1 `
            --allow-unauthenticated `
            --args="--http-addr=0.0.0.0:4001"

        $rqliteUrl = "rqlite-$($this.Name)-$projectNumber.$($this.Config.Region).run.app"
        Write-Host "🔁 Updating rqlite HTTP_ADV_ADDR=$rqliteUrl"
        Write-Host "Resolved advertised address: $rqliteUrl`:443"
        gcloud run services update "rqlite-$($this.Name)" `
            --region=$($this.Config.Region) `
            --update-env-vars=HTTP_ADV_ADDR="$rqliteUrl`:443" `
            --args="--http-addr=0.0.0.0:4001"

        ### Deploy mailpit-ui

        Write-Host "✅ Deploying mailpit-ui"
        gcloud run deploy "mailpit-ui-$($this.Name)" `
            --image=axllent/mailpit:latest `
            --port=8025 `
            --region=$($this.Config.Region) `
            --allow-unauthenticated `
            --ingress=all `
            --network=default `
            --subnet=default `
            --vpc-egress=all-traffic `
            --args="--database=https://$rqliteUrl" `
            --set-env-vars="MP_UI_AUTH=admin:$($this.AdminPassword)"

        $mailpitSa = (gcloud run services describe "mailpit-ui-$($this.Name)" `
            --region=$($this.Config.Region) `
            --format='value(spec.template.spec.serviceAccountName)')

        gcloud run services add-iam-policy-binding "rqlite-$($this.Name)" `
            --region=$($this.Config.Region) `
            --member="serviceAccount:$MailpitSA" `
            --role=roles/run.invoker

        ### Ensure GKE cluster

        $clusterName = "sandbox-cluster"
        Write-Host "📦 Ensuring GKE cluster exists"
        $clusterExists = $null
        $null = & gcloud container clusters describe $clusterName --region=$($this.Config.Region) 2>$null
        if ($LASTEXITCODE -eq 0) {
            $ClusterExists = $true
            Write-Log "✅ Cluster exists"
        } else {
            $ClusterExists = $false
            Write-Log "ℹ️ Cluster does not exist"
        }

        if (-not $ClusterExists) {
            gcloud container clusters create $ClusterName `
                --region=$($this.Config.Region) `
                --num-nodes=1 `
                --disk-type=pd-standard `
                --disk-size=30GB `
                --quiet
        }

        Write-Host "🌐 Getting GKE credentials"
        gcloud container clusters get-credentials $ClusterName --region=$($this.Config.Region)

        ### Deploy mailpit-smtp to GKE

        Write-Host "📦 Rendering and deploying mailpit-smtp to GKE"

        $vars = @{
            ENV        = $this.Name
            RQLITE_URL = $rqliteUrl
        }

        ConvertTo-FileFromTemplate `
            -TemplatePath "$workspaceRoot/scripts/templates/release-stage/mailpit-smtp.yaml.tpl" `
            -OutputPath "mailpit-smtp.yaml" `
            -Variables $vars    

        ConvertTo-FileFromTemplate `
            -TemplatePath "$workspaceRoot/scripts/templates/release-stage/mailpit-smtp-service.yaml.tpl" `
            -OutputPath "mailpit-smtp-service.yaml" `
            -Variables $vars    

        kubectl apply -f mailpit-smtp.yaml
        kubectl apply -f mailpit-smtp-service.yaml

        # Cleanup
        Remove-Item -Path mailpit-smtp.yaml
        Remove-Item -Path mailpit-smtp-service.yaml

        $mailpitUrl = "mailpit-ui-$($this.Name)-${projectNumber}.$($this.Config.Region).run.app"
        $timeout = 300
        $interval = 5
        $elapsed = 0
        $smtpIp = $null

        while ($true) {
            try {
                $smtpIp = kubectl get svc "mailpit-smtp-$($this.Name)" -o jsonpath='{.status.loadBalancer.ingress[0].ip}' 2>$null
                if ($smtpIp) { break }
            } catch {
                # Continue if the command fails
            }

            Write-Host "Waiting for LoadBalancer IP for mailpit-smtp-$($this.Name)... ($elapsed / $timeout seconds)"
            Start-Sleep -Seconds $interval
            $elapsed += $interval
            if ($elapsed -ge $timeout) {
                Write-Host "❌ Timed out waiting for LoadBalancer IP for mailpit-smtp-$($this.Name)"
                throw "❌ Timed out waiting for LoadBalancer IP for mailpit-smtp-$($this.Name)"
            }
        }

        ### Deploy MVC app

        Write-Host "✅ Deploying .NET 9 MVC App"
        gcloud run deploy "mvc-app-$($this.Name)" `
            --image="us-central1-docker.pkg.dev/$($this.Config.ProjectId)/ctf-sandbox-repo/ctf-sandbox:$($this.Version)" `
            --region=$($this.Config.Region) `
            --ingress=all `
            --allow-unauthenticated `
            --network=default `
            --subnet=default `
            --vpc-egress=all-traffic `
            --set-env-vars="EmailSettings__SmtpServer=$smtpIp,AdminAccount__Password=$($this.AdminPassword),EmailSettings__MailpitUrl=https://$mailpitUrl"

        Write-Host "✅ Deployment complete"
    }

    [void] Teardown() {
        Write-Log "🧹 Tearing down GCloud environment $($this.Name)"
        try {
            $clusterName = "sandbox-cluster"

            Write-Log "🧹 Deleting Cloud Run service: mvc-app-$($this.Name)"
            & gcloud run services delete "mvc-app-$($this.Name)" --region=$($this.Config.Region) --quiet
            if ($LASTEXITCODE -ne 0) {
                Write-Log "⚠️ MVC app deletion failed or didn't exist"
            }

            Write-Log "🧹 Deleting Cloud Run service: mailpit-ui-$($this.Name)"
            & gcloud run services delete "mailpit-ui-$($this.Name)" --region=$($this.Config.Region) --quiet
            if ($LASTEXITCODE -ne 0) {
                Write-Log "⚠️ Mailpit UI deletion failed or didn't exist"
            }

            Write-Log "🧹 Deleting Cloud Run service: rqlite-$($this.Name)"
            & gcloud run services delete "rqlite-$($this.Name)" --region=$($this.Config.Region) --quiet
            if ($LASTEXITCODE -ne 0) {
                Write-Log "⚠️ rqlite deletion failed or didn't exist"
            }

            Write-Log "🌐 Getting GKE credentials"
            & gcloud container clusters get-credentials $clusterName --region=$($this.Config.Region)
            if ($LASTEXITCODE -ne 0) {
                Write-Log "⚠️ Failed to get credentials for GKE cluster '$clusterName'"
                return
            }

            Write-Log "🧹 Deleting mailpit-smtp service from GKE"
            & kubectl delete service "mailpit-smtp-$($this.Name)"
            if ($LASTEXITCODE -ne 0) {
                Write-Log "⚠️ Mailpit SMTP service deletion failed or didn't exist"
            }

            Write-Log "🧹 Deleting mailpit-smtp deployment from GKE"
            & kubectl delete deployment "mailpit-smtp-$($this.Name)"
            if ($LASTEXITCODE -ne 0) {
                Write-Log "⚠️ Mailpit SMTP deployment deletion failed or didn't exist"
            }

            Write-Log "✅ Teardown complete for environment: $($this.Name)"
        }
        catch {
            Write-Log "⚠️ Failed to teardown GCloud $($this.Name) env: $_"
        }
    }
}


function Write-Log {
    param([string]$Message)
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "[$timestamp] $Message"
}

function Build-DotNetSolution {
    $config = Get-CICDConfig
    Write-Log "Building .NET solution at $($config.GetSolutionPath())"
    dotnet build $config.GetSolutionPath() -c Release
}

function Invoke-Tests {
    param(
        [string]$Stage,
        [string]$Env = "Default"
    )
    $config = Get-CICDConfig
    $Categories = $config.GetTestCategories($Stage, $Env)

    foreach ($category in $Categories) {
        Write-Log "Running tests for category: $category"
        dotnet test $config.GetSolutionPath() --no-build -c Release --filter "Category=$category" --logger "trx;LogFilePath=$category.trx"
    }
}

function Publish-DotNetApp {

    $config = Get-CICDConfig
    $ProjectPath = $config.GetProjectPath()
    $OutputPath = $config.GetPublishPath()
    Write-Log "Publishing .NET project $ProjectPath to $OutputPath"
    dotnet publish $ProjectPath -c Release -o $OutputPath
}

function Build-DockerImage {
    param(
        [string]$Version
    )

    $config = Get-CICDConfig
    $versionedTag = "$($config.DockerImageName):$Version"
    $dockerfilePath = $config.GetDockerfilePath()
    Write-Log "Building Docker image: $versionedTag from file $dockerfilePath"
    docker build -t $versionedTag -f $dockerFilePath .
}

function Push-DockerImage {
    param(
        [string]$Version
    )
    $config = Get-CICDConfig
    $versionedTag = "$($config.DockerImageName):$Version"

    Write-Log "Pushing Docker image: $versionedTag"
    docker push $versionedTag
}

function Get-CICDConfig {
    $configPath = Join-Path $env:WORKSPACE_ROOT "scripts/shared/config.psd1"
    $rawConfig = Import-PowerShellDataFile $configPath
    return [CICDConfig]::new($rawConfig)
}

function Test-IsShaAlreadyProcessed {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Version
    )

    $objectPath = Get-TestedShaGcsPath -Version $Version
    Write-Log "🔍 Checking if SHA already tested: $Version"
    $result = & gsutil ls $objectPath 2>$null

    return ($LASTEXITCODE -eq 0)
}


function Register-TestedSha {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Version
    )

    $bucketPath = Get-TestedShaGcsPath -Version $Version
    Write-Log "📦 Registering SHA $Version as tested"

    $tmpFile = New-TemporaryFile
    "Tested on $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" | Set-Content $tmpFile
    gsutil cp $tmpFile $bucketPath | Out-Null
    Remove-Item $tmpFile -Force
}


function Get-TestedShaGcsPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Version
    )

    $config = Get-CICDConfig
    return "gs://$($config.BucketName)/tested-shas/$Version"
}


function New-RandomPassword {
    param (
        [int]$Length = 16
    )

    $chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[]{}'
    $password = -join (1..$Length | ForEach-Object { $chars[(Get-Random -Max $chars.Length)] })
    return $password
}

function New-GCloudEnvironment {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Name,
        
        [Parameter(Mandatory = $true)]
        [string]$Version,
        
        [Parameter(Mandatory = $true)]
        [string]$AdminPassword
    )
    $config = Get-CICDConfig
    return [GCloudEnvironment]::new($Name, $Version, $AdminPassword, $config)
}

function New-DockerEnvironment {
    param (
        [Parameter(Mandatory = $true)]
        [string]$Name,
        
        [Parameter(Mandatory = $true)]
        [string]$Version,
        
        [Parameter(Mandatory = $true)]
        [string]$AdminPassword
    )
    $config = Get-CICDConfig
    return [DockerEnvironment]::new($Name, $Version, $AdminPassword, $config)
}

function ConvertTo-FileFromTemplate {
    param(
        [Parameter(Mandatory)]
        [string]$TemplatePath,

        [Parameter(Mandatory)]
        [string]$OutputPath,

        [Parameter(Mandatory)]
        [hashtable]$Variables
    )

    if (-not (Test-Path $TemplatePath)) {
        throw "Template file not found: $TemplatePath"
    }

    $content = Get-Content $TemplatePath -Raw

    foreach ($key in $Variables.Keys) {
        $pattern = "{{${key}}}"
        $replacement = [regex]::Escape($Variables[$key])
        $content = $content -replace [regex]::Escape($pattern), $Variables[$key]
    }

    Set-Content -Path $OutputPath -Value $content
}


# Export functions and classes
Export-ModuleMember -Function *

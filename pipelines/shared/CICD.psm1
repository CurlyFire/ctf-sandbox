class WorkspaceBoundConfig {
    hidden [string] Join([string]$relativePath) {
        $root = $env:WORKSPACE_ROOT
        if (-not $root) {
            throw "WORKSPACE_ROOT environment variable is not set."
        }
        return Join-Path $root $relativePath
    }
}

class GoogleCloudConfig {
    [string]$ProjectId
    [string]$Region
    [string]$Zone
    [string]$Bucket
    [string]$ProjectNumber

    [string] GetTestedShaGcsPath([string]$version) {
        if (-not $version) {
            throw "Version must be specified to get the GCS path."
        }
        
        if (-not $this.Bucket) {
            throw "Google Cloud Bucket is not configured."
        }

    return "gs://$($this.Bucket)/tested-shas/$version"
    }
}

class AppConfig : WorkspaceBoundConfig {
    [string]$DotnetSolution
    [string]$DotnetProject
    [string]$Dockerfile
    [string]$DevAppSettingsFile
    [string]$DatabaseFile
    [string]$PublishDir
    [string]$DockerImageName

    [string] GetSolutionPath() {
        return $this.Join($this.DotnetSolution)
    }

    [string] GetProjectPath() {
        return $this.Join($this.DotnetProject)
    }

    [string] GetPublishPath() {
        return $this.Join($this.PublishDir)
    }

    [string] GetDockerfilePath() {
        return $this.Join($this.Dockerfile)
    }

    [string] GetDevAppSettingsPath() {
        return $this.Join($this.DevAppSettingsFile)
    }

    [string] GetDatabaseFilePath() {
        return $this.Join($this.DatabaseFile)
    }
}

class DockerComposeConfig : WorkspaceBoundConfig {
    [string]$File
}

class IpInfoConfig {
    [string]$Url
}

class CICDConfig {
    [GoogleCloudConfig]$GoogleCloud
    [AppConfig]$App
    [DockerComposeConfig]$DockerCompose
    [IpInfoConfig]$IpInfo
    [hashtable]$TestCategories

    CICDConfig([hashtable]$rawConfig) {
        $gcloud = [GoogleCloudConfig]::new()
        $gcloud.ProjectId = $rawConfig.GoogleCloud.ProjectId
        $gcloud.Region    = $rawConfig.GoogleCloud.Region
        $gcloud.Zone      = $rawConfig.GoogleCloud.Zone
        $gcloud.Bucket    = $rawConfig.GoogleCloud.Bucket
        $gcloud.ProjectNumber = $rawConfig.GoogleCloud.ProjectNumber
        $this.GoogleCloud = $gcloud

        $appCfg = [AppConfig]::new()
        $appCfg.DotnetSolution     = $rawConfig.App.DotnetSolution
        $appCfg.DotnetProject      = $rawConfig.App.DotnetProject
        $appCfg.Dockerfile         = $rawConfig.App.Dockerfile
        $appCfg.DevAppSettingsFile = $rawConfig.App.DevAppSettingsFile
        $appCfg.DatabaseFile       = $rawConfig.App.DatabaseFile
        $appCfg.PublishDir         = $rawConfig.App.PublishDir
        $appCfg.DockerImageName    = $rawConfig.App.DockerImageName
        $this.App = $appCfg

        $compose = [DockerComposeConfig]::new()
        $compose.File         = $rawConfig.DockerCompose.File

        $this.DockerCompose = $compose

        $ipCfg = [IpInfoConfig]::new()
        $ipCfg.Url = $rawConfig.IpInfo.Url
        $this.IpInfo = $ipCfg

        $this.TestCategories = $rawConfig.TestCategories
    }

    [string[]] GetTestCategories([string]$stage, [string]$env = "Default") {
        if (-not $this.TestCategories.ContainsKey($stage)) {
            throw "❌ Stage '$stage' is not defined in the configuration."
        }

        $stageTable = $this.TestCategories[$stage]
        if (-not $stageTable.ContainsKey($env)) {
            $env = "Default"
        }

        return $stageTable[$env]
    }
}

class GCloudEnvironment {
    [string]$WebServerUrl
    [string]$MailpitUrl
    [string]$IpInfoUrl
    [string]$IpInfoToken
    [string]$WebAdminAccount
    [string]$MailpitAdminAccount
    [string]$AdminPassword
}

function Write-Log {
    param([string]$Message)
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "[$timestamp] $Message"
}

function Build-DotNetSolution {
    [CICDConfig]$config = Get-CICDConfig
    $solutionPath = $config.App.GetSolutionPath()
    Write-Log "Building .NET solution at $solutionPath"
    Invoke-NativeCommandWithoutReturn dotnet build $solutionPath -c Debug
}

function Invoke-Tests {
    param(
        [string]$Stage,
        [string]$EnvironmentName = "Default",
        [GCloudEnvironment]$GCloudEnvironment = $null
    )
    [CICDConfig]$config = Get-CICDConfig
    $Categories = $config.GetTestCategories($Stage, $EnvironmentName)
    try 
    {
        if ($null -ne $GCloudEnvironment) {
            Write-Log "Overriding test app settings with provided configuration using environment variables"
            $env:WebServer__Url = $GCloudEnvironment.WebServerUrl
            $env:WebServer__AdminAccount = $GCloudEnvironment.WebAdminAccount
            $env:WebServer__AdminPassword = $GCloudEnvironment.AdminPassword
            $env:Mailpit__Url = $GCloudEnvironment.MailpitUrl
            $env:Mailpit__AdminAccount = $GCloudEnvironment.MailpitAdminAccount
            $env:Mailpit__AdminPassword = $GCloudEnvironment.AdminPassword
            $env:IpInfo__Url = $GCloudEnvironment.IpInfoUrl
            $env:IpInfo__Token = $GCloudEnvironment.IpInfoToken
        } else {
            Write-Log "No deployment configuration provided, using app settings from config"
        }
        foreach ($category in $Categories) {
            Write-Log "Running tests for category: $category"
            Invoke-NativeCommandWithoutReturn dotnet test $config.App.GetSolutionPath() -c Debug --filter "Category=$category" --logger "trx;LogFilePath=$category.trx"
        }
    }
    finally {
        # Clean up environment variables
        Write-Log "Cleaning up environment variables"
        Remove-Item env:WebServer__Url -ErrorAction SilentlyContinue
        Remove-Item env:WebServer__AdminAccount -ErrorAction SilentlyContinue
        Remove-Item env:WebServer__AdminPassword -ErrorAction SilentlyContinue
        Remove-Item env:Mailpit__Url -ErrorAction SilentlyContinue
        Remove-Item env:Mailpit__AdminAccount -ErrorAction SilentlyContinue
        Remove-Item env:Mailpit__AdminPassword -ErrorAction SilentlyContinue
        Remove-Item env:IpInfo__Url -ErrorAction SilentlyContinue
        Remove-Item env:IpInfo__Token -ErrorAction SilentlyContinue
        Write-Log "Cleaned up environment variables"
    }
}

function Publish-DotNetApp {

    [CICDConfig]$config = Get-CICDConfig
    $ProjectPath = $config.App.GetProjectPath()
    $OutputPath = $config.App.GetPublishPath()
    Write-Log "Publishing .NET project $ProjectPath to $OutputPath"
    Invoke-NativeCommandWithoutReturn dotnet publish $ProjectPath -c Debug -o $OutputPath
}

function Build-DockerImage {
    param(
        [string]$Version
    )
    [CICDConfig]$config = Get-CICDConfig
    $versionedTag = "$($config.App.DockerImageName):$Version"
    $dockerfilePath = $config.App.GetDockerfilePath()
    Write-Log "Building Docker image: $versionedTag from file $dockerfilePath"
    Invoke-NativeCommandWithoutReturn docker build -t $versionedTag -f $dockerFilePath .
}

function Push-DockerImage {
    param(
        [string]$Version
    )
    [CICDConfig]$config = Get-CICDConfig
    $versionedTag = "$($config.App.DockerImageName):$Version"

    Write-Log "Pushing Docker image: $versionedTag"
    Invoke-NativeCommandWithoutReturn docker push $versionedTag
}

function Get-CICDConfig {
    $configPath = Join-Path $env:WORKSPACE_ROOT "pipelines/shared/config.psd1"
    $rawConfig = Import-PowerShellDataFile $configPath
    return [CICDConfig]::new($rawConfig)
}

function Test-IsShaAlreadyProcessed {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Version
    )
    [CICDConfig]$config = Get-CICDConfig

    $objectPath = $config.GoogleCloud.GetTestedShaGcsPath($Version)
    Write-Log "🔍 Checking if SHA already tested: $Version"
    $result = & gsutil ls $objectPath 2>$null

    return ($LASTEXITCODE -eq 0)
}


function Register-TestedSha {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Version
    )
    [CICDConfig]$config = Get-CICDConfig
    $bucketPath = $config.GoogleCloud.GetTestedShaGcsPath($Version)
    Write-Log "📦 Registering SHA $Version as tested"

    $tmpFile = New-TemporaryFile
    "Tested on $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" | Set-Content $tmpFile
    Invoke-NativeCommandWithoutReturn gsutil cp $tmpFile $bucketPath | Out-Null
    Remove-Item $tmpFile -Force
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

function Invoke-NativeCommandWithoutReturn() {
    # A handy way to run a command, and automatically throw an error if the
    # exit code is non-zero.

    if ($args.Count -eq 0) {
        throw "Must supply some arguments."
    }

    $command = $args[0]
    $commandArgs = @()
    if ($args.Count -gt 1) {
        # Always force array, even if only one element
        $commandArgs = @($args[1..($args.Count - 1)])
    }

    &{ 
        & $command $commandArgs
    } | Out-Host
    $result = $LASTEXITCODE

    if ($result -ne 0) {
        throw "$command $commandArgs exited with code $result."
    }
}

function Remove-AllGCloudRessources()
{
    [CICDConfig]$config = Get-CICDConfig
    $project = (gcloud config get-value project)
    $region = $config.GoogleCloud.Region
    $zone = $config.GoogleCloud.Zone
    Write-Host "⚙️ Using project: $project"
    Write-Host "📍 Region: $region"

    # 1. Delete Cloud Run services in us-central1
    Write-Host "`n🚮 Deleting Cloud Run services in $region..."
    $services = gcloud run services list --platform=managed --region=$region --format="value(metadata.name)"
    foreach ($svc in $services) {
        Write-Host "  🧹 Deleting Cloud Run service '$svc'"
        gcloud run services delete $svc --platform=managed --region=$region --quiet
    }

    # 2. Delete GKE clusters
    Write-Host "`n🧨 Deleting GKE clusters..."
    $clusters = gcloud container clusters list --zone $zone --format="value(name)"
    foreach ($cluster in $clusters) {
        Write-Host "  🧹 Deleting GKE cluster '$cluster' in zone '$zone'"
        gcloud container clusters delete --zone $zone $cluster --quiet
    }

    # 3. Delete VPC Access Connectors in us-central1
    Write-Host "`n🔌 Deleting VPC Access Connectors in $region..."
    $connectors = gcloud compute networks vpc-access connectors list --region=$region --format="value(name)"
    foreach ($connector in $connectors) {
        Write-Host "  🧹 Deleting VPC connector '$connector'"
        gcloud compute networks vpc-access connectors delete $connector --region=$region --quiet
    }

    # 4. Delete NAT configs and routers in us-central1
    Write-Host "`n🌐 Deleting Cloud NAT configs and routers in $region..."
    $routers = gcloud compute routers list --format="table(name,region)" | Select-Object -Skip 1
    foreach ($line in $routers) {
        $fields = $line -split "\s+"
        $routerName = $fields[0]
        $routerRegion = $fields[1]

        if ($routerRegion -eq $region) {
            $nats = gcloud compute routers nats list --router=$routerName --region=$region --format="value(name)"
            foreach ($nat in $nats) {
                Write-Host "  🧹 Deleting NAT config '$nat' from router '$routerName'"
                gcloud compute routers nats delete $nat --router=$routerName --region=$region --quiet
            }

            Write-Host "  🧹 Deleting router '$routerName'"
            gcloud compute routers delete $routerName --region=$region --quiet
        }
    }


    Write-Host "`n✅ Done. All resources in $region have been deleted to minimize cost."
}

function Deploy-GCloudEnvironment {
    [OutputType([GCloudEnvironment])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name,

        [Parameter(Mandatory = $true)]
        [string]$Version,

        [Parameter(Mandatory = $true)]
        [string]$WebAdminAccount,

        [Parameter(Mandatory = $true)]
        [string]$MailpitAdminAccount,        

        [Parameter(Mandatory = $true)]
        [string]$AdminPassword,

        [Parameter(Mandatory = $false)]
        [string]$IpInfoToken
    )

    Write-Log "🚀 Deploying GCloud environment $Name with version $Version"

    [CICDConfig]$config = Get-CICDConfig

    $workspaceRoot = $env:WORKSPACE_ROOT
    Write-Host "📡 Retrieving project number"
    $projectNumber = $config.GoogleCloud.ProjectNumber
    ### Deploy rqlite

    Write-Host "✅ Deploying rqlite (initial)"
    Invoke-NativeCommandWithoutReturn gcloud run deploy "rqlite-$Name" `
        --image=rqlite/rqlite `
        --port=4001 `
        --region=$($config.GoogleCloud.Region) `
        --ingress=internal `
        --min-instances=1 `
        --max-instances=1 `
        --allow-unauthenticated `
        --args="--http-addr=0.0.0.0:4001"

    $rqliteUrl = "rqlite-$Name-$projectNumber.$($config.GoogleCloud.Region).run.app"
    Write-Host "🔁 Updating rqlite HTTP_ADV_ADDR=$rqliteUrl"
    Write-Host "Resolved advertised address: $rqliteUrl`:443"
    Invoke-NativeCommandWithoutReturn gcloud run services update "rqlite-$Name" `
        --region=$($config.GoogleCloud.Region) `
        --update-env-vars=HTTP_ADV_ADDR="$rqliteUrl`:443"

    ### Deploy mailpit-ui

    Write-Host "✅ Deploying mailpit-ui"
    Invoke-NativeCommandWithoutReturn gcloud run deploy "mailpit-ui-$Name" `
        --image=axllent/mailpit:latest `
        --port=8025 `
        --region=$($config.GoogleCloud.Region) `
        --allow-unauthenticated `
        --ingress=all `
        --network=default `
        --subnet=default `
        --vpc-egress=all-traffic `
        --args="--database=https://$rqliteUrl" `
        --set-env-vars="MP_UI_AUTH=$($MailpitAdminAccount):$($AdminPassword)" `
        --startup-probe=httpGet.path=/readyz

    $mailpitSa = (gcloud run services describe "mailpit-ui-$Name" `
        --region=$($config.GoogleCloud.Region) `
        --format='value(spec.template.spec.serviceAccountName)')

    Invoke-NativeCommandWithoutReturn gcloud run services add-iam-policy-binding "rqlite-$Name" `
        --region=$($config.GoogleCloud.Region) `
        --member="serviceAccount:$MailpitSA" `
        --role=roles/run.invoker

    ### Ensure GKE cluster

    $clusterName = "sandbox-cluster"
    Write-Host "📦 Ensuring GKE cluster exists"
    $clusterExists = $null
    $null = gcloud container clusters describe $clusterName --zone=$($config.GoogleCloud.Zone) 2>$null
    if ($LASTEXITCODE -eq 0) {
        $ClusterExists = $true
        Write-Log "✅ Cluster exists"
    } else {
        $ClusterExists = $false
        Write-Log "ℹ️ Cluster does not exist"
    }

    if (-not $ClusterExists) {
        Invoke-NativeCommandWithoutReturn gcloud container clusters create $ClusterName `
            --zone=$($config.GoogleCloud.Zone) `
            --num-nodes=1 `
            --disk-type=pd-standard `
            --disk-size=30GB `
            --quiet
    }

    Write-Host "🌐 Getting GKE credentials"
    Invoke-NativeCommandWithoutReturn gcloud container clusters get-credentials $ClusterName --zone=$($config.GoogleCloud.Zone)

    ### Deploy mailpit-smtp to GKE

    Write-Host "📦 Rendering and deploying mailpit-smtp to GKE"

    $vars = @{
        ENV        = $Name
        RQLITE_URL = $rqliteUrl
    }

    ConvertTo-FileFromTemplate `
        -TemplatePath "$workspaceRoot/pipelines/stages/release/templates/mailpit-smtp.yaml.tpl" `
        -OutputPath "mailpit-smtp.yaml" `
        -Variables $vars    

    ConvertTo-FileFromTemplate `
        -TemplatePath "$workspaceRoot/pipelines/stages/release/templates/mailpit-smtp-service.yaml.tpl" `
        -OutputPath "mailpit-smtp-service.yaml" `
        -Variables $vars    

    Invoke-NativeCommandWithoutReturn kubectl apply -f mailpit-smtp.yaml
    Invoke-NativeCommandWithoutReturn kubectl apply -f mailpit-smtp-service.yaml

    # Cleanup
    Remove-Item -Path mailpit-smtp.yaml
    Remove-Item -Path mailpit-smtp-service.yaml

    $mailpitUrl = "mailpit-ui-$Name-${projectNumber}.$($config.GoogleCloud.Region).run.app"
    $timeout = 300
    $interval = 5
    $elapsed = 0
    $smtpIp = $null

    while ($true) {
        try {
            $smtpIp = kubectl get svc "mailpit-smtp-$Name" -o jsonpath='{.status.loadBalancer.ingress[0].ip}' 2>$null
            if ($smtpIp) { break }
        } catch {
            # Continue if the command fails
        }

        Write-Host "Waiting for LoadBalancer IP for mailpit-smtp-$Name... ($elapsed / $timeout seconds)"
        Start-Sleep -Seconds $interval
        $elapsed += $interval
        if ($elapsed -ge $timeout) {
            Write-Host "❌ Timed out waiting for LoadBalancer IP for mailpit-smtp-$Name"
            throw "❌ Timed out waiting for LoadBalancer IP for mailpit-smtp-$Name"
        }
    }

    ### Deploy MVC app

    Write-Host "✅ Deploying .NET 9 MVC App"

    # Check if the VPC connector already exists
    if (-not (& gcloud compute networks vpc-access connectors describe run-connector --region=$($config.GoogleCloud.Region) 2>$null)) {
        Write-Host "🛠 Creating VPC connector..."
        Invoke-NativeCommandWithoutReturn gcloud compute networks vpc-access connectors create run-connector `
            --region=$($config.GoogleCloud.Region) `
            --network=default `
            --range=10.8.0.0/28 `
            --max-instances=3 `
            --min-instances=2
    }
    else {
        Write-Host "✅ VPC connector 'run-connector' already exists."
    }

    # Check if the Cloud Router exists
    if (-not (& gcloud compute routers describe nat-router --region=$($config.GoogleCloud.Region) 2>$null)) {
        Write-Host "🛠 Creating Cloud Router..."
        Invoke-NativeCommandWithoutReturn gcloud compute routers create nat-router `
            --region=$($config.GoogleCloud.Region) `
            --network=default
    }
    else {
        Write-Host "✅ Cloud Router 'nat-router' already exists."
    }

    # Check if the Cloud NAT config exists
    if (-not (& gcloud compute routers nats describe nat-config --router=nat-router --region=$($config.GoogleCloud.Region) 2>$null)) {
        Write-Host "🛠 Creating Cloud NAT configuration..."
        Invoke-NativeCommandWithoutReturn gcloud compute routers nats create nat-config `
            --router=nat-router `
            --region=$($config.GoogleCloud.Region) `
            --nat-all-subnet-ip-ranges `
            --auto-allocate-nat-external-ips
    }
    else {
        Write-Host "✅ Cloud NAT 'nat-config' already exists."
    }

    Invoke-NativeCommandWithoutReturn gcloud run deploy "mvc-app-$Name" `
        --image="us-central1-docker.pkg.dev/$($config.GoogleCloud.ProjectId)/ctf-sandbox-repo/ctf-sandbox:$Version" `
        --region=$($config.GoogleCloud.Region) `
        --ingress=all `
        --allow-unauthenticated `
        --vpc-connector=run-connector `
        --vpc-egress=all-traffic `
        --set-env-vars="EmailSettings__SmtpServer=$smtpIp,AdminAccount__Email=$WebAdminAccount,AdminAccount__Password=$AdminPassword,EmailSettings__MailpitUrl=https://$mailpitUrl,IPInfo__Token=$IpInfoToken" `
        --startup-probe=httpGet.path=/health 

    Write-Host "✅ Deployment complete"

    $mailpitUrl = "https://mailpit-ui-$Name-$projectNumber.$($config.GoogleCloud.Region).run.app"
    $webAppUrl = "https://mvc-app-$Name-$projectNumber.$($config.GoogleCloud.Region).run.app"

    $envConfig = [GCloudEnvironment]::new()
    $envConfig.WebServerUrl = $webAppUrl
    $envConfig.MailpitUrl = $mailpitUrl
    $envConfig.IpInfoUrl = $config.IpInfo.Url
    $envConfig.WebAdminAccount = $WebAdminAccount
    $envConfig.MailpitAdminAccount = $MailpitAdminAccount
    $envConfig.AdminPassword = $AdminPassword
    $envConfig.IpInfoToken = $IpInfoToken
    return $envConfig
}

function Remove-GCloudEnvironment {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Name
    )

    Write-Log "🧹 Tearing down GCloud environment $Name"

    [CICDConfig]$config = Get-CICDConfig
    try {
        $clusterName = "sandbox-cluster"

        Write-Log "🧹 Deleting Cloud Run service: mvc-app-$Name"
        & gcloud run services delete "mvc-app-$Name" --region=$($config.GoogleCloud.Region) --quiet
        if ($LASTEXITCODE -ne 0) {
            Write-Log "⚠️ MVC app deletion failed or didn't exist"
        }

        Write-Log "🧹 Deleting Cloud Run service: mailpit-ui-$Name"
        & gcloud run services delete "mailpit-ui-$Name" --region=$($config.GoogleCloud.Region) --quiet
        if ($LASTEXITCODE -ne 0) {
            Write-Log "⚠️ Mailpit UI deletion failed or didn't exist"
        }

        Write-Log "🧹 Deleting Cloud Run service: rqlite-$Name"
        & gcloud run services delete "rqlite-$Name" --region=$($config.GoogleCloud.Region) --quiet
        if ($LASTEXITCODE -ne 0) {
            Write-Log "⚠️ rqlite deletion failed or didn't exist"
        }

        Write-Log "🌐 Getting GKE credentials"
        & gcloud container clusters get-credentials $clusterName --zone=$($config.GoogleCloud.Zone)
        if ($LASTEXITCODE -ne 0) {
            Write-Log "⚠️ Failed to get credentials for GKE cluster '$clusterName'"
            return
        }

        Write-Log "🧹 Deleting mailpit-smtp service from GKE"
        & kubectl delete service "mailpit-smtp-$Name"
        if ($LASTEXITCODE -ne 0) {
            Write-Log "⚠️ Mailpit SMTP service deletion failed or didn't exist"
        }

        Write-Log "🧹 Deleting mailpit-smtp deployment from GKE"
        & kubectl delete deployment "mailpit-smtp-$Name"
        if ($LASTEXITCODE -ne 0) {
            Write-Log "⚠️ Mailpit SMTP deployment deletion failed or didn't exist"
        }

        Write-Log "✅ Teardown complete for environment: $Name"
    }
    catch {
        Write-Log "⚠️ Failed to teardown GCloud $Name env: $_"
    }
}

# Export functions and classes
Export-ModuleMember -Function *
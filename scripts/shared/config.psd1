@{
    ProjectId                 = "fifth-compiler-458221-c9"
    Region                    = "us-central1"
    BucketName                = "ctf-sandbox"
    DockerImageName           = "us-central1-docker.pkg.dev/fifth-compiler-458221-c9/ctf-sandbox-repo/ctf-sandbox"
    DotnetSolution            = "src/ctf-sandbox.sln"
    DotnetProject             = "src/ctf-sandbox.csproj"
    DockerComposeFile         = "docker-compose.yml"
    Dockerfile                = "Dockerfile"
    PublishDir                = ".artifacts/publish"
    DevAppSettingsFile        = "src/appsettings.web.dev.json" 

    TestCategories = @{
        commit = @{
            Default = @("Unit", "NarrowIntegration", "Component", "ContractVerification")
        }
        acceptance = @{
            docker     = @("Smoke", "Acceptance")
            acceptance = @("Smoke", "Acceptance")
            e2e        = @("Smoke", "Acceptance", "ExternalSystemContract", "E2E")
        }
        release = @{
            uat        = @("Smoke")
            production = @("Smoke")
        }
    }
}
@{
    ProjectId                 = "fifth-compiler-458221-c9"
    Region                    = "us-central1"
    BucketName                = "ctf-sandbox"
    DockerImageName           = "us-central1-docker.pkg.dev/fifth-compiler-458221-c9/ctf-sandbox-repo/ctf-sandbox"
    DotnetSolution            = "src/ctf-sandbox/ctf-sandbox.sln"
    DotnetProject             = "src/ctf-sandbox/ctf-sandbox.csproj"
    DockerComposeFile         = ".devcontainer/docker-compose.yml"
    DockerComposeOverrideFile = "pipelines/shared/docker-compose.override.yml"
    DockerMailpit = @{
        Url = "http://mailpit:8025"
        SmtpPort = 1025
        Host = "mailpit"
    }
    DockerDevContainerWebUrl  = "http://localhost:8080"
    Dockerfile                = "src/ctf-sandbox/Dockerfile"
    PublishDir                = ".artifacts/publish"
    DevAppSettingsFile        = "src/ctf-sandbox/appsettings.web.dev.json"
    DatabaseFile              = "src/ctf-sandbox/app.db"

    TestCategories = @{
        commit = @{
            Default = @("Unit", "NarrowIntegration", "Component", "ContractVerification")
        }
        acceptance = @{
            docker     = @("Smoke_ExternalSystemsHealth", "Smoke_FrontEndHealth", "Smoke_UI", "Acceptance")
            acceptance = @("Smoke_ExternalSystemsHealth", "Smoke_FrontEndHealth", "Smoke_UI", "Acceptance")
            e2e        = @("Smoke_ExternalSystemsHealth", "Smoke_FrontEndHealth", "Smoke_UI", "Acceptance", "ExternalSystemContract", "E2E")
        }
        release = @{
            uat        = @("Smoke_ExternalSystemsHealth", "Smoke_FrontEndHealth", "Smoke_UI")
            production = @("Smoke_ExternalSystemsHealth", "Smoke_FrontEndHealth", "Smoke_UI")
        }
    }
}
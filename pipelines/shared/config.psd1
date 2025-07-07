@{
    GoogleCloud = @{
        ProjectId = "fifth-compiler-458221-c9"
        Region    = "us-central1"
        Zone      = "us-central1-a"
        Bucket    = "ctf-sandbox"
    }
    App = @{
        DotnetSolution            = "src/ctf-sandbox/ctf-sandbox.sln"
        DotnetProject             = "src/ctf-sandbox/ctf-sandbox.csproj"
        Dockerfile                = "src/ctf-sandbox/Dockerfile"
        DevAppSettingsFile        = "src/ctf-sandbox/appsettings.web.dev.json"
        DatabaseFile              = "src/ctf-sandbox/app.db"
        PublishDir                = ".artifacts/publish"
        DockerImageName           = "us-central1-docker.pkg.dev/fifth-compiler-458221-c9/ctf-sandbox-repo/ctf-sandbox"
    }
    DockerCompose = @{
        File                      = "docker-compose.yml"
        OverrideFile              = "docker-compose.override.yml"
        MailPit = @{
            HttpPort  = 8025
            SmtpPort = 1025
        }
        App = @{
            HttpPort = 8080
        }
    }
    IpInfo = @{
        Url   = "https://ipinfo.io"
    }
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
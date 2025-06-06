@{
    ProjectId         = "fifth-compiler-458221-c9"
    Region            = "us-central1"
    BucketName        = "ctf-sandbox"
    DockerImageName   = "us-central1-docker.pkg.dev/fifth-compiler-458221-c9/ctf-sandbox-repo/ctf-sandbox"
    DotnetSolution    = "./src/ctf-sandbox.sln"
    DotnetProject     = "./src/ctf-sandbox.csproj"
    PublishDir        = ".artifacts/publish"

    TestCategoriesPerStage   = @{
        Commit           = @("Unit", "NarrowIntegration", "Component", "ContractVerification")
        AcceptanceDocker = @("Smoke", "Acceptance")
        AcceptanceGCloud = @("Smoke", "Acceptance")
        E2E              = @("Smoke", "Acceptance", "ExternalSystemContract", "E2E")
    }
}
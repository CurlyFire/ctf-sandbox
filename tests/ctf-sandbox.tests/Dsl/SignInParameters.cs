namespace ctf_sandbox.tests.Dsl;

public record SignInParameters
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

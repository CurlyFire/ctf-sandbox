namespace ctf_sandbox.tests.Fixture;

public class Credentials
{
    public string Username { get; private set; }
    public string Password { get; private set; }
    public Credentials(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public bool IsEmpty() => string.IsNullOrWhiteSpace(Username) && string.IsNullOrWhiteSpace(Password);
}
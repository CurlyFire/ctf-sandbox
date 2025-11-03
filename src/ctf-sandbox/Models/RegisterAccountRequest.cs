using System;

namespace ctf_sandbox.Models;

public class RegisterAccountRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

}

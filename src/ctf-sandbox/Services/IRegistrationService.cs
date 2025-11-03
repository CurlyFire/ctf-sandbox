using Microsoft.AspNetCore.Identity;

namespace ctf_sandbox.Services;

public interface IRegistrationService
{
    Task<IdentityResult> RegisterAccountAsync(string email, string password, Func<string, string, string> urlGenerator);

    Task<IdentityResult> ConfirmAccountAsync(string userId, string code);
}
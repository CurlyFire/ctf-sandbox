using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;

namespace ctf_sandbox.Services;

public class RegistrationService : IRegistrationService
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly ILogger<RegistrationService> _logger;
    private readonly IEmailSender _emailSender;

    public RegistrationService(
        UserManager<IdentityUser> userManager,
        IUserStore<IdentityUser> userStore,
        SignInManager<IdentityUser> signInManager,
        ILogger<RegistrationService> logger,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _userStore = userStore;
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
    }

    public async Task<IdentityResult> RegisterAccountAsync(string email, string password, Func<string, string, string> urlGenerator)
    {
        var user = new IdentityUser();
        await _userStore.SetUserNameAsync(user, email, CancellationToken.None);
        await GetEmailStore().SetEmailAsync(user, email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with password.");

            // Add user to the default "User" role
            await _userManager.AddToRoleAsync(user, "User");

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = urlGenerator(userId, code);

            await _emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        return result;
    }

    public async Task<IdentityResult> ConfirmAccountAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var result = new IdentityResult();
        if (user == null)
        {
            result.Errors.Append(new IdentityError { Description = $"Unable to load user with ID '{userId}'." });
            return result;
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        result = await _userManager.ConfirmEmailAsync(user, code);
        return result;
    }

    private IUserEmailStore<IdentityUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<IdentityUser>)_userStore;
    }
}
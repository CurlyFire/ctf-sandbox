using Microsoft.AspNetCore.Mvc;
using ctf_sandbox.Models;
using Microsoft.AspNetCore.Authorization;
using ctf_sandbox.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ctf_sandbox.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IRegistrationService _registrationService;

    public AccountController(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }



    [AllowAnonymous]
    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterAccountRequest request)
    {

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        else
        {
            var result = await _registrationService.RegisterAccountAsync(request.Email, request.Password, (userId, code) =>
                {
                    return Url.Action("ConfirmEmail", "Account", new { userId, code }, Request.Scheme) ?? string.Empty;
                });
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }
        }
    }
    
    [AllowAnonymous]
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
    {
        var result = await _registrationService.ConfirmAccountAsync(userId, code);
        if (result.Succeeded)
        {
            return Ok("Email confirmed successfully.");
        }
        else
        {
            return BadRequest("Error confirming your email.");
        }
    }
}

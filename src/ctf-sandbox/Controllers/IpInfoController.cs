using ctf_sandbox.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class IpInfoController : ControllerBase
{
    private readonly IIpInfoService _ipInfoService;

    public IpInfoController(IIpInfoService ipInfoService)
    {
        _ipInfoService = ipInfoService;
    }

    [HttpGet("{ipAddress}")]
    public async Task<IActionResult> GetIpInfo(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return BadRequest("IP address is required");
        }

        var ipInfo = await _ipInfoService.GetIpInfoAsync(ipAddress);
        
        if (ipInfo == null)
        {
            return NotFound($"Could not retrieve information for IP address: {ipAddress}");
        }

        return Ok(ipInfo);
    }

}

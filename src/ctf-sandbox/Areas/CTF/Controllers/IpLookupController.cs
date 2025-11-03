using System.Net;
using ctf_sandbox.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.Areas.CTF.Controllers;

[Area("CTF")]
[Authorize] // Ensures only logged-in users can access
public class IpLookupController : Controller
{
    private readonly IIpInfoService _ipInfoService;

    public IpLookupController(IIpInfoService ipInfoService)
    {
        _ipInfoService = ipInfoService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Lookup(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip) || !IPAddress.TryParse(ip, out _))
        {
            return BadRequest("Invalid IP address");
        }

        var ipInfo = await _ipInfoService.GetIpInfoAsync(ip);
        if (ipInfo == null)
        {
            return NotFound("Could not retrieve information for the specified IP address");
        }

        return Json(ipInfo);
    }
}

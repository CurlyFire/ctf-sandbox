using ctf_sandbox.Models;
using Microsoft.Playwright;

namespace ctf_sandbox.tests.Drivers.UI.PageObjectModels;

public class IpInfoPage
{
    private readonly IPage _page;

    public IpInfoPage(IPage page)
    {
        _page = page;
    }

    public async Task<IpInfo> GetIpInfo(string ipAddress)
    {
        // Enter IP address in the input field
        await _page.Locator("#ipAddress").FillAsync(ipAddress);
        
        // Click the Lookup button
        await _page.GetByRole(AriaRole.Button, new() { Name = "Lookup IP address" }).ClickAsync();
        
        // Wait for results to appear
        await _page.Locator("#results").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        
        // Extract data from the results table
        var ipInfo = new IpInfo
        {
            Ip = await _page.Locator("[data-testid='result-ip']").TextContentAsync() ?? string.Empty,
            Hostname = await _page.Locator("[data-testid='result-hostname']").TextContentAsync(),
            City = await _page.Locator("[data-testid='result-city']").TextContentAsync(),
            Region = await _page.Locator("[data-testid='result-region']").TextContentAsync(),
            Country = await _page.Locator("[data-testid='result-country']").TextContentAsync(),
            Location = await _page.Locator("[data-testid='result-location']").TextContentAsync(),
            Organization = await _page.Locator("[data-testid='result-org']").TextContentAsync(),
            PostalCode = await _page.Locator("[data-testid='result-postal']").TextContentAsync(),
            Timezone = await _page.Locator("[data-testid='result-timezone']").TextContentAsync()
        };
        
        // Parse the anycast value
        var anycastText = await _page.Locator("[data-testid='result-anycast']").TextContentAsync();
        ipInfo.Anycast = anycastText?.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase);
        
        return ipInfo;
    }
}

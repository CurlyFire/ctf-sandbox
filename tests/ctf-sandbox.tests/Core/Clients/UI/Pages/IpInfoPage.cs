using ctf_sandbox.Models;
using Microsoft.Playwright;

namespace ctf_sandbox.tests.Core.Clients.UI.Pages;

public class IpInfoPage : ErrorPage
{
    public IpInfoPage(IPage page) : base(page)
    {
    }

    public async Task<IpInfo> GetIpInfo(string ipAddress)
    {
        // Enter IP address in the input field
        await Page.Locator("#ipAddress").FillAsync(ipAddress);
        
        // Click the Lookup button
        await Page.GetByRole(AriaRole.Button, new() { Name = "Lookup IP address" }).ClickAsync();
        
        // Wait for results to appear
        await Page.Locator("#results").WaitForAsync(new() { State = WaitForSelectorState.Visible });
        
        // Extract data from the results table
        var ipInfo = new IpInfo
        {
            Ip = await Page.Locator("[data-testid='result-ip']").TextContentAsync() ?? string.Empty,
            Hostname = await Page.Locator("[data-testid='result-hostname']").TextContentAsync(),
            City = await Page.Locator("[data-testid='result-city']").TextContentAsync(),
            Region = await Page.Locator("[data-testid='result-region']").TextContentAsync(),
            Country = await Page.Locator("[data-testid='result-country']").TextContentAsync(),
            Location = await Page.Locator("[data-testid='result-location']").TextContentAsync(),
            Organization = await Page.Locator("[data-testid='result-org']").TextContentAsync(),
            PostalCode = await Page.Locator("[data-testid='result-postal']").TextContentAsync(),
            Timezone = await Page.Locator("[data-testid='result-timezone']").TextContentAsync()
        };
        
        // Parse the anycast value
        var anycastText = await Page.Locator("[data-testid='result-anycast']").TextContentAsync();
        ipInfo.Anycast = anycastText?.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase);
        
        return ipInfo;
    }
}

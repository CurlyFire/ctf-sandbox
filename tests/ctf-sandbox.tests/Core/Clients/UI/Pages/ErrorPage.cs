using Microsoft.Playwright;
using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Clients.UI.Pages;

public class ErrorPage
{
    protected IPage Page { get; }

    public ErrorPage(IPage page)
    {
        Page = page;
    }

    public async Task<ValidationProblemDetails?> GetErrors()
    {
        var errors = new Dictionary<string, string[]>();
        string? title = null;
        string? detail = null;

        // Error page
        if (await Page.GetByRole(AriaRole.Alert, new() { Name = "Application error", Exact = true }).IsVisibleAsync())
        {
            title = "Error page";
            detail = string.Join(",", await GetVisibleTextValues(Page.Locator(".text-danger")));
        }
        // Developer exception page
        else if (await Page.GetByText("An unhandled exception occurred while processing the request.", new() { Exact = true }).IsVisibleAsync())
        {
            title = "Developer exception page";
            detail = await Page.Locator(".titleerror").TextContentAsync();
        }
        else
        {
            var validationSummary = Page.Locator(".validation-summary-errors");
            if (await validationSummary.IsVisibleAsync())
            {
                var summaryMessages = await GetVisibleMessages(validationSummary);
                if (summaryMessages.Length > 0)
                {
                    title = string.Join(" ", summaryMessages);
                }
            }

            foreach (var fieldMessage in await Page.Locator("[data-valmsg-for]").AllAsync())
            {
                if (!await fieldMessage.IsVisibleAsync())
                {
                    continue;
                }

                var message = (await fieldMessage.TextContentAsync())?.Trim();
                var fieldName = await fieldMessage.GetAttributeAsync("data-valmsg-for");
                if (!string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(fieldName))
                {
                    errors[fieldName] = [message];
                }
            }
        }

        if (title is null && errors.Count == 0)
        {
            return null;
        }

        return new ValidationProblemDetails(errors) { Title = title };
    }

    private static async Task<string[]> GetVisibleTextValues(ILocator locator)
    {
        var values = new List<string>();
        foreach (var item in await locator.AllAsync())
        {
            if (await item.IsVisibleAsync())
            {
                var value = (await item.TextContentAsync())?.Trim();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    values.Add(value);
                }
            }
        }

        return values.ToArray();
    }

    private static async Task<string[]> GetVisibleMessages(ILocator locator)
    {
        var messages = new List<string>();
        foreach (var item in await locator.Locator("li").AllAsync())
        {
            if (await item.IsVisibleAsync())
            {
                var message = (await item.TextContentAsync())?.Trim();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    messages.Add(message);
                }
            }
        }

        return messages.ToArray();
    }
}
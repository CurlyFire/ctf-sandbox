using ctf_sandbox.tests.PageObjectModels;
namespace ctf_sandbox.tests.Dsl;

public class CTFDsl
{
    private readonly HomePage _homePage;

    public CTFDsl(HomePage page)
    {
        _homePage = page;
    }

    public async Task<EmailsDsl> CheckEmails()
    {
        var emailsPage = await _homePage.GoToEmailsPage();
        return new EmailsDsl(emailsPage);
    }


}

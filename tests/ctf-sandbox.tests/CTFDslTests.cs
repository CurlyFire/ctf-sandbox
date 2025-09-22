using ctf_sandbox.tests.Dsl;

namespace ctf_sandbox.tests;

public class CTFDslTests
{
    protected CTFDsl CTFDsl { get; private set; }
    public CTFDslTests()
    {
        var webServer = new WebServer();
        var pageTest = new WebServerPageTest(webServer);
        var homePage = pageTest.GoToHomePage().GetAwaiter().GetResult();
        CTFDsl = new CTFDsl(homePage, webServer.WebServerCredentials, webServer.MailpitCredentials);
    }
}

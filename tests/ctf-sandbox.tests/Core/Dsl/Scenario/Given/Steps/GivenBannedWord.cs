namespace ctf_sandbox.tests.Core.Dsl.Scenario.Given.Steps;

public class GivenBannedWord : BaseGiven
{
    private string _word = ScenarioDefaults.DefaultBannedWord;

    internal GivenBannedWord(GivenStage givenStage) : base(givenStage)
    {
    }

    public GivenBannedWord WithWord(string word)
    {
        _word = word;
        return this;
    }

    internal override Task Execute(UseCaseDsl app) => app.ExternalSystems.BannedWords.CreateBannedWord(_word);
}

namespace ctf_sandbox.tests.Core.Dsl.Scenario.Given.Steps;

public abstract class BaseGiven
{
    private readonly GivenStage _givenStage;

    protected BaseGiven(GivenStage givenStage)
    {
        _givenStage = givenStage;
    }

    public GivenStage And() => _givenStage;

    public When.WhenStage When() => _givenStage.When();

    internal abstract Task Execute(UseCaseDsl app);
}

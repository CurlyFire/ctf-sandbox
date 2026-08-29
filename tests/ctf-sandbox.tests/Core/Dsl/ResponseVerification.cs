namespace ctf_sandbox.tests.Core.Dsl;

public class ResponseVerification<TResponse>
{
    public ResponseVerification(TResponse response, UseCaseContext context)
    {
        Response = response;
        Context = context;
    }

    protected TResponse Response { get; }
    protected UseCaseContext Context { get; }
}

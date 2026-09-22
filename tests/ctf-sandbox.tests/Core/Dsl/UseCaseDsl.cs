namespace ctf_sandbox.tests.Core.Dsl;

public class UseCaseDsl
{
    public CTFDsl CTF { get; }
    public ExternalSystemsDsl ExternalSystems { get; }

    public UseCaseDsl(CTFDsl ctf, ExternalSystemsDsl externalSystems)
    {
        CTF = ctf;
        ExternalSystems = externalSystems;
    }
}
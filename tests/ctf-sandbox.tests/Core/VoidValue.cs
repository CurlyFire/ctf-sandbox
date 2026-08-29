namespace ctf_sandbox.tests.Core;

public class VoidValue
{
    private VoidValue() { }

    public static VoidValue Empty { get; } = new VoidValue();
}

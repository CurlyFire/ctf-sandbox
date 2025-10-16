using System.Reflection;
using Xunit.Sdk;

namespace ctf_sandbox.tests.Utils;

public class ChannelAttribute : DataAttribute
{
    private readonly Channel[] _channels;

    public ChannelAttribute(params Channel[] channels)
    {
        _channels = channels;
    }


    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (_channels.Length == 0)
        {
            throw new ArgumentException("At least one channel must be specified.", nameof(_channels));
        }

        return _channels.Select(channel => new object[] { channel });
    }
}
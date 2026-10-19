using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests
{
    [CollectionDefinition(Name)]
    public class CTFTestCollection : ICollectionFixture<TestEnvironments>
    {
        public const string Name = nameof(CTFTestCollection);
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
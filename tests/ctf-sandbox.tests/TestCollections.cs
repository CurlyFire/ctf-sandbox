using ctf_sandbox.tests.Fixtures;

namespace ctf_sandbox.tests
{
    [CollectionDefinition(Name)]
    public class RealExternalSystemsTestCollection : ICollectionFixture<RealExternalSystemsEnvironmentFixture>
    {
        public const string Name = nameof(RealExternalSystemsTestCollection);
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [CollectionDefinition(Name)]
    public class StubbedExternalSystemsTestCollection : ICollectionFixture<StubbedExternalSystemsEnvironmentFixture>
    {
        public const string Name = nameof(StubbedExternalSystemsTestCollection);
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
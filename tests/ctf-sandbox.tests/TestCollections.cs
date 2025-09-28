using Xunit;

namespace ctf_sandbox.tests
{
    [CollectionDefinition("Server Tests")]
    public class ServerTestCollection : ICollectionFixture<Fixtures.ServerFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [CollectionDefinition("HomePage Tests")]
    public class HomePageTestCollection : ICollectionFixture<Fixtures.HomePageFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
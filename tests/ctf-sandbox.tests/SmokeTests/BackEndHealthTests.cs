using System.Data;
using Microsoft.Data.Sqlite;

namespace ctf_sandbox.tests.SmokeTests;

public class BackEndHealthTests : IClassFixture<ServerConfiguration>
{
    private readonly ServerConfiguration _serverConfiguration;

    public BackEndHealthTests(ServerConfiguration serverConfiguration)
    {
        _serverConfiguration = serverConfiguration;
    }

    [Trait("Category", "Smoke_BackEndHealth")]
    [Fact]
    public async Task Database_ShouldBeUpAndRunning()
    {
        // Arrange
        using var connection = new SqliteConnection(_serverConfiguration.DatabaseConnectionString);

        // Act
        await connection.OpenAsync();
        
        // Assert
        Assert.True(connection.State == ConnectionState.Open);
        
        // Cleanup
        await connection.CloseAsync();
    }
}

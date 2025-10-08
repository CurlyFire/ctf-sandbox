using System.Data;
using ctf_sandbox.tests.Fixtures;
using Microsoft.Data.Sqlite;

namespace ctf_sandbox.tests.SmokeTests;

public class BackEndHealthTests : IClassFixture<EnvironmentConfiguration>
{
    private readonly EnvironmentConfiguration _environmentConfiguration;

    public BackEndHealthTests(EnvironmentConfiguration environmentConfiguration)
    {
        _environmentConfiguration = environmentConfiguration;
    }

    [Trait("Category", "Smoke_BackEndHealth")]
    [Fact]
    public async Task Database_ShouldBeUpAndRunning()
    {
        // Arrange
        using var connection = new SqliteConnection(_environmentConfiguration.DatabaseConnectionString);

        // Act
        await connection.OpenAsync();
        
        // Assert
        Assert.True(connection.State == ConnectionState.Open);
        
        // Cleanup
        await connection.CloseAsync();
    }
}

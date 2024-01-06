using DbUp;

namespace MisAPI.Data;

public class DatabaseMigrator
{
    private readonly IConfiguration _configuration;

    public DatabaseMigrator(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void MigrateDatabase()
    {
        
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var migrationScriptsPath = _configuration.GetConnectionString("MigrationScriptsPath");
        
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        
        var runner = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsFromFileSystem(migrationScriptsPath)
            .LogToConsole()
            .LogScriptOutput()
            .WithVariablesDisabled()
            .Build();
        
        
        var result = runner.PerformUpgrade();
        
        if (!result.Successful)
        {
            throw new Exception("Database migration failed" + result.Error);
        }
    }


}
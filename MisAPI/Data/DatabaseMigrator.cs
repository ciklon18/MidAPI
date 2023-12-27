using System.Reflection;
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
        var migrationScriptsPath = _configuration.GetValue<string>("MigrationScriptsPath");
        
        var runner = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            // .WithScriptsFromFileSystem(migrationScriptsPath)
            .LogToConsole()
            .LogScriptOutput()
            .WithVariablesDisabled()
            .Build();
        
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        
        var result = runner.PerformUpgrade();
        
        if (!result.Successful)
        {
            throw new Exception("Database migration failed" + result.Error);
        }
    }
}
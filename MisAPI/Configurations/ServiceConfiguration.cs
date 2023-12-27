using System.IdentityModel.Tokens.Jwt;
using MisAPI.Converters;
using MisAPI.Data;
using MisAPI.Services.Impls;

namespace MisAPI.Configurations;

public static class ServiceConfiguration
{
    public static void AddSingletons(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddSingleton<JwtSecurityTokenHandler>();
        services.AddSingleton<Tokens>();
        services.AddSingleton<JsonDateTimeConverter>();
        services.AddSingleton<DatabaseMigrator>();

    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddHostedService<TokenCleanupService>();
    }
}
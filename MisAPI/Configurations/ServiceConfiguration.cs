using MisAPI.Services.Impls;
using MisAPI.Services.Interfaces;

namespace MisAPI.Configurations;

public static class ServiceConfiguration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddHostedService<TokenCleanupService>();
    }
}
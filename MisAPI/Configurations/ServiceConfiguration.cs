using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using MisAPI.Converters;
using MisAPI.Data;
using MisAPI.Services.Impls;
using MisAPI.Services.Interfaces;

namespace MisAPI.Configurations;

public static class ServiceConfiguration
{
    public static void AddSingletons(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddSingleton<JwtSecurityTokenHandler>();
        services.AddSingleton<Tokens>();
        services.AddSingleton<JsonDateTimeConverter>();
        services.AddSingleton<GuidConverter>();
        services.AddSingleton<GenderConverter>();
        services.AddSingleton<DatabaseMigrator>();

    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddHostedService<TokenCleanupService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IIcd10DictionaryService, Icd10DictionaryService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IInspectionService, InspectionService>();
        services.AddScoped<IConsultationService, ConsultationService>();
        services.AddScoped<IReportService, ReportService>();
        
    }
}
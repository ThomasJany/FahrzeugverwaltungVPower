using FahrzeugverwaltungVPower.Infrastructure;

namespace FahrzeugverwaltungVPower.API;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddHttpContextAccessor();
                
        string connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddInfrastructureServices(connectionString);

        return services;
    }
}

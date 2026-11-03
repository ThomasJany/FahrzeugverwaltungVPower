using FahrzeugverwaltungVPower.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FahrzeugverwaltungVPower.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
    {        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, o =>
            {
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                o.CommandTimeout(300);
            }));

        return services;
    }
}

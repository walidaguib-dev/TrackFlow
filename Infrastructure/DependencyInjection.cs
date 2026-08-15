using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection GetInfrastructureServices(
        this IServiceCollection services,
        WebApplicationBuilder builder
    )
    {
        services.AddDatabase(builder.Configuration);
        services.AddFusionCache();
        services.AddAuthenticationServices(builder);
        services.ConfigureCloudinary(builder);
        services.ConfigureBackgroundJobs(builder);
        services.AddRateLimitingServices();
        services.AddLoggingConfiguration(builder.Configuration);
        return services;
    }
}

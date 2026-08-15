using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Services;

namespace API
{
    public static class DependencyInjection
    {
        public static IServiceCollection GetApiServices(this IServiceCollection services)
        {
            services.AddDependencies();
            services.ConfigureGraphQL();
            services.ConfigureOpenApi();
            services.GetValidationServices();
            return services;
        }
    }
}

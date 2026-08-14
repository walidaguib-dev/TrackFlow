using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public static class Graphql
    {
        public static IServiceCollection ConfigureGraphQL(this IServiceCollection services)
        {
            services
                .AddGraphQLServer()
                .AddGraphQL()
                .ModifyCostOptions(options =>
                {
                    options.MaxFieldCost = 10000;
                })
                .AddProjections()
                .AddSorting()
                .AddFiltering()
                .AddPagingArguments();
            return services;
        }
    }
}

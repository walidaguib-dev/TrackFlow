using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.helpers;

namespace API.Services
{
    public static class OpenApi
    {
        public static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
        {
            services.AddOpenApi(
                "v1",
                options =>
                {
                    options.AddDocumentTransformer<OpenApiTransformer>();
                    options.AddDocumentTransformer(
                        (doc, ctx, ct) =>
                        {
                            doc.Info.Title = "CargoPin System API";
                            doc.Info.Description =
                                "Port operations API for managing Merchandises positions in Port";
                            doc.Info.Version = "v1";
                            return Task.CompletedTask;
                        }
                    );
                }
            );
            return services;
        }
    }
}

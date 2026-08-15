using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Routes
{
    public static class RoutesMapper
    {
        public static void MapEndpoints(this WebApplication app)
        {
            app.MapAuthEndpoints();
        }
    }
}

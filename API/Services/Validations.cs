using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Auth.Register;
using FluentValidation;

namespace API.Services
{
    public static class Validations
    {
        public static IServiceCollection GetValidationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
            return services;
        }
    }
}

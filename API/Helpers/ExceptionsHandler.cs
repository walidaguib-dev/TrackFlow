using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace API.Helpers
{
    /// <summary>
    /// Global exception handling middleware
    /// </summary>
    public static class ExceptionsHandler
    {
        /// <summary>
        /// Adds global exception handling with environment-aware responses
        /// </summary>
        /// <param name="builder">The application builder</param>
        /// <param name="environment">The hosting environment</param>
        /// <returns>The application builder</returns>
        public static IApplicationBuilder UseGlobalExceptionHandling(
            this IApplicationBuilder builder,
            IHostEnvironment environment
        )
        {
            builder.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                    switch (exception)
                    {
                        // 400 - Validation Errors (FluentValidation)
                        case ValidationException validationException:
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            var errors = validationException.Errors.Select(e => new
                            {
                                field = e.PropertyName,
                                message = e.ErrorMessage,
                            });
                            await context.Response.WriteAsJsonAsync(new { errors });
                            break;

                        // 401 - Authentication Errors
                        case UnauthorizedAccessException:
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsJsonAsync(
                                new
                                {
                                    error = "You are not authorized to perform this action.",
                                    errorCode = "UNAUTHORIZED",
                                }
                            );
                            break;

                        // 404 - Resource Not Found
                        case System.Collections.Generic.KeyNotFoundException:
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                            await context.Response.WriteAsJsonAsync(
                                new { error = exception.Message, errorCode = "RESOURCE_NOT_FOUND" }
                            );
                            break;

                        // 409 - Conflict (e.g., Duplicate)
                        case InvalidOperationException
                            when exception.Message.Contains(
                                "already exists",
                                StringComparison.OrdinalIgnoreCase
                            ):
                            context.Response.StatusCode = StatusCodes.Status409Conflict;
                            await context.Response.WriteAsJsonAsync(
                                new { error = exception.Message, errorCode = "CONFLICT" }
                            );
                            break;

                        // 400 - Argument/Invalid Operation
                        case ArgumentException:
                        case InvalidOperationException:
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            await context.Response.WriteAsJsonAsync(
                                new { error = exception.Message, errorCode = "INVALID_REQUEST" }
                            );
                            break;

                        // 500 - Everything Else
                        default:
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            if (environment.IsDevelopment())
                            {
                                await context.Response.WriteAsJsonAsync(
                                    new
                                    {
                                        error = exception?.Message,
                                        stackTrace = exception?.StackTrace,
                                        innerException = exception?.InnerException?.Message,
                                        exceptionType = exception?.GetType().Name,
                                    }
                                );
                            }
                            else
                            {
                                await context.Response.WriteAsJsonAsync(
                                    new
                                    {
                                        error = "An unexpected error occurred.",
                                        errorCode = "INTERNAL_SERVER_ERROR",
                                    }
                                );
                            }
                            break;
                    }
                });
            });

            return builder;
        }
    }
}

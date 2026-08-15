using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Auth.Register;
using MediatR;

namespace API.Routes
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/auth").WithTags("Auth");

            group
                .MapPost(
                    "/register",
                    async (
                        ISender sender,
                        RegisterRequest request,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        var command = new RegisterUserCommand(request);
                        var response = await sender.Send(command, cancellationToken);
                        return Results.Created(
                            uri: "/api/auth/register",
                            value: new
                            {
                                success = response.Success,
                                message = response.Message,
                                createdAt = response.CreatedAt,
                            }
                        );
                    }
                )
                .WithName("Register")
                .WithDescription("Register a new user")
                .AllowAnonymous();
            ;
        }
    }
}

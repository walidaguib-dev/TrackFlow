using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Auth.Register
{
    public class RegisterHandler(IUsersRepository usersRepository)
        : IRequestHandler<RegisterUserCommand, RegisterResponse>
    {
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<RegisterResponse> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken
        )
        {
            var user = request.RegisterRequest.MapToUser();
            var role = Enum.TryParse<UserRoles>(request.RegisterRequest.Role, true, out var roles)
                ? roles
                : UserRoles.Customer;
            var result = await _usersRepository.AddAsync(
                user,
                role,
                request.RegisterRequest.Password,
                cancellationToken
            );
            return new RegisterResponse
            {
                Success = true,
                Message = "Registration successful. Please verify your email.",
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}

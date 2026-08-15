using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories
{
    public class UsersRepository(UserManager<User> userManager) : IUsersRepository
    {
        private readonly UserManager<User> _userManager = userManager;

        public async Task<User> AddAsync(
            User user,
            UserRoles role,
            string Password,
            CancellationToken cancellationToken = default
        )
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email!);
            if (existingUser is not null)
            {
                throw new InvalidDataException("User already exists!");
            }

            var result = await _userManager.CreateAsync(user, Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role.ToString());
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Role assignment failed: {errors}");
            }
            return user;
        }
    }
}

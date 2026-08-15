using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces
{
    public interface IUsersRepository
    {
        public Task<User> AddAsync(
            User user,
            UserRoles role,
            string Password,
            CancellationToken cancellationToken = default
        );
    }
}

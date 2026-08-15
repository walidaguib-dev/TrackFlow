using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Features.Auth.Register
{
    public static class RegisterMapper
    {
        public static User MapToUser(this RegisterRequest request)
        {
            return new User { Email = request.Email, UserName = request.UserName };
        }
    }
}

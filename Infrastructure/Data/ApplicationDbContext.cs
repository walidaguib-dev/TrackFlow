using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var roles = new List<IdentityRole>
            {
                new()
                {
                    Id = "11111111-1111-1111-1111-111111111111",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "11111111-aaaa-bbbb-cccc-111111111111",
                },
                new()
                {
                    Id = "22222222-2222-2222-2222-222222222222",
                    Name = "Customer",
                    NormalizedName = "CUSTOMER",
                    ConcurrencyStamp = "22222222-aaaa-bbbb-cccc-222222222222",
                },
                new()
                {
                    Id = "33333333-3333-3333-3333-333333333333",
                    Name = "Driver",
                    NormalizedName = "DRIVER",
                    ConcurrencyStamp = "33333333-aaaa-bbbb-cccc-333333333333",
                },
                new()
                {
                    Id = "44444444-4444-4444-4444-444444444444",
                    Name = "Dispatcher",
                    NormalizedName = "DISPATCHER",
                    ConcurrencyStamp = "44444444-aaaa-bbbb-cccc-444444444444",
                },
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}

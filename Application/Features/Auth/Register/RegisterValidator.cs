using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Features.Auth.Register
{
    public class RegisterValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.RegisterRequest.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MaximumLength(100)
                .WithMessage("Email must not exceed 100 characters");

            RuleFor(x => x.RegisterRequest.UserName)
                .NotEmpty()
                .WithMessage("Full name is required")
                .MinimumLength(2)
                .WithMessage("Full name must be at least 2 characters")
                .MaximumLength(100)
                .WithMessage("Full name must not exceed 100 characters");

            RuleFor(x => x.RegisterRequest.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters")
                .MaximumLength(100)
                .WithMessage("Password must not exceed 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
                .WithMessage(
                    "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character"
                );

            RuleFor(x => x.RegisterRequest.Role)
                .NotEmpty()
                .WithMessage("Role is required")
                .Must(BeValidRole)
                .WithMessage("Role must be one of: Admin, Customer, Driver, Dispatcher");
        }

        private static bool BeValidRole(string role)
        {
            var validRoles = new[] { "Admin", "Customer", "Driver", "Dispatcher" };
            return validRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }
    }
}

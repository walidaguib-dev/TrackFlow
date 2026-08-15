using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Auth.Register
{
    /// <summary>
    /// Minimal response DTO — no sensitive data exposed
    /// </summary>
    public class RegisterResponse
    {
        /// <summary>
        /// Indicates whether registration was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// User-friendly message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// When the account was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}

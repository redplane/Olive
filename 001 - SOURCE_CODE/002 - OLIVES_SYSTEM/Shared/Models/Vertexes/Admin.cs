using Shared.Enumerations;

namespace Shared.Models.Vertexes
{
    public class Admin
    {
        /// <summary>
        /// Email of admin.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Encrypted password of admin account.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Status of admin account.
        /// </summary>
        public AccountStatus Status { get; set; }
    }
}
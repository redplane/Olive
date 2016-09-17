using ArangoDB.Client;
using Shared.Enumerations;

namespace Shared.Models.Vertexes
{
    public class Account
    {
        /// <summary>
        /// Id of account in graph database.
        /// </summary>
        [DocumentProperty(Identifier = IdentifierType.Handle)]
        public string Id { get; set; }

        /// <summary>
        /// Email of account.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// First name of account.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of account.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Encrypted password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Photo of account.
        /// </summary>
        public Photo Photo { get; set; }

        /// <summary>
        /// Status of account.
        /// </summary>
        public AccountStatus Status { get; set; }

        /// <summary>
        /// Gender account owner.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// Phone which account owner uses.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Account owner birthday.
        /// </summary>
        public double Birthday { get; set; }

        /// <summary>
        /// Role of account.
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// When the account was created.
        /// </summary>
        public double Created { get; set; }

        /// <summary>
        /// When the account was lastly modified.
        /// </summary>
        public double? LastModified { get; set; }
    }
}
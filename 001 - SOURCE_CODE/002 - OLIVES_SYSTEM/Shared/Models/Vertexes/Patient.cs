using Shared.Enumerations;

namespace Shared.Models.Vertexes
{
    public class Patient
    {
        /// <summary>
        /// Email of patient.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Patient first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Patient last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Password of patient account.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// When the patient was born.
        /// </summary>
        public double Birthday { get; set; }

        /// <summary>
        /// Phone number which patient uses.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gender of patient.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// Status of patient account.
        /// </summary>
        public AccountStatus Status { get; set; }

        /// <summary>
        /// When the account was created.
        /// </summary>
        public double Created { get; set; }

        /// <summary>
        /// When the account was lastly created.
        /// </summary>
        public double? LastModified { get; set; }
    }
}
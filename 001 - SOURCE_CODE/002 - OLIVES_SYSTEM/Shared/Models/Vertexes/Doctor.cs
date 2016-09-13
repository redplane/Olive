using Shared.Enumerations;

namespace Shared.Models.Vertexes
{
    public class Doctor
    {
        /// <summary>
        /// Email of doctor.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// First name of doctor.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of doctor.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Password of doctor account.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Specialty in which doctor operates.
        /// </summary>
        public string Specialty { get; set; }

        /// <summary>
        /// Rank of double.
        /// </summary>
        public double? Rank { get; set; }

        /// <summary>
        /// Account status.
        /// </summary>
        public AccountStatus Status { get; set; }

        /// <summary>
        /// Gender of doctor.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// Phone number which doctor uses.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Birthday of doctor.
        /// </summary>
        public double Birthday { get; set; }

        /// <summary>
        /// Doctor avatar.
        /// </summary>
        public Photo Photo { get; set; }

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
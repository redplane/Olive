using Shared.Enumerations;

namespace Shared.ViewModels
{
    public class PersonViewModel
    {
        /// <summary>
        /// Id of person.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Person first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Person last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Person email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Personal password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Personal birthday.
        /// </summary>
        public double? Birthday { get; set; }

        /// <summary>
        /// Personal role.
        /// </summary>
        public byte Role { get; set; }

        /// <summary>
        /// Created date.
        /// </summary>
        public double Created { get; set; }

        /// <summary>
        /// The last time profile was modified.
        /// </summary>
        public double? LastModified { get; set; }

        /// <summary>
        /// Gender of person.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// Status of account.
        /// </summary>
        public StatusAccount Status { get; set; }

        /// <summary>
        /// Address of person.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Personal avatar.
        /// </summary>
        public string Photo { get; set; }

        public string Phone { get; set; }
    }
}
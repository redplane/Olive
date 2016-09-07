namespace Olive.Admin.ViewModels
{
    /// <summary>
    /// Account information.
    /// </summary>
    public class PersonViewModel
    {
        /// <summary>
        /// Id of account.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Email of account.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Account first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Account last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Birthday
        /// </summary>
        public double? Birthday { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gender of account.
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        /// Role of account.
        /// </summary>
        public byte Role { get; set; }

        /// <summary>
        /// When the account was created.
        /// </summary>
        public double Created { get; set; }
        
        /// <summary>
        /// Status of account.
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// Address where person lives.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Url of photo.
        /// </summary>
        public string PhotoUrl { get; set; }
    }
}
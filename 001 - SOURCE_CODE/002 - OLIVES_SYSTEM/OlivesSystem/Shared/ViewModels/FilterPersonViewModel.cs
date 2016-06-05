using Shared.Models;

namespace Shared.ViewModels
{
    public class FilterPersonViewModel
    {
        /// <summary>
        /// Id of person
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Person first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Person last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Date after that person was born
        /// </summary>
        public long? BirthdayFrom { get; set; }

        /// <summary>
        /// Date before which that person had been born
        /// </summary>
        public long? BirthdayTo { get; set; }

        /// <summary>
        /// Gender of person
        /// </summary>
        public byte? Gender { get; set; }

        /// <summary>
        /// Email of person
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Phone number of person
        /// </summary>
        public string Phone { get; set; }
        
        public long? MoneyFrom { get; set; }

        public long? MoneyTo { get; set; }

        public long? CreatedFrom { get; set; }

        public long? CreatedTo { get; set; }

        public byte? Role { get; set; }

        public byte? Status { get; set; }
        
        public int? Page { get; set; }

        public int? Records { get; set; }
    }
}
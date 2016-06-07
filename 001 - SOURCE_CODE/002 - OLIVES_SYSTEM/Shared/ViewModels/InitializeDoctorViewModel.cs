using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Models;
using Shared.Models.Nodes;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class InitializeDoctorViewModel : Doctor
    {
#pragma warning disable 108, 114
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireFirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireLastName")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireBirthday")]
        public long? Birthday { get; set; }

        [Range(Constants.Gender.Male, Constants.Gender.Female, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidGender")]
        public byte Gender { get; set; }

        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireEmail")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "RequirePassword")]
        public string Password { get; set; }

        [RegexMatch(Regexes.Phone, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "RequirePhone")]
        public string Phone { get; set; }

        /// <summary>
        ///     Money of account.
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        ///     For doctor, address is compulsory.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireAddress")]
        public Coordinate Address { get; set; }

        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireSpecialization")]
        public string Specialization { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "RequireIdentityCard")]
        public string IdentityCardNo { get; set; }

        public byte Status { get; set; }
#pragma warning restore 108, 114
    }
}
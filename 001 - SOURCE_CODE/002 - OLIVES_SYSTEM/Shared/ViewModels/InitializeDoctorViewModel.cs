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
        public string Address { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "RequireSpecialization")]
        public string Speciality { get; set; }

        /// <summary>
        ///     Latitude of place where person lives.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        ///     Longitude of place where person lives.
        /// </summary>
        public double? Longitude { get; set; }


        #region Identity card

        [MaxLength(FieldLength.IdentityCardNoMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidIdentityCardMaxLength")]
        [RegexMatch(Regexes.IdentityCard, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidIdentityCard")]
        public string IdentityCardNo { get; set; }
        
        [TickToYearCompare(Values.MinimumAllowedYear, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidIdentityCardIssueDate")]
        public long IdentityCardIssueDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "RequireIdentityCardIssuePlace")]
        [CoordinateValidate(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidIdentityCardIssuePlace")]
        public Coordinate IdentityCardIssuePlace { get; set; }

        #endregion

#pragma warning restore 108, 114
    }
}
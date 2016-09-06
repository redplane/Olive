using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olive.Admin.ViewModels.Edit
{
    public class EditAdminProfileViewModel
    {
        /// <summary>
        ///     Person first name.
        /// </summary>
        [MaxLength(FieldLength.LastNameMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidFirstName")]
        public string FirstName { get; set; }

        /// <summary>
        ///     Person last name.
        /// </summary>
        [MaxLength(FieldLength.LastNameMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidLastName")]
        public string LastName { get; set; }

        /// <summary>
        ///     Birthday (ticks).
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? Birthday { get; set; }

        /// <summary>
        ///     Person gender.
        /// </summary>
        [InEnumerationsArray(new object[] { Shared.Enumerations.Gender.Female, Shared.Enumerations.Gender.Male },
            ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidGender")]
        public Gender? Gender { get; set; }

        /// <summary>
        ///     Password of this account.
        /// </summary>
        [MinLength(Values.MinPasswordLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "MinLengthPassword")]
        [MaxLength(Values.MaxPasswordLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "MaxLengthPassword")]
        [RegexMatch(Regexes.Password, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "RegexPassword")]
        public string Password { get; set; }
        
        /// <summary>
        ///     Phone number which is used for contacting.
        /// </summary>
        [RegexMatch(Regexes.Phone, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidPhone")]
        public string Phone { get; set; }

        /// <summary>
        /// Address of admin
        /// </summary>
        [StringLength(FieldLength.MaxAddressLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Address { get; set; }
        
    }
}
using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;

namespace Shared.Models.Nodes
{
    public class Person : IPerson
    {
        /// <summary>
        ///     Photo link of avatar
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        ///     Status of account [0 - Disabled | 1 - Pending | 2 - Active].
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        ///     Person GUID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Person first name.
        /// </summary>
        [MaxLength(FieldLength.LastNameMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidFirstName")]
        public string FirstName { get; set; }

        /// <summary>
        ///     Person last name.
        /// </summary>
        [MaxLength(FieldLength.LastNameMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidLastName")]
        public string LastName { get; set; }

        /// <summary>
        ///     Birthday (ticks).
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public long? Birthday { get; set; }

        /// <summary>
        ///     Person gender.
        /// </summary>
        [Range(Constants.Gender.Male, Constants.Gender.Female, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidGender")]
        public byte Gender { get; set; }

        /// <summary>
        ///     Email address which is used for registration or for contacting.
        /// </summary>
        [MaxLength(FieldLength.EmailMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidEmailMaximumLength")]
        [RegularExpression(Regexes.Email, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidEmailFormat")]
        public string Email { get; set; }

        /// <summary>
        ///     Password of this account.
        /// </summary>
        // TODO: Implement dataannotation for password
        public string Password { get; set; }

        /// <summary>
        ///     Phone number which is used for contacting.
        /// </summary>
        [RegexMatch(Regexes.Phone, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidPhone")]
        public string Phone { get; set; }

        /// <summary>
        ///     Money in wallet.
        /// </summary>
        public double Money { get; set; }

        /// <summary>
        ///     Time when account has been registered.
        /// </summary>
        public long Created { get; set; }

        /// <summary>
        ///     Latitude of place where person lives.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        ///     Longitude of place where person lives.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        ///     Address of person.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     Role of person [0 - Admin | 1 - Patient | 2 - Doctor]
        /// </summary>
        public byte Role { get; set; }

        /// <summary>
        ///     Time when accout has been modified.
        /// </summary>
        public long LastModified { get; set; }
    }
}
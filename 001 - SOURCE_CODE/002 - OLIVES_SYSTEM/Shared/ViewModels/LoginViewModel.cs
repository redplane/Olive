using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class LoginViewModel
    {
        /// <summary>
        ///     Email which was used for account registration.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireEmail")]
        [RegularExpression(Regexes.Email, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidEmailFormat")]
        [MaxLength(FieldLength.EmailMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidEmailMaximumLength")]
        public string Email { get; set; }

        // TODO: Uncomment attributes below.
        /// <summary>
        ///     Password of account.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequirePassword")]
        [RegexMatch(Regexes.Password, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "RegexPassword")]
        public string Password { get; set; }

        public int? Role { get; set; }
    }
}
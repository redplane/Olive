using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace Olives.ViewModels
{
    public class RequestActivationCodeViewModel
    {
        /// <summary>
        ///     Email which is used as login name.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireEmail")]
        [MaxLength(FieldLength.EmailMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidEmailMaximumLength")]
        [RegularExpression(Regexes.Email, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidEmailFormat")]
        public string Email { get; set; }
    }
}
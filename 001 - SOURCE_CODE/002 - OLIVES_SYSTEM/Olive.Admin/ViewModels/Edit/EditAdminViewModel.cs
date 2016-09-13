using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;

namespace OliveAdmin.ViewModels.Edit
{
    public class EditAdminViewModel
    {
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
    }
}
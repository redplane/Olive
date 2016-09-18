using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace OliveAdmin.ViewModels.Edit
{
    public class EditAccountViewModel
    {
        /// <summary>
        /// Email of account whose information should be changed.
        /// </summary>
        public string Email { get; set; }

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

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        /// <summary>
        /// Status of account which can be modified to.
        /// </summary>
        [InEnumerationsArray(new object[] {AccountStatus.Disabled, AccountStatus.Active}, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public AccountStatus? Status { get; set; }

        /// <summary>
        /// Gender which account owner can be.
        /// </summary>
        [InEnumerationsArray(new object[] { Shared.Enumerations.Gender.Male, Shared.Enumerations.Gender.Female }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public Gender? Gender { get; set; }
        
        /// <summary>
        /// Birthday of account owner.
        /// </summary>
        public double? Birthday { get; set; }
    }
}
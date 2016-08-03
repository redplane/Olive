using System.ComponentModel.DataAnnotations;
using Olives.Attributes;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializeRatingViewModel
    {
        /// <summary>
        ///     Target of rating.
        /// </summary>
        [AccountValidate(AccountValidateInputType.Id, true, Role.Doctor, StatusAccount.Active, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public int Target { get; set; }
        
        /// <summary>
        /// Rate point of rating.
        /// </summary>
        [Range(1, 5, ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeFromTo")]
        public int Rate { get; set; }

        /// <summary>
        /// Comment of rating.
        /// </summary>
        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Comment { get; set; }
    }
}
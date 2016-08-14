using System.ComponentModel.DataAnnotations;
using Olives.Attributes;
using Olives.Constants;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializeDiaryViewModel
    {
        /// <summary>
        /// Patient id
        /// </summary>
        [AccountValidate(AccountValidateInputType.Id, true, Role.Patient, StatusAccount.Active, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public int Target { get; set; }

        /// <summary>
        ///     Time when the diary is noted.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double Time { get; set; }

        /// <summary>
        ///     Note of diary.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(OlivesValues.MaxDiaryLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Olives.Attributes;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializePrescriptionViewModel
    {
        [NumericCompare(1, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        public int MedicalRecord { get; set; } = 1;

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(32, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainKey")]
        public string Name { get; set; }

        [NumericPropertyCompare("To", Comparision = Comparision.LowerEqual, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueMustBeEqualLowerThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double From { get; set; }

        [NumericPropertyCompare("From", Comparision = Comparision.GreaterEqual,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeEqualGreaterThan")]
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double To { get; set; }

        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }

        [MedicineListKeyValidate(MaxLengthName = FieldLength.MaxDictionaryKeyLength,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueIsInvalid")]
        [MedicineListLengthValidate(FieldLength.MaxDictionaryLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainKey")]
        public Dictionary<string, MedicineInfoViewModel> Medicines { get; set; }
    }
}
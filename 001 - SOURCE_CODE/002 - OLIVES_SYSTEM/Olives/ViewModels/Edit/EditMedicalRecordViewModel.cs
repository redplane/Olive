using System.Collections.Generic;
using Olives.Attributes;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Edit
{
    public class EditMedicalRecordViewModel
    {
        /// <summary>
        /// Category of medical record.
        /// </summary>
        [MedicalCategoryValidate(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public int? Category { get; set; }

        /// <summary>
        ///     List of noticed information.
        /// </summary>
        [DictionaryLength(FieldLength.MaxDictionaryLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainKey")]
        [DictionaryKeyValueLength(FieldLength.MaxDictionaryKeyLength, FieldLength.MaxDictionaryValueLength,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public Dictionary<string, string> Infos { get; set; }

        /// <summary>
        ///     Time when the record is created.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? Time { get; set; }
    }
}
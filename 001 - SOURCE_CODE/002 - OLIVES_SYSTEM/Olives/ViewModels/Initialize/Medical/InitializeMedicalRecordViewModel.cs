using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Olives.Attributes;
using Olives.Constants;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Initialize.Medical
{
    public class InitializeMedicalRecordViewModel
    {
        /// <summary>
        ///     Owner of medical record
        ///     As no owner is defined, the requester will be the owner of medical record.
        /// </summary>
        public int? Owner { get; set; }

        /// <summary>
        ///     Id of the medical record creator.
        /// </summary>
        public int? Creator { get; set; }

        /// <summary>
        ///     Name of medical record.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(OlivesValues.MaxMedicalRecordNameLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }

        /// <summary>
        ///     Id of category.
        /// </summary>
        [MedicalCategoryValidate(ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsInvalid")]
        public int Category { get; set; }

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
        public double Time { get; set; }
    }
}
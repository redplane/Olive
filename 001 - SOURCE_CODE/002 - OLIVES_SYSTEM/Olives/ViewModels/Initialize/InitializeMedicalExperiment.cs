using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Initialize
{
    public class InitializeMedicalExperiment
    {
        /// <summary>
        ///     Medical record experiment should belong to.
        /// </summary>
        [NumericCompare(1, Comparision = Comparision.GreaterEqual, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public int MedicalRecord { get; set; }

        /// <summary>
        /// Name of experiment note.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(32, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }

        /// <summary>
        /// Time when the note is about.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double Time { get; set; }

        /// <summary>
        ///     Experiment information.
        /// </summary>
        [DictionaryLength(FieldLength.MaxDictionaryKeyLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainKey")]
        [DictionaryKeyLength(FieldLength.MaxDictionaryKeyLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public Dictionary<string, double> Infos { get; set; }
    }
}
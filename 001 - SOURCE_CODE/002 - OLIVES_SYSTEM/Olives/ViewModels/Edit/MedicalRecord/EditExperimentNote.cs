using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Resources;

namespace Olives.ViewModels.Edit.MedicalRecord
{
    public class EditExperimentNote
    {
        /// <summary>
        ///     Name of medical experiment.
        /// </summary>
        [StringLength(32, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }

        /// <summary>
        ///     Time when the note is about.
        /// </summary>
        [EpochTimeCompare(Values.MinimumAllowedYear, Comparision = Comparision.Greater,
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeAfterYear")]
        public double? Time { get; set; }

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
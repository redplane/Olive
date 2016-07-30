using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;

namespace Olives.ViewModels.Edit
{
    public class EditMedicalExperiment
    {
        [StringLength(32, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }

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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.Resources;
using Shared.ViewModels;

namespace Olives.ViewModels.Initialize
{
    public class InitializeMedicalExperiment
    {
        /// <summary>
        ///     Medical record experiment should belong to.
        /// </summary>
        public int MedicalRecord { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsRequired")]
        [StringLength(32, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Name { get; set; }

        /// <summary>
        ///     Experiment information.
        /// </summary>
        public List<ExperimentInfoViewModel> Info { get; set; }
    }
}
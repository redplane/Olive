using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shared.Resources;
using Shared.ViewModels;

namespace Olives.ViewModels.Modify
{
    public class ModifyExperimentViewModel
    {
        /// <summary>
        /// List of experiment information.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsRequired")]
        public List<ExperimentInfoViewModel> Infos { get; set; } 
    }
}
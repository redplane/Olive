using System.ComponentModel.DataAnnotations;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class ExperimentInfoViewModel
    {
        [StringLength(32, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Key { get; set; }

        public double Value { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace Shared.ViewModels.Initialize
{
    public class InitializeHeartbeatViewModel
    {
        [Range(Values.MinHeartRate, Values.MaxHeartRate, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueMustBeFromTo")]
        public double Rate { get; set; }

        [StringLength(Values.NoteMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueCanOnlyContainCharacter")]
        public string Note { get; set; }
    }
}
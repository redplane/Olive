using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class FilterPatientViewModel : FilterPersonViewModel
    {
        [Range(Values.MinBodyWeight, Values.MaxBodyWeight, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidMinWeight")]
        public float? MinWeight { get; set; }

        [Range(Values.MinBodyWeight, Values.MaxBodyWeight, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidMaxWeight")]
        public float? MaxWeight { get; set; }
        
        [Range(Values.MinBodyHeight, Values.MaxBodyHeight, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidMinHeight")]
        public float? MinHeight { get; set; }

        [Range(Values.MinBodyHeight, Values.MaxBodyHeight, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidMaxHeight")]
        public float? MaxHeight { get; set; }
    }
}
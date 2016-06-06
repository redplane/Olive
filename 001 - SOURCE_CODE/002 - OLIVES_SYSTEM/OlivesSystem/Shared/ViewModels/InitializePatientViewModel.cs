using System.ComponentModel.DataAnnotations;
using Shared.Constants;

namespace Shared.ViewModels
{
    public class InitializePatientViewModel : InitializePersonViewModel
    {
        [Range(1, 500, ErrorMessage = ErrorCodes.InvalidPersonWeight)]
        public float? Height { get; set; }

        [Range(20, 500, ErrorMessage = ErrorCodes.InvalidPersonHeight)]
        public float? Weight { get; set; }
        
        public string []  Anamneses { get; set; }
    }
}
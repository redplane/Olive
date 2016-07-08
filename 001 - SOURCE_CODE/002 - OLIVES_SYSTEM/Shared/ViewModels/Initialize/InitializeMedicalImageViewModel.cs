using System.ComponentModel.DataAnnotations;
using MultipartDataMediaFormatter.Infrastructure;
using Shared.Resources;

namespace Shared.ViewModels.Initialize
{
    public class InitializeMedicalImageViewModel
    {
        public int MedicalRecord { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsRequired")]
        public HttpFile File { get; set; }
    }
}
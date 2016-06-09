using System.ComponentModel.DataAnnotations;
using Shared.Resources;

namespace OlivesAdministration.ViewModels
{
    public class RetrieveDoctorViewModel
    {
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequirePersonalId")]
        public string Id { get; set; }
    }
}
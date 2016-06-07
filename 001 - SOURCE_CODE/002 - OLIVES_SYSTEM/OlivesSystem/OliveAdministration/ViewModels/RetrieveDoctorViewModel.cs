using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace OliveAdministration.ViewModels
{
    public class RetrieveDoctorViewModel
    {
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequirePersonalId")]
        public string Id { get; set; }

        [MaxLength(FieldLength.IdentityCardMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidIdentityCard")]
        public string IdentityCardNo { get; set; }
    }
}
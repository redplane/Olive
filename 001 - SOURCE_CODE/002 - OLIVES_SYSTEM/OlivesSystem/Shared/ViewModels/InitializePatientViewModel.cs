using System.ComponentModel.DataAnnotations;
using Shared.Models.Nodes;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class InitializePatientViewModel : Person
    {
#pragma warning disable 108, 114
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireFirstName")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireLastName")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireEmail")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "RequirePassword")]
        public string Password { get; set; }

        public string Phone { get; set; }

        public double Money { get; set; }

        [Range(1, 500, ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "InvalidWeight")]
        public float? Height { get; set; }

        [Range(20, 500, ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "InvalidHeight")]
        public float? Weight { get; set; }

        public string[] Anamneses { get; set; }
#pragma warning restore 108, 114
    }
}
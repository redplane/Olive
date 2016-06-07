using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Models;
using Shared.Models.Nodes;
using Shared.Resources;

namespace OliveAdministration.ViewModels
{
    public class EditDoctorViewModel : Person
    {
#pragma warning disable 108, 114
        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "InvalidPersonalId")]
        public string Id { get; set; }

        [CoordinateValidation(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "InvalidAddress")
        ]
        public Coordinate Address { get; set; }

        [Required(ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "RequireSpecialization")]
        public string Specialization { get; set; }

        public double Rank { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "RequireIdentityCard")]
        public string IdentityCardNo { get; set; }
#pragma warning restore 108, 114
    }
}
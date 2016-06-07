using System.ComponentModel.DataAnnotations;
using Shared.Constants;
using Shared.Resources;

namespace Shared.Models.Nodes
{
    public class Doctor : Person
    {
        [MaxLength(FieldLength.SpecializationMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidSpecializationLength")]
        public string Specialization { get; set; }

        public double Rank { get; set; }

        [MaxLength(FieldLength.IdentityCardMaxLength, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidIdentityCard")]
        public string IdentityCardNo { get; set; }
    }
}
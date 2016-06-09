using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;

namespace Shared.Models.Nodes
{
    public class IdentityCard
    {
        [MaxLength(FieldLength.IdentityCardNoMaxLength, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidIdentityCardMaxLength")]
        [RegexMatch("^[0-9]*", ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidIdentityCard")]
        public string Id { get; set; }

        public long IssueDate { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
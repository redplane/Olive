using System.ComponentModel.DataAnnotations;
using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;

namespace Shared.Models.Nodes
{
    public class IdentityCard
    {
        [MaxLength(FieldLength.IdentityCardNoMaxLength, ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidIdentityCardMaxLength")]
        [RegexMatch("^[0-9]*", ErrorMessageResourceType = typeof(Language),
            ErrorMessageResourceName = "InvalidIdentityCard")]
        public string No { get; set; }
        
        public long IssueDate { get; set; }
        
        [Range(-85, 85, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidLatitude")]
        public double Latitude { get; set; }
        
        [Range(-180, 180, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidLongitude")]
        public double Longitude { get; set; }
        
    }
}
using Shared.Attributes;
using Shared.Constants;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class StatusStatisticViewModel
    {
        [InNumericArray(new []{Roles.Patient, Roles.Doctor}, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "InvalidRole")]
        public byte? Role { get; set; }
        
        public byte Status { get; set; }

        public long Total { get; set; } 
    }
}
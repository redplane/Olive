using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class StatusSummaryViewModel
    {
        [InEnumerationsArray(new object[] { Enumerations.Role.Patient, Enumerations.Role.Doctor}, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidRole")]
        public byte? Role { get; set; }

        public int Status { get; set; }

        public long Total { get; set; }
    }
}
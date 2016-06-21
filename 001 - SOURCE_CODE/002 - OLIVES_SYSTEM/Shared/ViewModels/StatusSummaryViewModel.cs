using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class StatusSummaryViewModel
    {
        [InNumericArray(new[] {AccountRole.Patient, AccountRole.Doctor}, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidRole")]
        public byte? Role { get; set; }

        public int Status { get; set; }

        public long Total { get; set; }
    }
}
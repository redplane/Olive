using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace Shared.ViewModels
{
    public class StatusStatisticViewModel
    {
        [InNumericArray(new[] {(int)AccountRole.Patient, (int)AccountRole.Doctor}, ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidRole")]
        public int? Role { get; set; }

        public int Status { get; set; }

        public long Total { get; set; }
    }
}
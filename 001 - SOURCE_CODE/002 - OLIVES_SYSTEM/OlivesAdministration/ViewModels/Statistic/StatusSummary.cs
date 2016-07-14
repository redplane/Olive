using Shared.Attributes;
using Shared.Resources;

namespace OlivesAdministration.ViewModels.Statistic
{
    public class StatusSummary
    {
        /// <summary>
        ///     Role which is defined for statistic purpose.
        /// </summary>
        [InEnumerationsArray(new object[] {Shared.Enumerations.Role.Patient, Shared.Enumerations.Role.Doctor},
            ErrorMessageResourceType = typeof (Language),
            ErrorMessageResourceName = "InvalidRole")]
        public byte? Role { get; set; }
    }
}
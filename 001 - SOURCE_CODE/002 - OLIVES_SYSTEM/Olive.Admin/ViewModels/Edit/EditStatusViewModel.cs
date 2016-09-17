using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace OliveAdmin.ViewModels.Edit
{
    public class EditStatusViewModel
    {
        /// <summary>
        ///     Status of account.
        /// </summary>
        [InEnumerationsArray(new object[] {AccountStatus.Active, AccountStatus.Disabled, AccountStatus.Pending},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public AccountStatus Status { get; set; }
    }
}
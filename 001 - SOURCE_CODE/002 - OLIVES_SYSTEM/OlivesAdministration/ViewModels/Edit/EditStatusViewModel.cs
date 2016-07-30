using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace OlivesAdministration.ViewModels.Edit
{
    public class EditStatusViewModel
    {
        /// <summary>
        ///     Status of account.
        /// </summary>
        [InEnumerationsArray(new object[] {StatusAccount.Active, StatusAccount.Inactive, StatusAccount.Pending},
            ErrorMessageResourceType = typeof (Language), ErrorMessageResourceName = "ValueMustBeOneOfArray")]
        public StatusAccount Status { get; set; }
    }
}
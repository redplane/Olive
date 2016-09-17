using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace OliveAdmin.ViewModels.Edit
{
    public class EditOrdinaryProfileViewModel
    {
        /// <summary>
        /// Status of account.
        /// </summary>
        [InEnumerationsArray(new object[] { AccountStatus.Disabled, AccountStatus.Active }, ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public AccountStatus? Status { get; set; }
    }
}
using Shared.Enumerations;

namespace OliveAdmin.ViewModels.Filter
{
    public class FilterAdminViewModel
    {
        /// <summary>
        ///     Email of admin.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     Email text comparision.
        /// </summary>
        public TextComparision EmailComparision { get; set; }

        /// <summary>
        ///     Password of admin account (encrypted)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Password comparision mode.
        /// </summary>
        public TextComparision PasswordComparision { get; set; }

        /// <summary>
        ///     List of statuses.
        /// </summary>
        public AccountStatus[] Statuses { get; set; }
        
        /// <summary>
        /// Whether accounts should be sorted ascendingly or decendingly.
        /// </summary>
        public SortDirection SortDirection { get; set; }
    }
}
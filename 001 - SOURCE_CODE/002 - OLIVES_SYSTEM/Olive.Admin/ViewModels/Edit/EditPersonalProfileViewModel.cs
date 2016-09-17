using Shared.Attributes;
using Shared.Enumerations;
using Shared.Resources;

namespace OliveAdmin.ViewModels.Edit
{
    /// <summary>
    ///     Class which defines structure of account modify.
    /// </summary>
    public class EditPersonalProfileViewModel
    {
        /// <summary>
        /// Account first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Account last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Account password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Available values which can be set to gender.
        /// </summary>
        [InEnumerationsArray(new object[] {Shared.Enumerations.Gender.Male, Shared.Enumerations.Gender.Female},
             ErrorMessageResourceType = typeof(Language), ErrorMessageResourceName = "ValueIsInvalid")]
        public Gender? Gender { get; set; }

        /// <summary>
        /// Phone number of account.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Account birthday.
        /// </summary>
        public double? Birthday { get; set; }
    }
}
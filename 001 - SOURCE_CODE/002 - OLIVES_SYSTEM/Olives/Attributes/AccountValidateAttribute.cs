using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Olives.Interfaces;
using Shared.Enumerations;
using Shared.Interfaces;

namespace Olives.Attributes
{
    public enum AccountValidateInputType
    {
        Email,
        Id
    }

    public class AccountValidateAttribute : ValidationAttribute
    {
        #region Properties

        /// <summary>
        /// Status of account.
        /// </summary>
        public StatusAccount? Status { get; set; }

        /// <summary>
        /// Role of account.
        /// </summary>
        public byte? Role { get; set; }

        /// <summary>
        /// Repository which provides functions to access account database.
        /// </summary>
        private IRepositoryAccountExtended RepositoryAccountExtended => DependencyResolver.Current.GetService<IRepositoryAccountExtended>();

        /// <summary>
        /// Type of object which is input to be checked.
        /// </summary>
        private readonly AccountValidateInputType _inputType;

        /// <summary>
        /// Whether the check is for account available or not.
        /// </summary>
        private readonly bool _isAccountAvailable;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize an instance of class with input parameters.
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="isAccountAvailable"></param>
        public AccountValidateAttribute(AccountValidateInputType inputType, bool isAccountAvailable)
        {
            _inputType = inputType;
            _isAccountAvailable = isAccountAvailable;
        }

        /// <summary>
        /// Initialize an instance of class with input parameters.
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="isAccountAvailable"></param>
        /// <param name="role"></param>
        public AccountValidateAttribute(AccountValidateInputType inputType, bool isAccountAvailable, Role role)
        {
            _inputType = inputType;
            _isAccountAvailable = isAccountAvailable;
            Role = (byte)role;
        }


        /// <summary>
        /// Initialize an instance of class with input parameters.
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="isAccountAvailable"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        public AccountValidateAttribute(AccountValidateInputType inputType, bool isAccountAvailable, Role role, StatusAccount status)
        {
            _inputType = inputType;
            _isAccountAvailable = isAccountAvailable;
            Role = (byte)role;
            Status = status;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check whether the input information is valid or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // No instance has been initialized.
            if (value == null)
                return ValidationResult.Success;

            int? id = null;
            string email = null;

            switch (_inputType)
            {
                case AccountValidateInputType.Id:
                    id = (int)value;
                    break;
                default:
                    email = (string) value;
                    break;
            }

            var account = RepositoryAccountExtended.FindPerson(id, email, null, Role, Status);
            if (!_isAccountAvailable)
            {
                if (account == null)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else
            {
                if (account == null)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        #endregion
    }
}
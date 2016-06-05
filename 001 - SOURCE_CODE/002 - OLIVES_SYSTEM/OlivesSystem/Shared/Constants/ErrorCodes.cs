namespace Shared.Constants
{
    public class ErrorCodes
    {
        /// <summary>
        ///     Email address is invalid.
        ///     [Only yahoo|google|hotmail is allowed]
        /// </summary>
        public const string InvalidEmail = "0x00000001";

        /// <summary>
        ///     Email address is required.
        /// </summary>
        public const string EmailIsRequired = "0x00000002";

        /// <summary>
        ///     Invalid email address length.
        /// </summary>
        public const string InvalidEmailLength = "0x00000003";

        /// <summary>
        ///     Invalid password.
        /// </summary>
        public const string PasswordIsRequired = "0x00000005";

        /// <summary>
        ///     Invalid password length.
        /// </summary>
        public const string InvalidPasswordLength = "0x00000006";

        /// <summary>
        ///     At least 1 Alphabet and 1 Number.
        /// </summary>
        public const string InvalidPasswordFormat = "0x00000007";

        /// <summary>
        ///     First name is required.
        /// </summary>
        public const string FirstNameIsRequired = "0x00000008";

        /// <summary>
        ///     First name maximum length is 32.
        /// </summary>
        public const string InvalidFirstNameLength = "0x00000009";

        /// <summary>
        ///     Last name is required.
        /// </summary>
        public const string LastNameIsRequired = "0x00000010";

        /// <summary>
        ///     Maximum length of last name is 32.
        /// </summary>
        public const string InvalidLastNameLength = "0x00000011";

        /// <summary>
        ///     Invalid gender.
        /// </summary>
        public const string InvalidGender = "0x00000012";

        /// <summary>
        ///     Phone number is required.
        /// </summary>
        public const string PhoneNumberIsRequired = "0x00000013";

        /// <summary>
        ///     Invalid phone number.
        /// </summary>
        public const string InvalidPhoneFormat = "0x00000014";

        /// <summary>
        ///     Specialization is required.
        /// </summary>
        public const string SpecializationIsRequired = "0x00000015";

        /// <summary>
        ///     Invalid specialization length.
        /// </summary>
        public const string InvalidSpecializationLength = "0x00000016";

        /// <summary>
        ///     Specialization areas is required.
        /// </summary>
        public const string SpecializationAreasIsRequired = "0x00000017";

        /// <summary>
        ///     User has already existed.
        /// </summary>
        public const string UserHasAlreadyExisted = "0x00000018";

        /// <summary>
        ///     Doctor id is required.
        /// </summary>
        public const string DoctorIdIsRequired = "0x00000019";

        /// <summary>
        ///     Doctor id is required.
        /// </summary>
        public const string InvalidBirthday = "0x00000020";

        /// <summary>
        ///     Account status is invalid.
        /// </summary>
        public const string InvalidAccountStatus = "0x00000021";

        /// <summary>
        ///     Address is invalid.
        /// </summary>
        public const string InvalidAddress = "0x00000022";

        /// <summary>
        ///     Cannot register doctor because of id or identity card no conflict.
        /// </summary>
        public const string DoctorIdentityConflict = "0x00000023";

        /// <summary>
        ///     Address is required.
        /// </summary>
        public const string AddressIsRequired = "0x00000024";

        /// <summary>
        /// Identity card is required.
        /// </summary>
        public const string IdentityIsRequired = "0x00000025";
        
        /// <summary>
        /// Identity card number can only contains 9 numbers.
        /// </summary>
        public const string InvalidIdentityCardLength = "0x00000026";

        /// <summary>
        /// Record must be from 1 to 20
        /// </summary>
        public const string InvalidPageRecords = "0x00000027";

        /// <summary>
        /// Page index must be from 0 and larger.
        /// </summary>
        public const string InvalidPageIndex = "0x00000028";
    }
}
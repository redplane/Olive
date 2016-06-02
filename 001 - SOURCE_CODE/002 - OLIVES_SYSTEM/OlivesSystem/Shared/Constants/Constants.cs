namespace Shared.Constants
{
    public class Constants
    {
        /// <summary>
        ///     Maximum length of Email address.
        /// </summary>
        public const int EmailMaxLength = 128;

        /// <summary>
        ///     Maximum length of Password.
        /// </summary>
        public const int PasswordMaxLength = 16;

        /// <summary>
        ///     Maximum length of Specialization.
        /// </summary>
        public const int SpecializationMaxLength = 128;

        public const int Male = 0;
        public const int Female = 1;

        public const string RequestHeaderAccountEmail = "account_email";

        public const string RequestHeaderAccountPassword = "account_password";
    }
}
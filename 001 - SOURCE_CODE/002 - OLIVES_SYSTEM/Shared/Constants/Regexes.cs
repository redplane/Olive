namespace Shared.Constants
{
    public class Regexes
    {
        /// <summary>
        ///     Regex for email.
        /// </summary>
        public const string Email = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        /// <summary>
        ///     Regex for email filtering.
        /// </summary>
        public const string EmailFilter = "^[a-zA-Z-_.@]*";

        /// <summary>
        ///     Minimum 8, Maximum 16 characters at least 1 Alphabet and 1 Number
        /// </summary>
        public const string Password = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]*";

        /// <summary>
        ///     Invalid phone number.
        /// </summary>
        public const string Phone = @"^([0-9]){0,15}$";
    }
}
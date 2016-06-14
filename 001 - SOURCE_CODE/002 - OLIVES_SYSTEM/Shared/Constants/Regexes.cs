namespace Shared.Constants
{
    public class Regexes
    {
        /// <summary>
        ///     Regex for email.
        /// </summary>
        public const string Email = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        /// <summary>
        ///     Minimum 8, Maximum 16 characters at least 1 Alphabet and 1 Number
        /// </summary>
        public const string Password = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,16}$";

        /// <summary>
        ///     Invalid phone number.
        /// </summary>
        public const string Phone = @"^([0-9]){0,15}$";

        /// <summary>
        ///     Regex of identity card.
        /// </summary>
        public const string IdentityCard = @"^[0-9]{9}$";

        /// <summary>
        ///     Regular expression of personal id.
        /// </summary>
        public const string PersonalId = "^[?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}?$";

        /// <summary>
        ///     Regular expression of personal note identity.
        /// </summary>
        public const string PersonalNoteIdentity = "^[?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}?$";
    }
}
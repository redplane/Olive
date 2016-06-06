namespace Shared.Constants
{
    public class Regexes
    {
        /// <summary>
        ///     Regex for email.
        /// </summary>
        public const string Email = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        /// <summary>
        ///     Regex of account password.
        /// </summary>
        public const string Password = @"^([a-fA-F0-9]){32}$";

        /// <summary>
        ///     Invalid phone number.
        /// </summary>
        public const string Phone = @"^([0-9]){0,15}$";

        /// <summary>
        /// Regex of identity card.
        /// </summary>
        public const string IdentityCard = @"^([0-9])$";

        /// <summary>
        /// Regular expression of personal id.
        /// </summary>
        public const string PersonalId = "^[?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}?$";
    }
}
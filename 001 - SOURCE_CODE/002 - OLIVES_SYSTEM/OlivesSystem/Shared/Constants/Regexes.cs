namespace Shared.Constants
{
    public class Regexes
    {
        /// <summary>
        ///     Regex for email.
        /// </summary>
        public const string Email = @"/^[^@]+@(yahoo|ymail|rocketmail|gmail|hotmail)\.com$/i";

        /// <summary>
        ///     Regex of account password.
        /// </summary>
        public const string Password = @"/^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]*/g";

        /// <summary>
        ///     Invalid phone number.
        /// </summary>
        public const string Phone = @"/^[0-9 ]*/g";
    }
}
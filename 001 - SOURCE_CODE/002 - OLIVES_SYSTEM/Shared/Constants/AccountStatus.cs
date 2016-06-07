namespace Shared.Constants
{
    public class AccountStatus
    {
        /// <summary>
        ///     Account is inactive (banned | disabled)
        /// </summary>
        public const byte Inactive = 0;

        /// <summary>
        ///     Account is waiting for confirmation from admins.
        /// </summary>
        public const byte Pending = 1;

        /// <summary>
        ///     Account is active.
        /// </summary>
        public const byte Active = 2;
    }
}
namespace Shared.Interfaces
{
    public interface IDbSetting
    {
        /// <summary>
        ///     Url of database.
        /// </summary>
        string Url { get; set; }

        /// <summary>
        ///     Username which is provided to access database.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        ///     Password which is provided to access database.
        /// </summary>
        string Password { get; set; }
    }
}
namespace Shared.Interfaces
{
    public interface IRepositoryAccount
    {
        /// <summary>
        ///     Find the md5 password from the original one.
        /// </summary>
        /// <param name="originalPassword"></param>
        /// <returns></returns>
        string FindMd5Password(string originalPassword);
    }
}
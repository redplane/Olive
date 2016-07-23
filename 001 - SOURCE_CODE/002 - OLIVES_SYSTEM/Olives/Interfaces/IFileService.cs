namespace Olives.Interfaces
{
    public interface IFileService
    {
        /// <summary>
        ///     Encode a file to base64 format string.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        string EncodeFileBase64(string fullPath);
    }
}
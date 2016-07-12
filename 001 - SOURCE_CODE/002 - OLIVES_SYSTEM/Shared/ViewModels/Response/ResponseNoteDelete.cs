namespace Shared.ViewModels.Response
{
    public class ResponseNoteDelete
    {
        /// <summary>
        ///     Number of note has been removed.
        /// </summary>
        public int Notes { get; set; }

        /// <summary>
        ///     Number of key-value pairs have been removed.
        /// </summary>
        public int Pairs { get; set; }
    }
}
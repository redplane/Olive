namespace Shared.Models.Nodes
{
    public class Transaction
    {
        /// <summary>
        ///     Transaction GUID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Message of transaction.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     Time when transaction is created.
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        ///     Cost.
        /// </summary>
        public double Cost { get; set; }
    }
}
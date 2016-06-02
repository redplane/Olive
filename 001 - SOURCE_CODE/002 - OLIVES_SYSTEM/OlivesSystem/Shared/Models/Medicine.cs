namespace Shared.Models
{
    public class Medicine
    {
        /// <summary>
        ///     Medicine name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Total quantity patient needs to
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        ///     Medicine unit.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        ///     Note.
        /// </summary>
        public string Note { get; set; }
    }
}
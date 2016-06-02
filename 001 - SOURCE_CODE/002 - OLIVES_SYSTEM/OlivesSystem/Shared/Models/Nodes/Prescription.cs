namespace Shared.Models.Nodes
{
    public class Prescription
    {
        /// <summary>
        ///     Prescription GUID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Name of prescription.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Valid time.
        /// </summary>
        public long From { get; set; }

        /// <summary>
        ///     Time before which prescription had been valid.
        /// </summary>
        public long To { get; set; }

        /// <summary>
        ///     Medicines patient needs to take.
        /// </summary>
        public Medicine[] Medicines { get; set; }
    }
}
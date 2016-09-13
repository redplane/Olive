namespace Shared.Models.Vertexes
{
    public class HeartRate
    {
        /// <summary>
        /// Rate of heart at the time measurement was done.
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// When the measurement was done.
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Note of measurement.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Time when the record was submitted to server.
        /// </summary>
        public double Created { get; set; }

        /// <summary>
        /// Time when the record was lastly modified.
        /// </summary>
        public double? LastModified { get; set; }
    }
}
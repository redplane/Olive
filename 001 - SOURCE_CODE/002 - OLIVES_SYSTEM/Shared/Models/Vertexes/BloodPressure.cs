namespace Shared.Models.Vertexes
{
    public class BloodPressure
    {
        /// <summary>
        /// Minimum pressure of blood.
        /// </summary>
        public double Diastolic { get; set; }

        /// <summary>
        /// Maximum pressure of blood.
        /// </summary>
        public double Systolic { get; set; }
    
        /// <summary>
        /// Time when the measurement was done.
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Note of measurement.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// When the measurement was created.
        /// </summary>
        public double Created { get; set; }

        /// <summary>
        /// When the measurement was lastly modified.
        /// </summary>
        public double? LastModified { get; set; }
    }
}
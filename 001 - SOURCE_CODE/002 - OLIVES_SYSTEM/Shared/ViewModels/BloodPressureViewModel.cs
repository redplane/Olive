namespace Shared.ViewModels
{
    public class BloodPressureViewModel
    {
        /// <summary>
        /// Id of record.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Owner of record.
        /// </summary>
        public  int Owner { get; set; }

        /// <summary>
        /// Minimun value of pressure.
        /// </summary>
        public double Diastolic { get; set; }

        /// <summary>
        /// Maximum value of pressure.
        /// </summary>
        public double Systolic { get; set; }

        /// <summary>
        /// Time when measurement was done.
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Note of measurement.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Time when record was created.
        /// </summary>
        public double Created { get; set; }

        /// <summary>
        /// Time when record was lastly modified.
        /// </summary>
        public double? LastModified { get; set; }
    }
}
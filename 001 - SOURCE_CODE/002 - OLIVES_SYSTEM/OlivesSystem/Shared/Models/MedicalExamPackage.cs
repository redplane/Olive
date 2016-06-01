namespace Shared.Models
{
    public class MedicalExamPackage
    {
        /// <summary>
        /// Package GUID.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Available duration of package.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Cost of this package.
        /// </summary>
        public double Cost { get; set; }
    }
}
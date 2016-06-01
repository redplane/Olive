namespace Shared.Models.Nodes
{
    public class MedicalRecord
    {
        /// <summary>
        /// Id of medical record.
        /// Mã bệnh án.
        /// </summary>
        public string Id { get; set; } 

        /// <summary>
        /// Medical record summary.
        /// Tổng quan bệnh án.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Examination which is needed doing.
        /// Các xét nghiệm cần làm.
        /// </summary>
        public string [] Tests { get; set; }

        /// <summary>
        /// Diagnoses.
        /// Các chẩn đoán.
        /// </summary>
        public string [] Diagnoses { get; set; }

        /// <summary>
        /// Morbidities which goes with the current one.
        /// Các bệnh liên quan.
        /// </summary>
        public string [] AdditionalMorbidities { get; set; }

        /// <summary>
        /// Differential diagnosis which support to the main diagnose.
        /// Chẩn đoán phân biệt.
        /// </summary>
        public string [] DifferentialDiagnosis { get; set; }

        /// <summary>
        /// Other pathologies which can affect to the the current diagnose.
        /// </summary>
        public string [] OtherPathologies { get; set; }

        /// <summary>
        /// GUID of images which about the current medical record.
        /// </summary>
        public string [] Images { get; set; }
    }
}
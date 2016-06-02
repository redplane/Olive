namespace Shared.Models.Nodes
{
    public class InternalMedicine : MedicalRecord
    {
        /// <summary>
        ///     Note about circulatory system.
        ///     Hệ tuần hoàn.
        /// </summary>
        public string[] Circulatory { get; set; }

        /// <summary>
        ///     Note about respiratory system.
        ///     Hệ hô hấp.
        /// </summary>
        public string[] Respirative { get; set; }

        /// <summary>
        ///     Digestive system.
        /// </summary>
        public string[] Digestion { get; set; }

        /// <summary>
        ///     Excretory system.
        ///     Hệ bài tiết (Thận - tiết nhiệu - sinh dục)
        /// </summary>
        public string[] Excretory { get; set; }

        /// <summary>
        ///     Neural system.
        ///     Hệ thần kinh.
        /// </summary>
        public string[] Nerve { get; set; }

        /// <summary>
        ///     Related to bone and muscle.
        ///     Hệ xương khớp.
        /// </summary>
        public string[] Musculoskeletal { get; set; }

        /// <summary>
        ///     Related to teeth, ear, ...
        ///     Răng - hàm - mặt
        /// </summary>
        public string[] Dentomaxillofacial { get; set; }

        /// <summary>
        ///     Eye.
        ///     Mắt.
        /// </summary>
        public string[] Optic { get; set; }
    }
}
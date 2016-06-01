namespace Shared.Models.Nodes
{
    public class PersonalNote
    {
        /// <summary>
        ///     Time when note was created.
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        ///     Temperature of patient when this note was created.
        /// </summary>
        public uint Temperature { get; set; }

        /// <summary>
        ///     Pulse of patient when this note was created.
        /// </summary>
        public uint Pulse { get; set; }

        /// <summary>
        ///     Minimum value of blood pressure of patient when this note was created.
        /// </summary>
        public uint BloodPressureMin { get; set; }

        /// <summary>
        ///     Maximum value of blood pressure of patient when this note was created.
        /// </summary>
        public uint BloodPressureMax { get; set; }
    }
}
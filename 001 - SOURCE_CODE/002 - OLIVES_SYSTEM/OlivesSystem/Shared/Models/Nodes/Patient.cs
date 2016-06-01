namespace Shared.Models.Nodes
{
    public class Patient : Person
    {
        /// <summary>
        /// Height of person.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Weight of person.
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// What causes person is allergy with.
        /// </summary>
        public Allergy [] Allergies { get; set; }

        /// <summary>
        /// What causes person is addictive with.
        /// </summary>
        public Addiction [] Addictions { get; set; }

        /// <summary>
        /// Family anamneses.
        /// </summary>
        public string [] Anamneses { get; set; }

        /// <summary>
        /// Personal health note of patients.
        /// This is for daily , weekly and monthly statistic.
        /// </summary>
        public PersonalNote[] HealthNotes { get; set; }
    }
}
namespace Shared.Models.Nodes
{
    public class Patient : Person
    {
        /// <summary>
        ///     Height of person.
        /// </summary>
        public float? Height { get; set; }

        /// <summary>
        ///     Weight of person.
        /// </summary>
        public float? Weight { get; set; }
        
        /// <summary>
        ///     Family anamneses.
        /// </summary>
        public string[] Anamneses { get; set; }
    }
}
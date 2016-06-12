namespace Shared.Models.Nodes
{
    public class Addiction
    {
        /// <summary>
        /// Which agent causes patient be addictive with.
        /// </summary>
        public string Cause { get; set; }
        
        /// <summary>
        /// Note of addiction.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Time when note was created.
        /// </summary>
        public long Created { get; set; }

        /// <summary>
        /// Time when note was lastly modified.
        /// </summary>
        public long LastModified { get; set; }
    }
}
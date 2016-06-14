namespace Shared.Models.Nodes
{
    public class Allergy
    {
        /// <summary>
        ///     Which agent causes patient be allergic with.
        /// </summary>
        public string Cause { get; set; }

        /// <summary>
        ///     Note content.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        ///     Date when allergy was created.
        /// </summary>
        public long Created { get; set; }

        /// <summary>
        ///     Date when note was lastly modified.
        /// </summary>
        public long LastModified { get; set; }
    }
}
using Shared.Models;

namespace Shared.ViewModels
{
    public class RelativeViewModel
    {
        /// <summary>
        ///     Doctor who is related to requester.
        /// </summary>
        public Person Relative { get; set; }

        /// <summary>
        ///     Status of relationship between the requester and doctor.
        /// </summary>
        public byte RelationshipStatus { get; set; }

        /// <summary>
        ///     Time when relationship was created.
        /// </summary>
        public double Created { get; set; }
    }
}
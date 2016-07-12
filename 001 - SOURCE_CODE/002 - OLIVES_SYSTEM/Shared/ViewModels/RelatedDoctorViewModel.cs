using Shared.Models;

namespace Shared.ViewModels
{
    public class RelatedDoctorViewModel
    {
        /// <summary>
        ///     Doctor who is related to requester.
        /// </summary>
        public Doctor Doctor { get; set; }

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
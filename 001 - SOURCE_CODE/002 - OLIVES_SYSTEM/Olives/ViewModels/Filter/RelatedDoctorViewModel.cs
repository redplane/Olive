using Shared.Models;

namespace Olives.ViewModels.Filter
{
    public class RelatedDoctorViewModel
    {
        /// <summary>
        /// Id of relationship
        /// </summary>
        public int Relation { get; set; }

        /// <summary>
        ///     Doctor who is related to requester.
        /// </summary>
        public Doctor Doctor { get; set; }
        
        /// <summary>
        ///     Time when relationship was created.
        /// </summary>
        public double Created { get; set; }
    }
}
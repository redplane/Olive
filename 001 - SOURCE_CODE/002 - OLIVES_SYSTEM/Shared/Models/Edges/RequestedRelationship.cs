
namespace Shared.Models.Edges
{
    public class RequestedRelationship
    {
        /// <summary>
        /// Message of relationship request.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// When the request was created.
        /// </summary>
        public double Created { get; set; }
    }
}
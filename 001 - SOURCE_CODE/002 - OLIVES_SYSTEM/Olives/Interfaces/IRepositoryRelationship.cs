using System.Threading.Tasks;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Response;
using Shared.Models;

namespace Olives.Interfaces
{
    public interface IRepositoryRelationship
    {
        /// <summary>
        ///     Find the relation between 2 people.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<Relation> FindRelationshipAsync(FilterRelationshipViewModel filter);

        /// <summary>
        ///     Delete a relation asynchronously.
        /// </summary>
        /// <param name="filter">Id of relationship</param>
        /// <returns></returns>
        Task<int> DeleteRelationshipAsync(FilterRelationshipViewModel filter);

        /// <summary>
        ///     Filter relationship base on the role of requester.
        /// </summary>
        /// <param name="filter"></param>
        Task<ResponseRelationshipFilter> FilterRelationshipAsync(FilterRelationshipViewModel filter);

        /// <summary>
        ///     Filter related doctors.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        Task<ResponseRelatedDoctorFilter> FilterRelatedDoctorAsync(int requester, int page,
            int? records);

        /// <summary>
        ///     This function is to check whether 2 people are connected to each other or not.
        /// </summary>
        /// <param name="firstPerson"></param>
        /// <param name="secondPerson"></param>
        /// <returns></returns>
        Task<bool> IsPeopleConnected(int firstPerson, int secondPerson);
    }
}
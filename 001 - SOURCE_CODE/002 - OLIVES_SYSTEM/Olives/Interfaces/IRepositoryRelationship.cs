using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;
using FilterRelationshipViewModel = Olives.ViewModels.Filter.FilterRelationshipViewModel;

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
        ///     Find a relation whose id match with search condition and person is taking part in it.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="person"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<IList<Relation>> FindRelationParticipation(int id, int person, byte? status);

        /// <summary>
        ///     Delete a relation asynchronously.
        /// </summary>
        /// <param name="filter">Id of relationship</param>
        /// <returns></returns>
        Task<int> DeleteRelationAsync(FilterRelationshipViewModel filter);

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
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Models;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryRelation
    {
        /// <summary>
        ///     Find the relation between 2 people.
        /// </summary>
        /// <param name="firstPerson"></param>
        /// <param name="secondPerson"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<IList<Relation>> FindRelationshipAsync(int firstPerson, int secondPerson, byte? status);

        /// <summary>
        ///     Find a relation by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="person"></param>
        /// <param name="role">Whether person is the source (0) or target (1) of relationship</param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Relation> FindRelationshipAsync(int id, int? person, RoleRelationship? role, StatusRelation? status);

        /// <summary>
        ///     Initialize a relationship to database.
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        Task<Relation> InitializeRelationAsync(Relation relation);

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
        /// <param name="id">Id of relationship</param>
        /// <param name="requester">Id of person who request to delete relationship.</param>
        /// <param name="role">The participation of requester in relationship.</param>
        /// <param name="status">Status of relationship.</param>
        /// <returns></returns>
        Task<int> DeleteRelationAsync(int id, int? requester, RoleRelationship? role, StatusRelation? status);

        /// <summary>
        ///     Filter relationship base on the role of requester.
        /// </summary>
        /// <param name="filter"></param>
        Task<ResponseRelationshipFilter> FilterRelationshipAsync(FilterRelationshipViewModel filter);

        /// <summary>
        ///     Filter related doctors.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        Task<ResponseRelatedDoctorFilter> FilterRelatedDoctorAsync(int requester, StatusRelation? status, int page,
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
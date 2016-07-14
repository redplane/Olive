using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryAccount
    {
        #region Shared

        /// <summary>
        ///     Find person by using specific information asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<IList<Person>> LoginAsync(LoginViewModel info);

        /// <summary>
        ///     Find person by using specific information synchronously.
        /// </summary>
        /// <param name="id">Id of person</param>
        /// <param name="email">Email of person</param>
        /// <param name="password">Password of person</param>
        /// <param name="role"></param>
        /// <returns></returns>
        Person FindPerson(int? id, string email, string password, byte? role);

        /// <summary>
        ///     Find person by using specific information asynchronously.
        /// </summary>
        /// <param name="id">Id of person</param>
        /// <param name="email">Email of person</param>
        /// <param name="password">Password of person</param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Person> FindPersonAsync(int? id, string email, string password, byte? role, StatusAccount? status);

        /// <summary>
        ///     Find a person asynchronously by using activation code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<Person> FindPersonAsync(string code);

        /// <summary>
        ///     Edit person status asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Person> EditPersonStatusAsync(int id, byte status);

        /// <summary>
        ///     Summary person by using role.
        /// </summary>
        /// <returns></returns>
        Task<IList<StatusSummaryViewModel>> SummarizePersonRoleAsync(byte? role);

        /// <summary>
        ///     Initialize or update person information asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Person> InitializePersonAsync(Person info);

        /// <summary>
        ///     Edit a person by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Person> EditPersonProfileAsync(int id, Person info);

        #endregion

        #region Doctor

        /// <summary>
        ///     Filter doctor by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Doctor> FindDoctorAsync(int id, StatusAccount? status);

        /// <summary>
        ///     Filter doctor by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseDoctorFilter> FilterDoctorAsync(FilterDoctorViewModel filter);

        #endregion

        #region Patient

        /// <summary>
        ///     Find the patient by using id and perhaps status asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Patient> FindPatientAsync(int id, byte? status);
        
        /// <summary>
        ///     Filter doctor by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponsePatientFilter> FilterPatientAsync(FilterPatientViewModel filter);

        /// <summary>
        ///     Activate patient's account by search person id.
        /// </summary>
        /// <param name="code"></param>
        Task<bool> InitializePatientActivation(string code);

        #endregion

        #region Relation

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
        /// <param name="requester"></param>
        /// <param name="partner"></param>
        /// <param name="role"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        Task<ResponseRelationshipFilter> FilterRelationshipAsync(int requester, int? partner, RoleRelationship? role,
            TypeRelation? type, StatusRelation? status, int page, int records);

        /// <summary>
        ///     Filter related doctors.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        Task<ResponseRelatedDoctorFilter> FilterRelatedDoctorAsync(int requester, StatusRelation? status, int page,
            int records);

        /// <summary>
        ///     Filter related doctors.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        Task<ResponseRelativeFilter> FilterRelativeAsync(int requester, StatusRelation? status, int page,
            int records);

        #endregion
    }
}
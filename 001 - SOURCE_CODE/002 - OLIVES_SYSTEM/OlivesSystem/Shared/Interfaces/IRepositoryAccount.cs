using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models.Nodes;
using Shared.ViewModels;

namespace Shared.Interfaces
{
    public interface IRepositoryAccount
    {
        Task<IPerson> LoginAsync(LoginViewModel info);
        
        /// <summary>
        /// Base on personal identity or identity card, check whether doctor can be registered or not.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identityCardNo"></param>
        /// <returns></returns>
        Task<bool> IsDoctorAbleToRegister(string id, string identityCardNo);
        
        /// <summary>
        ///     Check whether person with specific information exists or not.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailCaseSensitive"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<IPerson> GetPersonExistAsync(string email, bool emailCaseSensitive, string password, byte? role);

        /// <summary>
        ///     Filter person by using specific conditions.
        /// </summary>
        /// <param name="filter">Specific conditions.</param>
        /// <returns></returns>
        Task<object> FilterPersonAsync(FilterPersonViewModel filter);

        /// <summary>
        ///     Filter doctors by using specific conditions.
        /// </summary>
        /// <param name="filter">Specific conditions</param>
        /// <returns></returns>
        Task<IList<Doctor>> FilterDoctorAsync(FilterDoctorViewModel filter);
        
        /// <summary>
        ///     Retrieve personal information by using specific conditions.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailCaseSensitive"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        IPerson GetPersonExist(string email, bool emailCaseSensitive = false,
            string password = "", byte? role = null);

        /// <summary>
        ///     Check whether doctor has been registered or not.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<Doctor> FindDoctor(FilterDoctorViewModel filter);

        /// <summary>
        ///     Check whether doctor has been registered or not.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identityCardId"></param>
        /// <returns></returns>
        Task<IList<Doctor>> FindDoctor(string id, string identityCardId);

        #region Patient

        /// <summary>
        /// Find patient by using specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IList<Patient>> FindPatientById(string id);

        #endregion

        #region Doctor

        #endregion
        
        #region Shared

        /// <summary>
        ///     Initialize a person in database.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool InitializePerson(IPerson info);

        /// <summary>
        ///     Update person information by using id for search.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<object> EditPersonAsync(string id, IPerson info);

        /// <summary>
        /// Using id and email to check whether person can be created or not.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> IsPatientAbleToCreated(string id, string email);

        /// <summary>
        /// Change account status base on account id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<bool> ModifyAccountStatus(string id, byte status);
        
        #endregion
    }
}
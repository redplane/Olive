using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Response;

namespace Shared.Interfaces
{
    public interface IRepositoryAllergy
    {
        /// <summary>
        /// Filter allergies with specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<ResponseAllergyFilter> FilterAllergy(AllergyGetViewModel filter);

        /// <summary>
        /// Initialize an allergy with given information.
        /// </summary>
        /// <param name="info"></param>
        Task<Allergy> InitializeAllergyAsync(Allergy info);

        /// <summary>
        /// Find allergy by using id and owner id.
        /// </summary>
        /// <param name="account">Account name</param>
        /// <param name="password">Account password</param>
        /// <param name="id">Id of allergy</param>
        /// <returns></returns>
        Task<IList<Allergy>> FindAllergyAsync(string account, string password, int id);

        /// <summary>
        /// Delete an allergy synchronously.
        /// </summary>
        /// <param name="allergy"></param>
        /// <returns></returns>
        void DeleteAllergy(Allergy allergy);
    }
}
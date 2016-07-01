using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;

namespace Shared.Interfaces
{
    public interface IRepositoryPlace
    {

        #region Country

        /// <summary>
        /// Find list of countries by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IList<Country>> FindCountry(int id);

        #endregion

        #region City

        /// <summary>
        /// Find list of cities by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IList<City>> FindCity(int id);

        #endregion

    }
}
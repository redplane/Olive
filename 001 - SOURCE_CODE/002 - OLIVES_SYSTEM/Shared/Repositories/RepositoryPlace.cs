using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Shared.Interfaces;
using Shared.Models;

namespace Shared.Repositories
{
    public class RepositoryPlace : IRepositoryPlace
    {
        #region Country

        /// <summary>
        /// Find countries by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IList<Country>> FindCountry(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the countries using id.
            var results = context.Countries.Where(x => x.Id == id);

            return await results.ToListAsync();
        }

        #endregion

        #region City

        /// <summary>
        /// Find a list of cities by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IList<City>> FindCity(int id)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the countries using id.
            var results = context.Cities.Where(x => x.Id == id);

            return await results.ToListAsync();
        }

        #endregion
    }
}
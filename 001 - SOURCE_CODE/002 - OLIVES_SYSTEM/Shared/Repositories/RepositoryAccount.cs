using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;

namespace Shared.Repositories
{
    public class RepositoryAccount : IRepositoryAccount
    {
        #region Properties

        /// <summary>
        ///     Context which provides functions to access database.
        /// </summary>
        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructor

        public RepositoryAccount(IOliveDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion
        
        #region Methods

        /// <summary>
        ///     Find person by using email, password and role.
        /// </summary>
        /// <param name="id">Id of person</param>
        /// <param name="email">Email which is used for filtering.</param>
        /// <param name="password">Password of account.</param>
        /// <param name="role">As role is specified. Find account by role.</param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Person FindPerson(int? id, string email, string password, byte? role, StatusAccount? status)
        {
            var context = _dataContext.Context;

            // By default, take all people in database.
            IQueryable<Person> result = context.People;

            // Id is specified.
            if (id != null)
                result = result.Where(x => x.Id == id);

            // Email is specified.
            if (!string.IsNullOrEmpty(email))
                result = result.Where(x => x.Email.Equals(email));

            // Password is specified.
            if (!string.IsNullOrEmpty(password))
                result = result.Where(x => x.Password.Equals(password));

            // Role is specified.
            if (role != null)
                result = result.Where(x => x.Role == role);

            if (status != null)
                result = result.Where(x => x.Status == (byte) status.Value);

            return result.FirstOrDefault();
        }
        
        /// <summary>
        ///     Find person by using email, password and role.
        /// </summary>
        /// <param name="id">Id of person</param>
        /// <param name="email">Email which is used for filtering.</param>
        /// <param name="password">Password of account.</param>
        /// <param name="role">As role is specified. Find account by role.</param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<Person> FindPersonAsync(int? id, string email, string password, byte? role,
            StatusAccount? status)
        {
            var context = _dataContext.Context;

            // By default, take all people in database.
            IQueryable<Person> result = context.People;

            // Id is specified.
            if (id != null)
                result = result.Where(x => x.Id == id);

            // Email is specified.
            if (!string.IsNullOrEmpty(email))
                result = result.Where(x => x.Email.Equals(email));

            // Password is specified.
            if (!string.IsNullOrEmpty(password))
                result = result.Where(x => x.Password.Equals(password));

            // Role is specified.
            if (role != null)
                result = result.Where(x => x.Role == role);

            // Status has been specified.
            if (status != null)
            {
                var castedStatus = (byte) status;
                result = result.Where(x => x.Status == castedStatus);
            }

            return await result.FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Initialize or update person information asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Person> InitializePersonAsync(Person info)
        {
            var context = _dataContext.Context;

            // Add or update information base on the primary key.
            context.People.AddOrUpdate(info);

            // Save change to database.
            await context.SaveChangesAsync();

            return info;
        }

        /// <summary>
        /// Find the md5 hashed password from the original one.
        /// </summary>
        /// <param name="originalPassword"></param>
        /// <returns></returns>
        public string FindMd5Password(string originalPassword)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                // Find the byte stream from the original password string.
                var originalPasswordBytes = Encoding.ASCII.GetBytes(originalPassword);
                
                // Find the hashed byte stream.
                var originalPasswordHashedBytes = md5.ComputeHash(originalPasswordBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                foreach (byte t in originalPasswordHashedBytes)
                    sb.Append(t.ToString("X2"));

                return sb.ToString();
            }
        }

        #endregion
    }
}
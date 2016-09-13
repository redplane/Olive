using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models.Vertexes;
using Shared.ViewModels.Filter;

namespace Shared.Repositories
{
    public class RepositoryAccount : IRepositoryAccount
    {
        #region Properties

        /// <summary>
        ///     Context which provides functions to access database.
        /// </summary>
        private readonly ArangoDatabase _arangoClient;

        #endregion

        #region Constructor

        public RepositoryAccount(ArangoDatabase arangoClient)
        {
            _arangoClient = arangoClient;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find the md5 hashed password from the original one.
        /// </summary>
        /// <param name="originalPassword"></param>
        /// <returns></returns>
        public string FindMd5Password(string originalPassword)
        {
            // Use input string to calculate MD5 hash
            using (var md5 = MD5.Create())
            {
                // Find the byte stream from the original password string.
                var originalPasswordBytes = Encoding.ASCII.GetBytes(originalPassword);

                // Find the hashed byte stream.
                var originalPasswordHashedBytes = md5.ComputeHash(originalPasswordBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                foreach (var t in originalPasswordHashedBytes)
                    sb.Append(t.ToString("X2"));

                return sb.ToString();
            }
        }

        #endregion
    }
}
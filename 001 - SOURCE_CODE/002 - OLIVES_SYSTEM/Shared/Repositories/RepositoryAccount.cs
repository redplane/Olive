using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models.Vertexes;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;
using Shared.ViewModels.Response.Filter;
using QueryableExtensions = System.Data.Entity.QueryableExtensions;

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
        public string FindEncryptedPassword(string originalPassword)
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

        /// <summary>
        /// Initialize account asynchronously.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<Account> InitializeAccountAsync(Account account)
        {
            // Do upsert function.
            var accounts = await _arangoClient.Query()
                .Upsert(aql => new {_key = account.Email}, 
                aql => account, (_, x) => account)
                .In<Account>()
                .ToListAsync();

            // Return null as no account has been modified.
            if (accounts == null || accounts.Count < 1)
                return null;

            return accounts[0];
        }
        
        /// <summary>
        ///     Find the first admin account which matches with the specific conditions synchronously.
        /// </summary>
        /// <param name="filterAccountViewModel"></param>
        /// <returns></returns>
        public Account FindAccount(FilterAccountViewModel filterAccountViewModel)
        {
            // List of admins in database.
            IQueryable<Account> accounts = _arangoClient.Query<Account>();

            // Filter admin account by using specific conditions.
            accounts = FilterAccounts(accounts, filterAccountViewModel);

            // Find the first result.
            return accounts
                .Select(x => x)
                .FirstOrDefault();
        }

        /// <summary>
        ///     Find the first admin account which matches with the specific conditions asynchronously.
        /// </summary>
        /// <param name="filterAdminViewModel"></param>
        /// <returns></returns>
        public async Task<Account> FindAccountAsync(FilterAccountViewModel filterAdminViewModel)
        {
            // List of admins in database.
            IQueryable<Account> accounts = _arangoClient.Query<Account>();

            // Filter admin account by using specific conditions.
            accounts = FilterAccounts(accounts, filterAdminViewModel);

            // Find the first result.
            return await accounts
                .Select(x => x)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Filter patients by using specific conditions.
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="filterAccountViewModel"></param>
        /// <returns></returns>
        private IQueryable<Account> FilterAccounts(IQueryable<Account> accounts,
            FilterAccountViewModel filterAccountViewModel)
        {
            // Email is specified.
            if (!string.IsNullOrWhiteSpace(filterAccountViewModel.Email))
                switch (filterAccountViewModel.EmailComparision)
                {
                    case TextComparision.Contain:
                        accounts = accounts.Where(x => AQL.Contains(x.Email, filterAccountViewModel.Email));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        accounts = accounts.Where(x => AQL.Upper(x.Email) == filterAccountViewModel.Email.ToUpper());
                        break;
                    default:
                        accounts = accounts.Where(x => x.Email == filterAccountViewModel.Email);
                        break;
                }
            
            // First name is specified.
            if (!string.IsNullOrWhiteSpace(filterAccountViewModel.FirstName))
                switch (filterAccountViewModel.FirstNameComparision)
                {
                    case TextComparision.Contain:
                        accounts = accounts.Where(x => AQL.Contains(x.FirstName, filterAccountViewModel.FirstName));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        accounts =
                            accounts.Where(x => AQL.Upper(x.FirstName) == filterAccountViewModel.FirstName.ToUpper());
                        break;
                    default:
                        accounts = accounts.Where(x => x.FirstName == filterAccountViewModel.FirstName);
                        break;
                }

            // Last name is specified.
            if (!string.IsNullOrWhiteSpace(filterAccountViewModel.LastName))
            {
                switch (filterAccountViewModel.LastNameComparision)
                {
                    case TextComparision.Contain:
                        accounts = accounts.Where(x => AQL.Contains(x.LastName, filterAccountViewModel.LastName));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        accounts =
                            accounts.Where(x => AQL.Upper(x.LastName) == filterAccountViewModel.LastName.ToUpper());
                        break;
                    default:
                        accounts = accounts.Where(x => x.LastName == filterAccountViewModel.LastName);
                        break;
                }
            }

            // Password is specified.
            if (!string.IsNullOrWhiteSpace(filterAccountViewModel.Password))
            {
                switch (filterAccountViewModel.PasswordComparision)
                {
                    case TextComparision.Contain:
                        accounts = accounts.Where(x => AQL.Contains(x.Password, filterAccountViewModel.Password));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        accounts =
                            accounts.Where(x => AQL.Upper(x.Password) == filterAccountViewModel.Password.ToUpper());
                        break;
                    default:
                        accounts = accounts.Where(x => x.Password == filterAccountViewModel.Password);
                        break;
                }
            }
            
            // Birthday range is specified.
            if (filterAccountViewModel.MinBirthday != null)
                accounts = accounts.Where(x => x.Birthday >= filterAccountViewModel.MinBirthday.Value);

            if (filterAccountViewModel.MaxBirthday != null)
                accounts = accounts.Where(x => x.Birthday <= filterAccountViewModel.MaxBirthday.Value);

            // Phone is specified.
            if (!string.IsNullOrWhiteSpace(filterAccountViewModel.Phone))
                switch (filterAccountViewModel.PhoneComparision)
                {
                    case TextComparision.Contain:
                        accounts = accounts.Where(x => AQL.Contains(x.Phone, filterAccountViewModel.Phone));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        accounts = accounts.Where(x => AQL.Upper(x.Phone) == filterAccountViewModel.Phone.ToUpper());
                        break;
                    default:
                        accounts = accounts.Where(x => x.Phone == filterAccountViewModel.Phone);
                        break;
                }

            // Genders are specified.
            if ((filterAccountViewModel.Genders != null) && (filterAccountViewModel.Genders.Length > 0))
                accounts = accounts.Where(x => AQL.In(x.Gender, filterAccountViewModel.Genders));

            // Created is specified.
            if (filterAccountViewModel.MinCreated != null)
                accounts = accounts.Where(x => x.Created >= filterAccountViewModel.MinCreated.Value);

            if (filterAccountViewModel.MaxCreated != null)
                accounts = accounts.Where(x => x.Created <= filterAccountViewModel.MaxCreated.Value);

            // Last modified is specified.
            if (filterAccountViewModel.MinLastModified != null)
                accounts =
                    accounts.Where(
                        x => (x.LastModified != null) && (x.LastModified >= filterAccountViewModel.MinLastModified));

            if (filterAccountViewModel.MaxLastModified != null)
                accounts =
                    accounts.Where(
                        x => (x.LastModified != null) && (x.LastModified <= filterAccountViewModel.MinLastModified));

            return accounts;
        }

        /// <summary>
        ///     Filter accounts by using specific conditions with pagination.
        /// </summary>
        /// <param name="filterAccountViewModel"></param>
        /// <returns></returns>
        public async Task<ResponseAccountFilter> FilterAccountsAsync(FilterAccountViewModel filterAccountViewModel)
        {
            // Take all accounts.
            IQueryable<Account> accounts = _arangoClient.Query<Account>();
            accounts = FilterAccounts(accounts, filterAccountViewModel);

            // Response initialization.
            var responseAccountFilter = new ResponseAccountFilter();
            responseAccountFilter.Total = await QueryableExtensions.CountAsync(accounts);

            // Do pagination.
            if (filterAccountViewModel.Records != null)
            {
                responseAccountFilter.Accounts =
                    await accounts.Skip(filterAccountViewModel.Page * filterAccountViewModel.Records.Value)
                        .Take(filterAccountViewModel.Records.Value)
                        .ToListAsync();

                return responseAccountFilter;
            }

            responseAccountFilter.Accounts = await accounts.ToListAsync();
            return responseAccountFilter;
        }

        #endregion
    }
}
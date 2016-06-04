using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;
using Shared.Constants;
using Shared.Interfaces;
using Shared.Models.Nodes;
using Shared.ViewModels;

namespace DotnetSignalR.Repository
{
    public class RepositoryAccount : IRepositoryAccount
    {
        /// <summary>
        ///     Instance which connects to neo4j database.
        /// </summary>
        private readonly GraphClient _graphClient;

        public RepositoryAccount()
        {
        }

        /// <summary>
        ///     Initialize an instance of RepositoryAccount class.
        /// </summary>
        /// <param name="graphClient"></param>
        public RepositoryAccount(GraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        /// <summary>
        ///     Create person asynchronously with given parameter.
        /// </summary>
        /// <param name="info"></param>
        public async Task<bool> CreatePersonAsync(IPerson info)
        {
            await _graphClient.Cypher
                .Create("(n:Person {info})")
                .WithParam("info", info)
                .ExecuteWithoutResultsAsync();

            return true;
        }

        /// <summary>
        ///     Check whether person exists or not.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailCaseSensitive">Whether email should be found case insensitively.</param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<IPerson> GetPersonExistAsync(string email, bool emailCaseSensitive = false,
            string password = "", byte? role = null)
        {
            var exactEmail = email;
            if (!emailCaseSensitive)
                exactEmail = "~(?i)" + email;

            var query = _graphClient.Cypher
                .Match("(n:Person)")
                .Where($"n.Email = '{exactEmail}'");

            if (!string.IsNullOrEmpty(password))
                query = query.AndWhere<IPerson>(n => n.Password == password);

            if (role != null)
                query = query.AndWhere<IPerson>(n => n.Role == role);

            // Retrieve query result asynchronously.
            var queryResult = await query.Return(n => n.As<Person>()).Limit(1).ResultsAsync;

            // No result has been retrieved.
            if (queryResult == null)
                return null;

            // Retrieve the first queried result.
            var person = queryResult.FirstOrDefault();

            if (person == null)
                return null;

            return person;
        }

        /// <summary>
        ///     Log user in
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IPerson> LoginAsync(LoginViewModel info)
        {
            try
            {
                // Query construction.
                var query = await _graphClient.Cypher.Match("(admin:Person)")
                    .Where<IPerson>(admin => admin.Email == info.Email)
                    .AndWhere<IPerson>(admin => admin.Password == info.Password)
                    .AndWhere<IPerson>(admin => admin.Role == Roles.Admin)
                    .Return(admin => admin.As<IPerson>())
                    .Limit(1)
                    .ResultsAsync;

                // Invalid query result.
                var result = query.FirstOrDefault();
                if (result == null)
                    return null;

                result.Password = "";
                result.Id = "";

                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Filter person asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Person>> FilterPersonAsync(FilterPersonViewModel filter)
        {
            ICypherFluentQuery query;
            bool isWhereConditionUsed;

            // Query construction.
            CypherFilterPerson(filter, out query, out isWhereConditionUsed);

            // Calculate the number of records should be skip over.
            var skippedRecords = filter.Page*filter.Records;

            // Execute query asynchronously.
            var results = await query.Return(n => n.As<Person>())
                .Skip(skippedRecords)
                .Limit(filter.Records)
                .ResultsAsync;

            return results;
        }

        /// <summary>
        ///     Filter doctor with specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Doctor>> FilterDoctorAsync(FilterDoctorViewModel filter)
        {
            bool isWhereConditionUsed;
            ICypherFluentQuery query;

            // Filter must contain specific role.
            filter.Role = Roles.Doctor;

            // Firstly, filter general information.
            CypherFilterPerson(filter, out query, out isWhereConditionUsed);

            #region Specialization

            if (!string.IsNullOrEmpty(filter.Specialization))
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where($"n.Specialization =~ '(?i).*{filter.Specialization}.*'");
                    isWhereConditionUsed = true;
                }
                else
                    query = query.AndWhere($"n.Specialization =~ '(?i).*{filter.Specialization}.*'");
            }

            #endregion

            #region Specialization Areas

            if (filter.SpecializationAreas != null && filter.SpecializationAreas.Length > 0)
            {
                // TODO: Implement later.
                throw new NotImplementedException("Implement this please.");
            }

            #endregion

            #region Rank

            // Start of rank.
            if (filter.RankFrom != null)
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where<Doctor>(n => n.Rank >= filter.RankFrom);
                    isWhereConditionUsed = true;
                }
                else
                    query = query.AndWhere<Doctor>(n => n.Rank >= filter.RankFrom);
            }

            // End of rank.
            if (filter.RankTo != null)
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where<Doctor>(n => n.Rank <= filter.RankTo);
                    isWhereConditionUsed = true;
                }
                else
                    query = query.AndWhere<Doctor>(n => n.Rank <= filter.RankTo);
            }

            #endregion

            #region Cypher query execution

            // Calculate the number of records should be skip over.
            var skippedRecords = filter.Page*filter.Records;

            // Execute query asynchronously.
            var results = await query.Return(n => n.As<Doctor>())
                .Skip(skippedRecords)
                .Limit(filter.Records)
                .ResultsAsync;

            #endregion

            return results;
        }

        /// <summary>
        ///     Update personal information by search user GUID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<object> UpdatePersonAsync(string id, IPerson info)
        {
            // Keep the id of information.
            info.Id = id;

            var result = await _graphClient.Cypher
                .Match("(n:Person)")
                .Where<IPerson>(n => n.Id == id)
                .Set("n = {info}")
                .WithParam("info", info)
                .Return(n => n.As<Node<string>>())
                .ResultsAsync;

            return result;
        }

        /// <summary>
        ///     Personal information filter query construction.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="query"></param>
        /// <param name="isWhereConditionUsed"></param>
        private void CypherFilterPerson(FilterPersonViewModel filter, out ICypherFluentQuery query,
            out bool isWhereConditionUsed)
        {
            // By default, where condition hasn't been used.
            isWhereConditionUsed = false;

            // Whether where condition has been used or not.
            query = _graphClient.Cypher
                .Match("(n:Person)");

            #region First name

            if (!string.IsNullOrEmpty(filter.FirstName))
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where($"n.FirstName =~ '(?i).*{filter.FirstName}.*'");
                    isWhereConditionUsed = true;
                }
                else
                {
                    query.AndWhere($"n.FirstName =~ '(?i).*{filter.FirstName}.*'");
                }
            }

            #endregion

            #region Last name

            if (!string.IsNullOrEmpty(filter.LastName))
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where($"n.LastName =~ '(?i).*{filter.LastName}.*'");
                    isWhereConditionUsed = true;
                }
                else
                {
                    query.AndWhere($"n.LastName =~ '(?i).*{filter.LastName}.*'");
                }
            }

            #endregion

            #region Birthday

            // Start date of birth is set.
            if (filter.BirthdayFrom != null)
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where<IPerson>(n => n.Birthday >= filter.BirthdayFrom);
                    isWhereConditionUsed = true;
                }
                else
                    query = query.AndWhere<IPerson>(n => n.Birthday >= filter.BirthdayFrom);
            }

            // End date of birth is set.
            if (filter.BirthdayTo != null)
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where<IPerson>(n => n.Birthday <= filter.BirthdayTo);
                    isWhereConditionUsed = true;
                }
                else
                    query = query.AndWhere<IPerson>(n => n.Birthday <= filter.BirthdayTo);
            }

            #endregion

            #region Gender

            if (filter.Gender != null)
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where<IPerson>(n => n.Gender == filter.Gender);
                    isWhereConditionUsed = true;
                }
                else
                    query = query.AndWhere<IPerson>(n => n.Gender == filter.Gender);
            }

            #endregion

            #region Money

            if (filter.MoneyFrom != null)
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where<IPerson>(n => n.Money >= filter.MoneyFrom);
                    isWhereConditionUsed = true;
                }
                else
                    query = query.AndWhere<IPerson>(n => n.Money >= filter.MoneyFrom);
            }

            if (filter.MoneyTo != null)
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where<IPerson>(n => n.Money <= filter.MoneyTo);
                    isWhereConditionUsed = true;
                }
                else
                    query = query.AndWhere<IPerson>(n => n.Money <= filter.MoneyTo);
            }

            #endregion

            #region Created date

            if (filter.CreatedFrom != null)
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where<IPerson>(n => n.Created >= filter.CreatedFrom);
                    isWhereConditionUsed = true;
                }
                else
                    query = query.AndWhere<IPerson>(n => n.Created >= filter.CreatedFrom);
            }

            if (filter.CreatedTo != null)
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where<IPerson>(n => n.Created <= filter.CreatedTo);
                    isWhereConditionUsed = true;
                }
                else
                    query = query.AndWhere<IPerson>(n => n.Created <= filter.CreatedTo);
            }

            #endregion

            #region Role

            if (filter.Role != null)
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where<IPerson>(n => n.Role == filter.Role);
                    isWhereConditionUsed = true;
                }
                else
                    query = query.Where<IPerson>(n => n.Role == filter.Role);
            }

            #endregion
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;
using Neo4jClient.Transactions;
using Shared.Constants;
using Shared.Interfaces;
using Shared.Models.Nodes;
using Shared.ViewModels;

namespace Shared.Repositories
{
    public class RepositoryAccount : IRepositoryAccount
    {
        #region Properties

        /// <summary>
        ///     Instance which connects to neo4j database.
        /// </summary>
        private readonly GraphClient _graphClient;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize an instance of RepositoryAccount class.
        /// </summary>
        /// <param name="graphClient"></param>
        public RepositoryAccount(GraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        /// <summary>
        ///     Initialize an instance of RepositoryAccount class.
        /// </summary>
        public RepositoryAccount()
        {
        }

        #endregion

        #region Doctor

        /// <summary>
        ///     Find doctor by using GUID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IList<Doctor>> FindDoctorById(string id)
        {
            var resultAsync = await _graphClient.Cypher.Match("(n:Person)")
                .Where<IPerson>(n => n.Id == id)
                .AndWhere<IPerson>(n => n.Role == Roles.Doctor)
                .Return(n => n.As<Doctor>())
                .ResultsAsync;

            var results = resultAsync.ToList();
            return results;
        }

        /// <summary>
        /// Check identity and identity card to decide whether doctor can be registered or not.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identityCardNo"></param>
        /// <returns></returns>
        public async Task<bool> IsDoctorAbleToRegister(string id, string identityCardNo)
        {
            // By default, where condition hasn't been used.
            var hasWhereCondition = false;

            // Execute query asynchronously.
            var query = _graphClient.Cypher
                .Match("(n:Person)");

            #region Doctor identity

            if (!string.IsNullOrEmpty(id))
            {
                id = $"~'(?i){id}'";
                query = !hasWhereCondition
                    ? query.Where<Doctor>(n => n.Id == id)
                    : query.OrWhere<Doctor>(n => n.Id == id);

                hasWhereCondition = true;
            }

            #endregion

            #region Doctor identity card number

            if (!string.IsNullOrEmpty(identityCardNo))
            {
                query = !hasWhereCondition
                    ? query.Where($"n.IdentityCardNo =~ '(?i){identityCardNo}'")
                    : query.OrWhere($"n.IdentityCardNo =~ '(?i){identityCardNo}'");
            }

            #endregion

            // Retrieve query result.
            var queryResult = await query
                .Return(n => n.Count())
                .ResultsAsync;

            // Retrieve counter.
            var result = queryResult.SingleOrDefault();

            // Count the number of result.
            return result == 0;
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
            // Initialize match command
            var query = _graphClient.Cypher
                .Match("(n:Person)");

            // Email case sensitive check.
            if (emailCaseSensitive)
                query = query.Where<IPerson>(n => n.Email == email);
            else
                query = query.Where($"n.Email =~ '(?i){email}'");

            // Password filter if this property isn't empty.
            if (!string.IsNullOrEmpty(password))
                query = query.AndWhere<IPerson>(n => n.Password == password);

            // Role filter if this property isn't empty.
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
        ///     Check whether person exists or not.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="emailCaseSensitive">Whether email should be found case insensitively.</param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public IPerson GetPersonExist(string email, bool emailCaseSensitive = false,
            string password = "", byte? role = null)
        {
            // Initialize match command
            var query = _graphClient.Cypher
                .Match("(n:Person)");

            // Email case sensitive check.
            if (emailCaseSensitive)
                query = query.Where<IPerson>(n => n.Email == email);
            else
                query = query.Where($"n.Email =~ '(?i){email}'");

            // Password filter if this property isn't empty.
            if (!string.IsNullOrEmpty(password))
                query = query.AndWhere<IPerson>(n => n.Password == password);

            // Role filter if this property isn't empty.
            if (role != null)
                query = query.AndWhere<IPerson>(n => n.Role == role);

            // Retrieve query result asynchronously.
            var queryResult = query.Return(n => n.As<Person>()).Limit(1).Results;

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
        ///     Filter person asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<object> FilterPersonAsync(FilterPersonViewModel filter)
        {
            ICypherFluentQuery query;
            bool isWhereConditionUsed;

            // Query construction.
            FilterPerson(filter, out query, out isWhereConditionUsed);

            // Calculate the number of records should be skip over.
            var skippedRecords = filter.Page * filter.Records;

            // Execute query asynchronously.
            var results = await query.Return(n => n.As<Node<string>>())
                .Skip(skippedRecords)
                .Limit(filter.Records)
                .ResultsAsync;

            return results;
        }

        /// <summary>
        ///     Filter patients by using specific conditions
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponsePersonFilter> FilterPatientAsync(FilterPatientViewModel filter)
        {
            bool isWhereConditionUsed;
            ICypherFluentQuery query;

            // Filter must contain specific role.
            filter.Role = Roles.Patient;

            // Firstly, filter general information.
            FilterPerson(filter, out query, out isWhereConditionUsed);

            #region Weight

            // Start of rank.
            if (filter.MinWeight != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<Patient>(n => n.Weight >= filter.MinWeight)
                    : query.AndWhere<Patient>(n => n.Weight >= filter.MinWeight);
                ;
                isWhereConditionUsed = true;
            }

            // End of rank.
            if (filter.MaxWeight != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<Doctor>(n => n.Rank <= filter.MaxWeight.Value)
                    : query.AndWhere<Doctor>(n => n.Rank <= filter.MaxHeight.Value);
                isWhereConditionUsed = true;
            }

            #endregion

            #region Height

            // Start of rank.
            if (filter.MinHeight != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<Patient>(n => n.Height >= filter.MinHeight)
                    : query.AndWhere<Patient>(n => n.Height >= filter.MinHeight);
                ;
                isWhereConditionUsed = true;
            }

            // End of rank.
            if (filter.MaxHeight != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<Patient>(n => n.Height <= filter.MaxHeight)
                    : query.AndWhere<Patient>(n => n.Height <= filter.MaxHeight);
                isWhereConditionUsed = true;
            }

            #endregion

            #region Cypher query execution

            // Initialize filter result.
            var result = new ResponsePersonFilter();

            // Count total records.
            var cypherCountAsync = await query.Return(n => n.Count())
                .ResultsAsync;
            result.Total = (int)cypherCountAsync.SingleOrDefault();

            // No record has been retrieved.
            if (result.Total < 1)
                return result;

            // Calculate the number of records should be skip over.
            var skippedRecords = filter.Page * filter.Records;

            // Execute query asynchronously.
            var resultsAsync = await query.Return(n => n.As<Patient>())
                .Skip(skippedRecords)
                .Limit(filter.Records)
                .ResultsAsync;

            #endregion

            // Update result data.
            result.Data = new List<IPerson>(resultsAsync);

            return result;
        }

        /// <summary>
        ///     Filter doctor with specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponsePersonFilter> FilterDoctorAsync(FilterDoctorViewModel filter)
        {
            bool isWhereConditionUsed;
            ICypherFluentQuery query;

            // Filter must contain specific role.
            filter.Role = Roles.Doctor;

            // Firstly, filter general information.
            FilterPerson(filter, out query, out isWhereConditionUsed);

            #region Specialization

            if (!string.IsNullOrEmpty(filter.Specialization))
            {
                var specializationQuery = $"n.Specialization =~'(?i).*{filter.Specialization}.*'";
                query = !isWhereConditionUsed
                    ? query.Where(specializationQuery)
                    : query.AndWhere(specializationQuery);
                isWhereConditionUsed = true;
            }

            #endregion

            #region Rank

            // Start of rank.
            if (filter.MinRank != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<Doctor>(n => n.Rank >= filter.MinRank)
                    : query.AndWhere<Doctor>(n => n.Rank >= filter.MinRank);
                ;
                isWhereConditionUsed = true;
            }

            // End of rank.
            if (filter.MaxRank != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<Doctor>(n => n.Rank <= filter.MaxRank)
                    : query.AndWhere<Doctor>(n => n.Rank <= filter.MaxRank);
                isWhereConditionUsed = true;
            }

            #endregion

            #region Identity card number

            if (!string.IsNullOrEmpty(filter.IdentityCardNo))
            {
                var identityCardQuery = $"n.IdentityCardNo =~ '(?i).*{filter.IdentityCardNo}.*'";
                query = !isWhereConditionUsed
                    ? query.Where(identityCardQuery)
                    : query.AndWhere(identityCardQuery);
                //isWhereConditionUsed = true;
            }

            #endregion

            #region Cypher query execution

            // Initialize filter result.
            var result = new ResponsePersonFilter();

            // Count total records.
            var cypherCountAsync = await query.Return(n => n.Count())
                .ResultsAsync;
            result.Total = (int)cypherCountAsync.SingleOrDefault();

            // No record has been retrieved.
            if (result.Total < 1)
                return result;

            // Calculate the number of records should be skip over.
            var skippedRecords = filter.Page * filter.Records;

            // Execute query asynchronously.
            var resultsAsync = await query.Return(n => n.As<Doctor>())
                .Skip(skippedRecords)
                .Limit(filter.Records)
                .ResultsAsync;

            #endregion

            // Update result data.
            result.Data = new List<IPerson>(resultsAsync);

            return result;
        }

        #endregion

        #region Patient

        /// <summary>
        ///     Find person by using GUID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IList<Patient>> FindPatientById(string id)
        {
            var resultAsync = await _graphClient.Cypher.Match("(n:Person)")
                .Where<IPerson>(n => n.Id == id)
                .Return(n => n.As<Patient>())
                .ResultsAsync;

            var results = resultAsync.ToList();
            return results;
        }

        /// <summary>
        ///     Using id and email to check whether person can be created or not.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> IsPatientAbleToCreated(string id, string email)
        {
            // Initialize match query.
            var query = _graphClient.Cypher.Match("(n:Person)")
                .Where<IPerson>(n => n.Role == Roles.Patient);

            var isWhereAvailable = true;

            // Patient id.
            if (!string.IsNullOrEmpty(id))
            {
                query = isWhereAvailable
                    ? query.AndWhere<IPerson>(n => n.Id == id)
                    : query.OrWhere<IPerson>(n => n.Id == id);

                isWhereAvailable = false;
            }

            // Patient email
            if (!string.IsNullOrEmpty(email))
            {
                var cypherEmail = $"n.Email =~ '(?i){email}'";

                query = isWhereAvailable
                    ? query.AndWhere(cypherEmail)
                    : query.OrWhere(cypherEmail);

                isWhereAvailable = false;
            }

            var resultAsync = await query.Return(n => n.Count())
                .ResultsAsync;

            // Retrieve counter.
            var result = resultAsync.SingleOrDefault();

            // Count the number of result.
            return result == 0;
        }

        /// <summary>
        /// Initialize personal note of patient.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="note"></param>
        public bool InitializePatientNote(string id, PersonalNote note)
        {
            var transactClient = (ITransactionalGraphClient)_graphClient;
            using (var transactSession = transactClient.BeginTransaction())
            {
                try
                {
                    // Create :NOTES connection from person to personal note as conditions are met.
                    transactClient.Cypher
                        .Match("(p:Person)")
                        .Where<IPerson>(p => p.Id == id)
                        .AndWhere<IPerson>(p => p.Role == Roles.Admin)
                        .Create("(p)-[:NOTES]->(a:PersonalNote {note})")
                        .WithParam("note", note)
                        .ExecuteWithoutResults();

                    transactSession.Commit();
                    return true;
                }
                catch (Exception exception)
                {
                    // TODO: Initialize a log here.
                    // Rollback the transaction because of error.
                    transactSession.Rollback();

                    return false;
                }
            }
        }

        /// <summary>
        /// Initialize an allergy connected to a person.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="allergy"></param>
        /// <returns></returns>
        public bool InitializePatientAllergies(string id, Allergy allergy)
        {
            var transactClient = (ITransactionalGraphClient)_graphClient;
            using (var transactSession = transactClient.BeginTransaction())
            {
                try
                {
                    // Create :NOTES connection from person to personal note as conditions are met.
                    transactClient.Cypher
                        .Match("(p:Person)")
                        .Where<IPerson>(p => p.Id == id)
                        .AndWhere<IPerson>(p => p.Role == Roles.Admin)
                        .Create("(p)-[:IS_ALLERGIC_WITH]->(a:Allergy {allergy})")
                        .WithParam("allergy", allergy)
                        .ExecuteWithoutResults();

                    transactSession.Commit();

                    return true;
                }
                catch (Exception exception)
                {
                    // TODO: Initialize a log here.
                    // Rollback the transaction because of error.
                    transactSession.Rollback();

                    return false;
                }
            }
        }

        /// <summary>
        /// Initialize addiction causes to a patient by using patient id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="addiction"></param>
        /// <returns></returns>
        public bool InitializePatientAddiction(string id, Addiction addiction)
        {
            var transactClient = (ITransactionalGraphClient)_graphClient;
            using (var transactSession = transactClient.BeginTransaction())
            {
                try
                {
                    // Create :NOTES connection from person to personal note as conditions are met.
                    transactClient.Cypher
                        .Match("(p:Person)")
                        .Where<IPerson>(p => p.Id == id)
                        .AndWhere<IPerson>(p => p.Role == Roles.Admin)
                        .Create("(p)-[:ADDICTS_WITH]->(a:Addiction {addiction})")
                        .WithParam("addiction", addiction)
                        .ExecuteWithoutResults();

                    transactSession.Commit();

                    return true;
                }
                catch (Exception exception)
                {
                    // TODO: Initialize a log here.
                    // Rollback the transaction because of error.
                    transactSession.Rollback();

                    return false;
                }
            }
        }

        #endregion

        #region Shared

        /// <summary>
        ///     Create person synchronously with given parameter.
        /// </summary>
        /// <param name="info"></param>
        public bool InitializePerson(IPerson info)
        {
            // Cast normal graph client to a transact client to do a transaction.
            var transactClient = (ITransactionalGraphClient)_graphClient;

            using (var transaction = transactClient.BeginTransaction())
            {
                try
                {
                    var query = transactClient.Cypher
                        .Merge($"(p:Person {{Id : '{info.Id}'}})")
                        .Set("p = {person}")
                        .WithParam("person", info);

                    // Input parameters. 
                    query.ExecuteWithoutResults();

                    // Confirm to do execute transaction.
                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {
                    // As exception is thrown, roll back the transaction and tell client the transaction is failed.
                    transaction.Rollback();

                    return false;
                }
            }
        }

        /// <summary>
        ///     Update personal information by search user GUID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Node<string>>> EditPersonAsync(string id, IPerson info)
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
        ///     Log user in
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IPerson> LoginAsync(LoginViewModel info)
        {
            try
            {
                // Query construction.
                var query = _graphClient.Cypher.Match("(admin:Person)")
                    .Where<IPerson>(admin => admin.Email == info.Email)
                    .AndWhere<IPerson>(admin => admin.Password == info.Password);

                // If role is specified, filter account by role.
                if (info.Role != null) query = query.AndWhere<IPerson>(admin => admin.Role == info.Role);

                // Retrieve result asynchronously.
                var resultAsync = await query.Return(admin => admin.As<Person>())
                    .ResultsAsync;

                // Result is invalid.
                if (resultAsync == null)
                    return null;

                // Invalid query result.
                var result = resultAsync.ToList();

                // Not only unique result has been retrieved.
                if (result.Count != 1)
                    return null;
                
                result[0].Password = "";
                result[0].Id = "";

                return result[0];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Change account status base on account id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<bool> ModifyAccountStatus(string id, byte status)
        {
            try
            {
                await _graphClient.Cypher.OptionalMatch("(n:Person)")
                    .Where<IPerson>(n => n.Id == id)
                    .Set($"n.Status = {status}")
                    .ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Personal information filter query construction.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="query"></param>
        /// <param name="isWhereConditionUsed"></param>
        private void FilterPerson(FilterPersonViewModel filter, out ICypherFluentQuery query,
            out bool isWhereConditionUsed)
        {
            // By default, where condition hasn't been used.
            isWhereConditionUsed = false;

            // Whether where condition has been used or not.
            query = _graphClient.Cypher
                .Match("(n:Person)");

            #region First name

            // Filter by first name.
            if (!string.IsNullOrEmpty(filter.FirstName))
            {
                var queryFirstName = $"n.FirstName =~'(?i).*{filter.FirstName}.*'";
                query = query.Where(queryFirstName);
                isWhereConditionUsed = true;
            }

            #endregion

            #region Last name

            // Filter by last name.
            if (!string.IsNullOrEmpty(filter.LastName))
            {
                // Last name matching query construction.
                var queryLastName = $"n.LastName =~'(?i).*{filter.LastName}.*'";
                query = !isWhereConditionUsed ? query.Where(queryLastName) : query.AndWhere(queryLastName);
                isWhereConditionUsed = true;
            }

            #endregion

            #region Birthday

            // Start date of birth is set.
            if (filter.MinBirthday != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<IPerson>(n => n.Birthday >= filter.MinBirthday)
                    : query.AndWhere<IPerson>(n => n.Birthday >= filter.MinBirthday);
                isWhereConditionUsed = true;
            }

            // End date of birth is set.
            if (filter.MaxBirthday != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<IPerson>(n => n.Birthday <= filter.MaxBirthday)
                    : query.AndWhere<IPerson>(n => n.Birthday <= filter.MaxBirthday);
                isWhereConditionUsed = true;
            }

            #endregion

            #region Gender

            if (filter.Gender != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<IPerson>(n => n.Gender == filter.Gender)
                    : query.AndWhere<IPerson>(n => n.Gender == filter.Gender);
                isWhereConditionUsed = true;
            }

            #endregion

            #region Money

            if (filter.MoneyFrom != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<IPerson>(n => n.Money >= filter.MoneyFrom)
                    : query.AndWhere<IPerson>(n => n.Money >= filter.MoneyFrom);
                isWhereConditionUsed = true;
            }

            if (filter.MoneyTo != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<IPerson>(n => n.Money <= filter.MoneyTo)
                    : query.AndWhere<IPerson>(n => n.Money <= filter.MoneyTo);
                isWhereConditionUsed = true;
            }

            #endregion

            #region Created date

            if (filter.MinCreated != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<IPerson>(n => n.Created >= filter.MinCreated)
                    : query.AndWhere<IPerson>(n => n.Created >= filter.MinCreated);
                isWhereConditionUsed = true;
            }

            if (filter.MaxCreated != null)
            {
                if (!isWhereConditionUsed)
                {
                    query = query.Where<IPerson>(n => n.Created <= filter.MaxCreated);
                    isWhereConditionUsed = true;
                }
                else
                    query = query.AndWhere<IPerson>(n => n.Created <= filter.MaxCreated);
            }

            #endregion

            #region Role

            if (filter.Role != null)
            {
                query = !isWhereConditionUsed
                    ? query.Where<IPerson>(n => n.Role == filter.Role)
                    : query.Where<IPerson>(n => n.Role == filter.Role);
                isWhereConditionUsed = true;
            }

            #endregion
        }

        #endregion
    }
}
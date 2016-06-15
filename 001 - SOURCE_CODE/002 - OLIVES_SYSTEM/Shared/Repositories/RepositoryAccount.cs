using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;
using Neo4jClient.Transactions;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
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
        private readonly ITransactionalGraphClient _graphClient;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize an instance of RepositoryAccount class.
        /// </summary>
        /// <param name="graphClient"></param>
        public RepositoryAccount(ITransactionalGraphClient graphClient)
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
                .AndWhere<IPerson>(n => n.Role == AccountRole.Doctor)
                .Return(n => n.As<Doctor>())
                .ResultsAsync;

            var results = resultAsync.ToList();
            return results;
        }

        /// <summary>
        ///     Check identity and identity card to decide whether doctor can be registered or not.
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
        ///     Filter person asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Node<string>>> FilterPersonAsync(FilterPersonViewModel filter)
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
            filter.Role = AccountRole.Patient;

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
            filter.Role = AccountRole.Doctor;

            // Firstly, filter general information.
            FilterPerson(filter, out query, out isWhereConditionUsed);

            #region Speciality

            if (!string.IsNullOrEmpty(filter.Speciality))
            {
                var specializationQuery =
                    $"n.{MethodHelper.Instance.RetrievePropertyName(() => filter.Speciality)} =~'(?i).*{filter.Speciality}.*'";
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

            //#region Identity card number

            //if (!string.IsNullOrEmpty(filter.IdentityCardNo))
            //{
            //    var identityCardQuery = $"n.IdentityCardNo =~ '(?i).*{filter.IdentityCardNo}.*'";
            //    query = !isWhereConditionUsed
            //        ? query.Where(identityCardQuery)
            //        : query.AndWhere(identityCardQuery);
            //    //isWhereConditionUsed = true;
            //}

            //#endregion

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

        /// <summary>
        ///     This function is for initializing a doctor with an identity card.
        /// </summary>
        /// <param name="doctor"></param>
        /// <param name="identityCard"></param>
        /// <returns></returns>
        public async Task<long> InitializeDoctor(Doctor doctor, IdentityCard identityCard)
        {
            // Query initialization.
            ICypherFluentQuery query;

            //OPTIONAL MATCH (p:Person) WHERE p.Id = '1234' WITH p
            query = _graphClient.Cypher.OptionalMatch("(p:Person)")
                .Where<Doctor>(p => p.Id == doctor.Id)
                .With("p");

            //OPTIONAL MATCH (i:IdentityCard) WHERE i.Id = '12345' 
            query = query.OptionalMatch("(i:IdentityCard)")
                .Where<IdentityCard>(i => i.No == identityCard.No);

            #region With section

            var withCommand = "CASE ";
            withCommand += "WHEN p IS NULL THEN 1 ";
            withCommand += "WHEN i IS NULL THEN 2 ";
            withCommand += "ELSE 0 ";
            withCommand += "END AS results";

            query = query.With(withCommand);

            #endregion

            #region Foreach section

            var forEachCommand =
                "(result in case when results = 0 then [1] else [] end | CREATE(p) -[:HAS_IDENTITY]->(i))";

            query = query.With("withCommand")
                .ForEach(forEachCommand);

            #endregion

            var resultAsync = await query.Return(n => n.Count())
                .ResultsAsync;

            return resultAsync.SingleOrDefault();
        }

        /// <summary>
        ///     Change doctor status by using id and role.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        public async void ModifyPersonStatus(string id, int status)
        {
            var query = _graphClient.Cypher.Match("(p:Person)")
                .Where<IPerson>(p => p.Id == id)
                .Set($"p.Status = {status}");

            await query.ExecuteWithoutResultsAsync();

            /*
             * Find user by using specific id
             * - If status of user has been changed before, return 0
             * - Person hasn't been found or not only unique record has been retrieved, return -1
             * - Otherwise 1
             */
            //var query = _graphClient.Cypher.OptionalMatch("(p:Person)")
            //    .Where<IPerson>(p => p.Id == id)
            //    .With(
            //        $"p,CASE WHEN COUNT(p) <> 1 THEN 0 ELSE 1 END as result")
            //    .ForEach(
            //        $"(temp in CASE WHEN result=1 THEN [1] else [] end | SET p.Status = {status})")
            //        .Return(p => Return.As<int>("result"));

            //// Retrieve result.
            //var resultAsync = await query.ResultsAsync;
            //return resultAsync.SingleOrDefault();
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
                .Where<IPerson>(n => n.Role == AccountRole.Patient);

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
        ///     Initialize personal note of patient.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="note"></param>
        public bool InitializePatientNote(string id, PersonalNote note)
        {
            var transactClient = _graphClient;
            using (var transactSession = transactClient.BeginTransaction())
            {
                try
                {
                    // Create :NOTES connection from person to personal note as conditions are met.
                    transactClient.Cypher
                        .Match("(p:Person)")
                        .Where<IPerson>(p => p.Id == id)
                        .AndWhere<IPerson>(p => p.Role == AccountRole.Admin)
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
        ///     Initialize an allergy connected to a person.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="allergy"></param>
        /// <returns></returns>
        public bool InitializePatientAllergies(string id, Allergy allergy)
        {
            var transactClient = _graphClient;
            using (var transactSession = transactClient.BeginTransaction())
            {
                try
                {
                    // Create :NOTES connection from person to personal note as conditions are met.
                    transactClient.Cypher
                        .Match("(p:Person)")
                        .Where<IPerson>(p => p.Id == id)
                        .AndWhere<IPerson>(p => p.Role == AccountRole.Admin)
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
        ///     Initialize addiction causes to a patient by using patient id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="addiction"></param>
        /// <returns></returns>
        public bool InitializePatientAddiction(string id, Addiction addiction)
        {
            var transactClient = _graphClient;
            using (var transactSession = transactClient.BeginTransaction())
            {
                try
                {
                    // Create :NOTES connection from person to personal note as conditions are met.
                    transactClient.Cypher
                        .Match("(p:Person)")
                        .Where<IPerson>(p => p.Id == id)
                        .AndWhere<IPerson>(p => p.Role == AccountRole.Admin)
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
        public async Task<Node<string>> InitializePerson(IPerson info)
        {
            try
            {
                var query = _graphClient.Cypher
                    .Merge($"(p:Person {{Id : '{info.Id}'}})")
                    .Set("p = {person}")
                    .WithParam("person", info)
                    .Return(p => p.As<Node<string>>());

                // Do the query and receive the result.
                var resultAsync = await query.ResultsAsync;

                // Retrieve the first node.
                var result = resultAsync.FirstOrDefault();

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Create person synchronously with given parameter.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="code">Account activation code</param>
        public async Task<ResponsePersonCode> InitializePerson(IPerson info, ActivationCode code)
        {
            try
            {
                var query = _graphClient.Cypher
                    .Merge($"(p:Person {{Id : '{info.Id}'}})-[:HAS_ACTIVATION_CODE]->(a:ActivationCode)")
                    .DetachDelete("a")
                    .Set("p = {person}")
                    .WithParam("person", info)
                    .Create("(p)-[:HAS_ACTIVATION_CODE]->(c:ActivationCode {code}")
                    .WithParam("code", code)
                    .Return((p, c) =>
                        new ResponsePersonCode
                        {
                            Person = p.As<Person>(),
                            Code = c.As<ActivationCode>()
                        });

                // Do the query and receive the result.
                var resultAsync = await query.ResultsAsync;

                // Retrieve the first node.
                var result = resultAsync.FirstOrDefault();

                return result;
            }
            catch (Exception)
            {
                return null;
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
                var query = _graphClient.Cypher.Match("(p:Person)")
                    .Where<IPerson>(p => p.Email == info.Email)
                    .AndWhere<IPerson>(p => p.Password == info.Password);

                // If role is specified, filter account by role.
                if (info.Role != null) query = query.AndWhere<IPerson>(p => p.Role == info.Role);

                // Retrieve result asynchronously.
                var resultAsync = await query.Return(p => p.As<Person>()).ResultsAsync;

                // Result is invalid.
                if (resultAsync == null)
                    return null;

                // Invalid query result.
                var result = resultAsync.ToList();

                // Not only unique result has been retrieved.
                if (result.Count != 1)
                    return null;

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
        public async Task<bool> EditPersonStatus(string id, int status)
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
        /// <param name="isWhereUnavailable"></param>
        private void FilterPerson(FilterPersonViewModel filter, out ICypherFluentQuery query,
            out bool isWhereUnavailable)
        {
            // By default, where condition hasn't been used.
            isWhereUnavailable = false;

            // Whether where condition has been used or not.
            query = _graphClient.Cypher
                .Match("(n:Person)");

            #region Email

            // Filter by email.
            if (!string.IsNullOrEmpty(filter.Email))
            {
                var queryEmail = $"n.Email =~'(?i).*{filter.Email}.*'";
                query = (!isWhereUnavailable) ? query.Where(queryEmail) : query.AndWhere(queryEmail);
                isWhereUnavailable = true;
            }

            #endregion

            #region Phone

            // Filter by phone.
            if (!string.IsNullOrEmpty(filter.Phone))
            {
                var queryPhone = $"n.Phone =~'(?i).*{filter.Phone}.*'";
                query = (!isWhereUnavailable) ? query.Where(queryPhone) : query.AndWhere(queryPhone);
                isWhereUnavailable = true;
            }

            #endregion
            
            #region First name

            // Filter by first name.
            if (!string.IsNullOrEmpty(filter.FirstName))
            {
                var queryFirstName = $"n.FirstName =~'(?i).*{filter.FirstName}.*'";
                query = query.Where(queryFirstName);
                isWhereUnavailable = true;
            }

            #endregion

            #region Last name

            // Filter by last name.
            if (!string.IsNullOrEmpty(filter.LastName))
            {
                // Last name matching query construction.
                var queryLastName = $"n.LastName =~'(?i).*{filter.LastName}.*'";
                query = !isWhereUnavailable ? query.Where(queryLastName) : query.AndWhere(queryLastName);
                isWhereUnavailable = true;
            }

            #endregion

            #region Birthday

            // Start date of birth is set.
            if (filter.MinBirthday != null)
            {
                query = !isWhereUnavailable
                    ? query.Where<IPerson>(n => n.Birthday >= filter.MinBirthday)
                    : query.AndWhere<IPerson>(n => n.Birthday >= filter.MinBirthday);
                isWhereUnavailable = true;
            }

            // End date of birth is set.
            if (filter.MaxBirthday != null)
            {
                query = !isWhereUnavailable
                    ? query.Where<IPerson>(n => n.Birthday <= filter.MaxBirthday)
                    : query.AndWhere<IPerson>(n => n.Birthday <= filter.MaxBirthday);
                isWhereUnavailable = true;
            }

            #endregion

            #region Gender

            if (filter.Gender != null)
            {
                query = !isWhereUnavailable
                    ? query.Where<IPerson>(n => n.Gender == filter.Gender)
                    : query.AndWhere<IPerson>(n => n.Gender == filter.Gender);
                isWhereUnavailable = true;
            }

            #endregion

            #region Money

            if (filter.MinMoney != null)
            {
                query = !isWhereUnavailable
                    ? query.Where<IPerson>(n => n.Money >= filter.MinMoney)
                    : query.AndWhere<IPerson>(n => n.Money >= filter.MinMoney);
                isWhereUnavailable = true;
            }

            if (filter.MaxMoney != null)
            {
                query = !isWhereUnavailable
                    ? query.Where<IPerson>(n => n.Money <= filter.MaxMoney)
                    : query.AndWhere<IPerson>(n => n.Money <= filter.MaxMoney);
                isWhereUnavailable = true;
            }

            #endregion

            #region Created date

            if (filter.MinCreated != null)
            {
                query = !isWhereUnavailable
                    ? query.Where<IPerson>(n => n.Created >= filter.MinCreated)
                    : query.AndWhere<IPerson>(n => n.Created >= filter.MinCreated);
                isWhereUnavailable = true;
            }

            if (filter.MaxCreated != null)
            {
                if (!isWhereUnavailable)
                {
                    query = query.Where<IPerson>(n => n.Created <= filter.MaxCreated);
                    isWhereUnavailable = true;
                }
                else
                    query = query.AndWhere<IPerson>(n => n.Created <= filter.MaxCreated);
            }

            #endregion

            #region Role

            if (filter.Role != null)
            {
                query = !isWhereUnavailable
                    ? query.Where<IPerson>(n => n.Role == filter.Role)
                    : query.AndWhere<IPerson>(n => n.Role == filter.Role);
                isWhereUnavailable = true;
            }

            #endregion

            #region Status

            if (filter.Status != null)
            {
                query = !isWhereUnavailable
                    ? query.Where<IPerson>(n => n.Status == filter.Status)
                    : query.AndWhere<IPerson>(n => n.Status == filter.Status);
            }

            #endregion

            #region Last Modified

            if (filter.MinLastModified != null)
            {
                query = !isWhereUnavailable
                    ? query.Where<IPerson>(n => n.LastModified >= filter.MinLastModified)
                    : query.AndWhere<IPerson>(n => n.LastModified >= filter.MinLastModified);
            }

            if (filter.MaxLastModified != null)
            {
                query = !isWhereUnavailable
                    ? query.Where<IPerson>(n => n.LastModified <= filter.MaxLastModified)
                    : query.AndWhere<IPerson>(n => n.LastModified <= filter.MaxLastModified);
            }

            #endregion
        }

        /// <summary>
        ///     Check whether person exists or not.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public IPerson FindPerson(string email, string password, int? role)
        {
            // No email has been specified.
            if (string.IsNullOrEmpty(email))
                throw new Exception("Email is required.");

            // Initialize match command
            var query = _graphClient.Cypher
                .Match("(n:Person)")
                .Where<IPerson>(n => n.Email == email);

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
        ///     Find person by using GUID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Person> FindPerson(string id)
        {
            var query = _graphClient.Cypher
                .Match("(n:Person)")
                .Where<IPerson>(n => n.Id == id);

            // Retrieve query result asynchronously.
            var resultAsync = await query.Return(n => n.As<Person>()).Limit(1).ResultsAsync;

            // No result has been retrieved.
            if (resultAsync == null)
                return null;

            // Retrieve the first queried result.
            var person = resultAsync.FirstOrDefault();

            if (person == null)
                return null;

            return person;
        }

        /// <summary>
        ///     Statistic person base on conditions.
        /// </summary>
        /// <param name="role">Role of person</param>
        /// <returns></returns>
        public async Task<IList<StatusStatisticViewModel>> SummarizePersonRole(int? role)
        {
            // Query initialization.
            var query = _graphClient.Cypher.Match("(p:Person)");

            // Role has been specified.
            if (role != null)
                query = query.Where<IPerson>(p => p.Role == role);

            // Retrieve the statistic result asynchronously.
            var resultAsync = await query.Return(p => new StatusStatisticViewModel
            {
                Role = p.As<Person>().Role,
                Status = p.As<Person>().Status,
                Total = All.Count()
            }).ResultsAsync;

            return resultAsync?.ToList();
        }

        #endregion
    }
}
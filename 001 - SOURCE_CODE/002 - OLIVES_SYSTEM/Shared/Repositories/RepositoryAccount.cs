using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace Shared.Repositories
{
    public class RepositoryAccount : IRepositoryAccount
    {
        #region Patient

        /// <summary>
        ///     Find the patient with id and may be status asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<Patient> FindPatientAsync(int id, byte? status)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all patients.
            IQueryable<Patient> patients = context.Patients;

            // Filter patient by id.
            patients = patients.Where(x => x.Id == id);

            // Status is defined.
            if (status != null)
                patients = patients.Where(x => x.Person.Status == (byte) status);

            return await patients.FirstOrDefaultAsync();
        }
        
        /// <summary>
        ///     Filter doctor by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponsePatientFilter> FilterPatientAsync(FilterPatientViewModel filter)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Join the table first.
            var results = from person in context.People.Where(x => x.Role == (byte) Role.Patient)
                join patient in context.Patients on person.Id equals patient.Id
                select patient;
            
            #region Result filter

            // Filter doctor by using email.
            if (!string.IsNullOrEmpty(filter.Email))
                results = results.Where(x => x.Person.Email.Contains(filter.Email));

            // Filter doctor by using phone number.
            if (!string.IsNullOrEmpty(filter.Phone))
                results = results.Where(x => x.Person.Phone.Contains(filter.Phone));

            // Filter by last modified.
            if (filter.MinLastModified != null)
                results = results.Where(x => x.Person.LastModified >= filter.MinLastModified);

            if (filter.MaxLastModified != null)
                results = results.Where(x => x.Person.LastModified <= filter.MaxLastModified);

            // Filter by using name
            if (!string.IsNullOrEmpty(filter.Name))
                results = results.Where(x => x.Person.FullName.Contains(filter.Name));

            // Filter by using birthday.
            if (filter.MinBirthday != null)
                results = results.Where(x => x.Person.Birthday >= filter.MinBirthday);

            if (filter.MaxBirthday != null)
                results = results.Where(x => x.Person.Birthday <= filter.MaxBirthday);

            // Filter by gender.
            if (filter.Gender != null)
                results = results.Where(x => x.Person.Gender == filter.Gender);

            // Filter by money.
            if (filter.MinMoney != null)
                results = results.Where(x => x.Money >= filter.MinMoney);

            if (filter.MaxMoney != null)
                results = results.Where(x => x.Money <= filter.MaxMoney);

            // Filter by created.
            if (filter.MinCreated != null)
                results = results.Where(x => x.Person.Created >= filter.MinCreated);

            if (filter.MaxCreated != null)
                results = results.Where(x => x.Person.Created <= filter.MaxCreated);

            // Filter by status.
            if (filter.Status != null)
                results = results.Where(x => x.Person.Status == filter.Status);

            // Filter by height.
            if (filter.MinHeight != null) results = results.Where(x => x.Height >= filter.MinHeight);
            if (filter.MaxHeight != null) results = results.Where(x => x.Height <= filter.MaxHeight);

            // Filter by weight.
            if (filter.MinWeight != null) results = results.Where(x => x.Weight >= filter.MinWeight);
            if (filter.MaxWeight != null) results = results.Where(x => x.Weight <= filter.MaxWeight);

            #endregion

            var response = new ResponsePatientFilter();
            response.Total = await results.CountAsync();

            // How many records should be skipped.
            var skippedRecords = filter.Page*filter.Records;
            response.Patients = await results
                .OrderBy(x => x.Person.Status)
                .Skip(skippedRecords)
                .Take(filter.Records)
                .ToListAsync();
            
            return response;
        }

        /// <summary>
        ///     Find and activate patient's account and remove the activation code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<bool> InitializePatientActivation(string code)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var results = from p in context.People
                        join c in context.AccountCodes.Where(x => x.Code.Equals(code) && x.Type == (byte)TypeAccountCode.Activation) on p.Id equals c.Owner
                        select new
                        {
                            Person = p,
                            Code = c
                        };

                    // Retrieve the total matched result.
                    var resultsCount = await results.CountAsync();

                    // No result has been returned.
                    if (resultsCount < 1)
                        throw new InstanceNotFoundException($"Couldn't find person whose code is : {code}");

                    // Not only one result has been returned.
                    if (resultsCount > 1)
                        throw new Exception($"There are too many people whose code is : {code}");

                    // Retrieve the first queried result.
                    var result = await results.FirstOrDefaultAsync();
                    if (result == null)
                        throw new InstanceNotFoundException($"Couldn't find person whose code is : {code}");

                    // Update the person status and remove the activation code.
                    var person = result.Person;
                    var activationCode = result.Code;
                    person.Status = (byte) StatusAccount.Active;
                    context.People.AddOrUpdate(person);
                    context.AccountCodes.Remove(activationCode);
                    await context.SaveChangesAsync();
                    // Commit the transaction.
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    // Something happens, roll the transaction back.
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #endregion

        #region Doctor

        /// <summary>
        ///     Filter doctor by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseDoctorFilter> FilterDoctorAsync(FilterDoctorViewModel filter)
        {
            // Database connection context initialization.
            var context = new OlivesHealthEntities();

            // Take all people from database.
            var people = context.People.Where(x => x.Role == (byte) Role.Doctor);

            // By default, take all doctors.
            IQueryable<Doctor> doctors = context.Doctors;

            #region People filter

            // Filter doctor by using email.
            if (!string.IsNullOrEmpty(filter.Email))
                people = people.Where(x => x.Email.Contains(filter.Email));

            // Filter doctor by using phone number.
            if (!string.IsNullOrEmpty(filter.Phone))
                people = people.Where(x => x.Phone.Contains(filter.Phone));

            // Filter by using full name.
            if (!string.IsNullOrEmpty(filter.Name))
                people = people.Where(x => x.FullName.Contains(filter.Name));

            // Filter by using birthday.
            if (filter.MinBirthday != null)
                people = people.Where(x => x.Birthday >= filter.MinBirthday);

            if (filter.MaxBirthday != null)
                people = people.Where(x => x.Birthday <= filter.MaxBirthday);

            // Filter by gender.
            if (filter.Gender != null)
                people = people.Where(x => x.Gender == filter.Gender);

            // Filter by last modified.
            if (filter.MinLastModified != null)
                people = people.Where(x => x.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                people = people.Where(x => x.LastModified <= filter.MaxLastModified);

            // Filter by created.
            if (filter.MinCreated != null)
                people = people.Where(x => x.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                people = people.Where(x => x.Created <= filter.MaxCreated);

            // Filter by status.
            if (filter.Status != null)
                people = people.Where(x => x.Status == filter.Status);

            #endregion

            #region Doctors

            // Filter by money.
            if (filter.MinMoney != null) doctors = doctors.Where(x => x.Money >= filter.MinMoney);
            if (filter.MaxMoney != null) doctors = doctors.Where(x => x.Money <= filter.MaxMoney);

            // Filter by rank.
            if (filter.MinRank != null) doctors = doctors.Where(x => x.Rank >= filter.MinRank);
            if (filter.MaxRank != null) doctors = doctors.Where(x => x.Rank <= filter.MaxRank);

            // Filter by id of specialty.
            if (filter.Specialty != null) doctors = doctors.Where(x => x.SpecialtyId == filter.Specialty);

            #endregion

            #region Specialties

            // By default, join all specialties.
            IQueryable<Specialty> specialties = context.Specialties;

            // Specialty has been specified.
            if (filter.Specialty != null)
                specialties = specialties.Where(x => x.Id == filter.Specialty);

            #endregion

            // Join the tables first.
            var results = from p in people
                join d in doctors on p.Id equals d.Id
                join s in specialties on d.SpecialtyId equals s.Id
                select d;

            var responseFilter = new ResponseDoctorFilter();
            responseFilter.Total = await results.CountAsync();

            // How many records should be skipped.
            var skippedRecords = filter.Page*filter.Records;

            responseFilter.Doctors = await results
                .OrderBy(x => x.Person.Status)
                .Skip(skippedRecords)
                .Take(filter.Records)
                .ToListAsync();
                
            return responseFilter;
        }

        /// <summary>
        ///     Initialize a doctor asynchronously.
        /// </summary>
        /// <param name="doctor"></param>
        /// <returns></returns>
        public async Task<Doctor> InitializeDoctorAsync(Doctor doctor)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Doctors.Add(doctor);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return doctor;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        ///     Find the doctor by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<Doctor> FindDoctorAsync(int id, StatusAccount? status)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            IQueryable<Doctor> doctors = context.Doctors;

            // Find doctors by using id.
            doctors = doctors.Where(x => x.Id == id);

            // Status is defined.
            if (status != null)
                doctors = doctors.Where(x => x.Person.Status == (byte) status);

            return await doctors.FirstOrDefaultAsync();
        }

        #endregion

        #region Shared

        /// <summary>
        ///     Find person by using email, password and role.
        /// </summary>
        /// <param name="id">Id of person</param>
        /// <param name="email">Email which is used for filtering.</param>
        /// <param name="password">Password of account.</param>
        /// <param name="role">As role is specified. Find account by role.</param>
        /// <returns></returns>
        public Person FindPerson(int? id, string email, string password, byte? role)
        {
            // Database context intialize.
            var context = new OlivesHealthEntities();

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
            // Database context initialization.
            var context = new OlivesHealthEntities();

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
        ///     Find a person asynchronously by using activation code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<Person> FindPersonAsync(string code)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the person whose activation code matches the condition.
            var results = from p in context.People
                join a in context.AccountCodes.Where(x => x.Code.Equals(code)) on p.Id equals a.Owner
                select p;

            // Count the number of matched records.
            var records = await results.CountAsync();

            // Result is not unique.
            if (records != 1)
                return null;

            return await results.FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Edit person status.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<Person> EditPersonStatusAsync(int id, byte status)
        {
            // Find person by using specific id.
            var person = await FindPersonAsync(id, null, null, null, StatusAccount.Active);

            // Cannot find the person.
            if (person == null)
                return null;

            var context = new OlivesHealthEntities();

            person.Status = status;
            context.People.AddOrUpdate(person);
            await context.SaveChangesAsync();
            return person;
        }

        /// <summary>
        ///     Login with specific information asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IList<Person>> LoginAsync(LoginViewModel info)
        {
            var context = new OlivesHealthEntities();

            // Retrieve account from database with specific conditions.
            var result = context.People.Where(x => x.Email.Equals(info.Email) && x.Password.Equals(info.Password));
            if (info.Role != null)
                result = result.Where(x => x.Role == info.Role.Value);

            return await result.ToListAsync();
        }

        /// <summary>
        ///     Summary person by using role.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<StatusSummaryViewModel>> SummarizePersonRoleAsync(byte? role)
        {
            var context = new OlivesHealthEntities();
            IQueryable<Person> result = context.People;

            if (role != null)
                result = result.Where(x => x.Role == role);

            var filteredResult = result.GroupBy(x => new {x.Role, x.Status})
                .OrderBy(x => x.Key)
                .Select(x => new StatusSummaryViewModel
                {
                    Role = x.Key.Role,
                    Status = x.Key.Status,
                    Total = x.Count()
                });

            return await filteredResult.ToListAsync();
        }

        /// <summary>
        ///     Initialize or update person information asynchronously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Person> InitializePersonAsync(Person info)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Add or update information base on the primary key.
            context.People.AddOrUpdate(info);

            // Save change to database.
            await context.SaveChangesAsync();

            return info;
        }

        /// <summary>
        ///     Edit person profile asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Person> EditPersonProfileAsync(int id, Person info)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Information hasn't been specified.
            if (info == null)
                throw new Exception("Personal information is required.");

            // Keep the id.
            info.Id = id;

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Add or update information base on the primary key.
                    context.People.AddOrUpdate(info);

                    #region Relationship update

                    // Because data is redundant in Relationship table, name should be changed.
                    var relationships = context.Relations.Where(x => x.Source == id || x.Target == id);

                    foreach (var relationship in relationships)
                    {
                        if (relationship.Source == id)
                        {
                            relationship.SourceFirstName = info.FirstName;
                            relationship.SourceLastName = info.LastName;
                            continue;
                        }

                        relationship.TargetFirstName = info.FirstName;
                        relationship.TargetLastName = info.LastName;
                    }

                    #endregion

                    #region Appointment update

                    var appointments = context.Appointments.Where(x => x.Maker == id || x.Dater == id);
                    foreach (var appointment in appointments)
                    {
                        if (appointment.Maker == id)
                        {
                            appointment.MakerFirstName = info.FirstName;
                            appointment.MakerLastName = info.LastName;
                            continue;
                        }

                        appointment.DaterFirstName = info.FirstName;
                        appointment.DaterLastName = info.LastName;
                    }

                    #endregion

                    #region Rating update

                    var ratings = context.Ratings.Where(x => x.Maker == id || x.Target == id);
                    foreach (var rating in ratings)
                    {
                        if (rating.Maker == id)
                        {
                            rating.MakerFirstName = info.FirstName;
                            rating.MakerLastName = info.LastName;
                            continue;
                        }

                        rating.TargetFirstName = info.FirstName;
                        rating.TargetLastName = info.LastName;
                    }

                    #endregion

                    // Save change to database.
                    await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();
                    return info;
                }
                catch
                {
                    // Error happens, transaction will be rolled back and error will be thrown to client.
                    transaction.Rollback();

                    throw;
                }
            }
        }

        #endregion

        #region Relationship

        /// <summary>
        ///     Initialize a relationship to database.
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        public async Task<Relation> InitializeRelationAsync(Relation relation)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Save relation to database.
            context.Relations.Add(relation);
            await context.SaveChangesAsync();

            return relation;
        }

        /// <summary>
        ///     Find a relation by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="person"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<Relation> FindRelationshipAsync(int id, int? person, RoleRelationship? role,
            StatusRelation? status)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Query result.
            IQueryable<Relation> relationships = context.Relations;

            #region Query

            // Filter relationship by using id.
            relationships = relationships.Where(x => x.Id == id);

            // Source is specified.
            if (person != null)
            {
                // Role role is specified.
                if (role == RoleRelationship.Source)
                    relationships = relationships.Where(x => x.Source == person.Value);
                else if (role == RoleRelationship.Target)
                    relationships = relationships.Where(x => x.Target == person.Value);
                else
                    relationships = relationships.Where(x => x.Source == person.Value || x.Target == person.Value);
            }

            // Status is specified.
            if (status != null)
                relationships = relationships.Where(x => x.Status == (byte) status);

            return await relationships.FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Find the relation between 2 people.
        /// </summary>
        /// <param name="firstPerson"></param>
        /// <param name="secondPerson"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<IList<Relation>> FindRelationshipAsync(int firstPerson, int secondPerson, byte? status = null)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the participation of people in relationships.
            var results = context.Relations.Where(
                x =>
                    (x.Source == firstPerson && x.Target == secondPerson) || x.Source == secondPerson ||
                    x.Target == firstPerson);

            // Find the status which matches with the status we wanna find.
            results = results.Where(x => x.Status == status);
            return await results.ToListAsync();
        }

        /// <summary>
        ///     Find a relation whose id match with search condition and person is taking part in it.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="person"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<IList<Relation>> FindRelationParticipation(int id, int person, byte? status)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all relationship.
            IQueryable<Relation> results = context.Relations;

            // Find the relation whose id is matched and has the specific person takes part in.
            results = results.Where(x => x.Id == id && (x.Source == id || x.Target == id));

            // Status is defined
            if (status != null)
                results = results.Where(x => x.Status == status);

            return await results.ToListAsync();
        }

        /// <summary>
        ///     Delete a relation asynchronously.
        /// </summary>
        /// <param name="id">Id of relationship</param>
        /// <param name="requester">Id of person who request to delete relationship.</param>
        /// <param name="role">The participation of requester in relationship.</param>
        /// <param name="status">Status of relationship.</param>
        /// <returns></returns>
        public async Task<int> DeleteRelationAsync(int id, int? requester, RoleRelationship? role,
            StatusRelation? status)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all relationships.
            IQueryable<Relation> relationships = context.Relations;

            // Find the relationship by using id.
            relationships = relationships.Where(x => x.Id == id);

            // Requester is defined. Find the his/her participation in the relationship.
            if (requester != null)
            {
                if (role == RoleRelationship.Source)
                    relationships = relationships.Where(x => x.Source == requester.Value);
                else if (role == RoleRelationship.Target)
                    relationships = relationships.Where(x => x.Target == requester.Value);
                else
                    relationships = relationships.Where(x => x.Source == requester || x.Target == requester);
            }

            // Status is defined.
            if (status != null)
                relationships = relationships.Where(x => x.Status == (byte) status);

            // Find the relation whose id is matched and has the specific person takes part in.
            context.Relations.RemoveRange(relationships);

            return await context.SaveChangesAsync();
        }

        /// <summary>
        ///     Filter relationship base on the role of requester.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="partner"></param>
        /// <param name="role"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        public async Task<ResponseRelationshipFilter> FilterRelationshipAsync(int requester, int? partner,
            RoleRelationship? role, TypeRelation? type, StatusRelation? status, int page, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all relationship.
            IQueryable<Relation> relationships = context.Relations;

            // In case the relationship role is defined.
            if (role == RoleRelationship.Source)
            {
                // Requester is the source of relationship.
                relationships = relationships.Where(x => x.Source == requester);

                // Therefore, partner is the target of relationship.
                if (partner != null)
                    relationships = relationships.Where(x => x.Target == partner.Value);
            }
            else if (role == RoleRelationship.Target)
            {
                // Requester is the target of relationship.
                relationships = relationships.Where(x => x.Target == requester);

                // Therefore, partner is the source of relationship.
                if (partner != null)
                    relationships = relationships.Where(x => x.Source == partner.Value);
            }
            else
                relationships = relationships.Where(x => x.Source == requester || x.Target == requester);

            // Type is defined.
            if (type != null)
                relationships = relationships.Where(x => x.Type == (byte) type.Value);

            // Status is defined.
            if (status != null)
                relationships = relationships.Where(x => x.Status == (byte) status.Value);

            // Response initialization.
            var response = new ResponseRelationshipFilter();
            response.Total = await relationships.CountAsync();

            var skippedRecord = page*records;
            response.Relationships = await relationships
                .OrderByDescending(x => x.Created)
                .Skip(skippedRecord)
                .Take(records)
                .ToListAsync();

            return response;
        }

        /// <summary>
        ///     Filter related doctors.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public async Task<ResponseRelatedDoctorFilter> FilterRelatedDoctorAsync(int requester, StatusRelation? status,
            int page, int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all relationship.
            IQueryable<Relation> relationships = context.Relations;

            // Take the relationship whose source is requester and type is provide treatment.
            relationships = relationships.Where(x => x.Source == requester && x.Type == (byte) TypeRelation.Treatment);

            // Status is defined.
            if (status != null)
                relationships = relationships.Where(x => x.Status == (byte) status.Value);

            // Take all people who are doctor.
            IQueryable<Doctor> doctors = context.Doctors;

            var fullResult = from r in relationships
                join d in doctors on r.Target equals d.Id
                select new RelatedDoctorViewModel
                {
                    Doctor = d,
                    RelationshipStatus = r.Status,
                    Created = r.Created
                };

            var response = new ResponseRelatedDoctorFilter();
            response.Total = await fullResult.CountAsync();
            response.List = await fullResult.OrderByDescending(x => x.Created)
                .Skip(page*records)
                .Take(records)
                .ToListAsync();

            return response;
        }

        /// <summary>
        ///     Filter relative by using specific conditions.
        /// </summary>
        /// <param name="requester"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public async Task<ResponseRelativeFilter> FilterRelativeAsync(int requester, StatusRelation? status, int page,
            int records)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // By default, take all relationship.
            IQueryable<Relation> relationships = context.Relations;

            // Take the relationship whose source is requester and type is provide treatment.
            relationships = relationships.Where(x => x.Source == requester && x.Type == (byte) TypeRelation.Relative);

            // Status is defined.
            if (status != null)
                relationships = relationships.Where(x => x.Status == (byte) status.Value);

            // Take all people who are doctor.
            IQueryable<Person> relatives = context.People;

            var fullResult = from r in relationships
                join relative in relatives on r.Target equals relative.Id
                select new RelativeViewModel
                {
                    Relative = relative,
                    RelationshipStatus = r.Status,
                    Created = r.Created
                };

            var response = new ResponseRelativeFilter();
            response.Total = await fullResult.CountAsync();
            response.List = await fullResult.OrderByDescending(x => x.Created)
                .Skip(page*records)
                .Take(records)
                .ToListAsync();

            return response;
        }

        #endregion

        #endregion
    }
}
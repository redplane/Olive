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
        ///     Filter patient by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="requester"></param>
        /// <returns></returns>
        public async Task<ResponsePatientFilter> FilterPatientAsync(FilterPatientViewModel filter,
            Person requester = null)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Response initialization.
            var response = new ResponsePatientFilter();

            // By default, take all patients.
            IQueryable<Patient> patients = context.Patients;

            // Requester is defined, this means only related patients can be shown up.
            if (requester != null)
            {
                // Requester's role is not a doctor.
                if (requester.Role != (byte) Role.Doctor)
                {
                    response.Patients = new List<Patient>();
                    response.Total = 0;
                    return response;
                }

                // Take all relations which requester takes part in.
                IQueryable<Relation> relationships = context.Relations;
                relationships = relationships.Where(x => x.Target == requester.Id);

                patients = from p in patients
                    join r in relationships on p.Id equals r.Source
                    select p;
            }

            // Filter doctor by using email.
            if (!string.IsNullOrEmpty(filter.Email))
                patients = patients.Where(x => x.Person.Email.Contains(filter.Email));

            // Filter doctor by using phone number.
            if (!string.IsNullOrEmpty(filter.Phone))
                patients = patients.Where(x => x.Person.Phone.Contains(filter.Phone));

            // Filter by last modified.
            if (filter.MinLastModified != null)
                patients = patients.Where(x => x.Person.LastModified >= filter.MinLastModified);

            if (filter.MaxLastModified != null)
                patients = patients.Where(x => x.Person.LastModified <= filter.MaxLastModified);

            // Filter by using name
            if (!string.IsNullOrEmpty(filter.Name))
                patients = patients.Where(x => x.Person.FullName.Contains(filter.Name));

            // Filter by using birthday.
            if (filter.MinBirthday != null)
                patients = patients.Where(x => x.Person.Birthday >= filter.MinBirthday);

            if (filter.MaxBirthday != null)
                patients = patients.Where(x => x.Person.Birthday <= filter.MaxBirthday);

            // Filter by gender.
            if (filter.Gender != null)
                patients = patients.Where(x => x.Person.Gender == filter.Gender);

            // Filter by money.
            if (filter.MinMoney != null)
                patients = patients.Where(x => x.Money >= filter.MinMoney);

            if (filter.MaxMoney != null)
                patients = patients.Where(x => x.Money <= filter.MaxMoney);

            // Filter by created.
            if (filter.MinCreated != null)
                patients = patients.Where(x => x.Person.Created >= filter.MinCreated);

            if (filter.MaxCreated != null)
                patients = patients.Where(x => x.Person.Created <= filter.MaxCreated);

            // Filter by status.
            if (filter.Status != null)
                patients = patients.Where(x => x.Person.Status == filter.Status);

            // Filter by height.
            if (filter.MinHeight != null) patients = patients.Where(x => x.Height >= filter.MinHeight);
            if (filter.MaxHeight != null) patients = patients.Where(x => x.Height <= filter.MaxHeight);

            // Filter by weight.
            if (filter.MinWeight != null) patients = patients.Where(x => x.Weight >= filter.MinWeight);
            if (filter.MaxWeight != null) patients = patients.Where(x => x.Weight <= filter.MaxWeight);

            // Caculate the total matched results.
            response.Total = await patients.CountAsync();

            // Order by status.
            patients = patients.OrderBy(x => x.Person.Status);

            // Record is specified.
            if (filter.Records != null)
            {
                patients = patients.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Take the list of filtered patient.
            response.Patients = await patients
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
                        join c in
                            context.AccountCodes.Where(
                                x => x.Code.Equals(code) && x.Type == (byte) TypeAccountCode.Activation) on p.Id equals
                            c.Owner
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

            // Filter doctor by place.
            if (!string.IsNullOrWhiteSpace(filter.City)) doctors = doctors.Where(x => x.City.Contains(filter.City));
            if (!string.IsNullOrWhiteSpace(filter.Country))
                doctors = doctors.Where(x => x.Country.Contains(filter.Country));

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

            // Response initialization.
            var responseFilter = new ResponseDoctorFilter();

            // Total matched result.
            responseFilter.Total = await results.CountAsync();

            // Sort by status.
            results = results.OrderBy(x => x.Person.Status);

            // Record is defined.
            if (filter.Records != null)
            {
                results = results.Skip(filter.Page*filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            responseFilter.Doctors = await results
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
        /// <param name="status"></param>
        /// <returns></returns>
        public Person FindPerson(int? id, string email, string password, byte? role, StatusAccount? status)
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
    }
}
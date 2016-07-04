using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Management.Instrumentation;
using System.Net.Sockets;
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
        ///     Find a patient by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IList<PatientViewModel>> FindPatientAsync(int id)
        {
            var context = new OlivesHealthEntities();
            var results = from person in context.People.Where(x => x.Id == id)
                          join patient in context.Patients.Where(x => x.Id == id) on person.Id equals patient.Id
                          where person.Id == id
                          select new PatientViewModel
                          {
                              Address = person.Address,
                              Birthday = person.Birthday,
                              Created = person.Created,
                              Email = person.Email,
                              FirstName = person.FirstName,
                              Gender = (Gender)person.Gender,
                              LastModified = person.LastModified,
                              LastName = person.LastName,
                              Money = 0,
                              Password = person.Password,
                              Phone = person.Phone,
                              Photo = person.Photo,
                              Role = person.Role,
                              Status = (StatusAccount)person.Status,
                              Height = patient.Height,
                              Weight = patient.Weight
                          };

            return await results.ToListAsync();
        }

        /// <summary>
        ///     Filter doctor by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponsePatientFilter> FilterPatientAsync(FilterPatientViewModel filter)
        {
            var context = new OlivesHealthEntities();

            // Join the table first.
            var results = context.People
                .Where(x => x.Role == (byte)Role.Patient)
                .Join(context.Patients, p => p.Id, d => d.Id,
                    (p, d) => new
                    {
                        Person = p,
                        Patient = d
                    });

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

            // Filter by using first name
            if (!string.IsNullOrEmpty(filter.FirstName))
                results = results.Where(x => x.Person.FirstName.Contains(filter.FirstName));

            // Filter by using last name.
            if (!string.IsNullOrEmpty(filter.LastName))
                results = results.Where(x => x.Person.LastName.Contains(filter.LastName));

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
                results = results.Where(x => x.Patient.Money >= filter.MinMoney);

            if (filter.MaxMoney != null)
                results = results.Where(x => x.Patient.Money <= filter.MaxMoney);

            // Filter by created.
            if (filter.MinCreated != null)
                results = results.Where(x => x.Person.Created >= filter.MinCreated);

            if (filter.MaxCreated != null)
                results = results.Where(x => x.Person.Created <= filter.MaxCreated);

            // Filter by status.
            if (filter.Status != null)
                results = results.Where(x => x.Person.Status == filter.Status);

            // Filter by height.
            if (filter.MinHeight != null) results = results.Where(x => x.Patient.Height >= filter.MinHeight);
            if (filter.MaxHeight != null) results = results.Where(x => x.Patient.Height <= filter.MaxHeight);

            // Filter by weight.
            if (filter.MinWeight != null) results = results.Where(x => x.Patient.Weight >= filter.MinWeight);
            if (filter.MaxWeight != null) results = results.Where(x => x.Patient.Weight <= filter.MaxWeight);

            #endregion

            var responseFilter = new ResponsePatientFilter();
            responseFilter.Total = await results.CountAsync();

            // How many records should be skipped.
            var skippedRecords = filter.Page * filter.Records;
            var filteredResults = results
                .OrderBy(x => x.Person.Status)
                .Skip(skippedRecords)
                .Take(filter.Records)
                .Select(x => new PatientViewModel
                {
                    Address = x.Person.Address,
                    Birthday = x.Person.Birthday,
                    Created = x.Person.Created,
                    Email = x.Person.Email,
                    FirstName = x.Person.FirstName,
                    Gender = (Gender)x.Person.Gender,
                    Id = x.Person.Id,
                    LastModified = x.Person.LastModified,
                    LastName = x.Person.LastName,
                    Money = x.Patient.Money,
                    Phone = x.Person.Phone,
                    Role = (byte)Role.Doctor,
                    Password = x.Person.Password,
                    Status = (StatusAccount)x.Person.Status,
                    Photo = x.Person.Photo,
                    Height = x.Patient.Height,
                    Weight = x.Patient.Weight
                });

            responseFilter.Users = await filteredResults.ToListAsync();
            return responseFilter;
        }

        /// <summary>
        /// Initialize a patient to database.
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public async Task<Patient> InitializePatientAsync(Patient patient)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Patients.Add(patient);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return patient;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Find and activate patient's account and remove the activation code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<bool> ActivatePatientAccount(string code)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var results = from p in context.People
                                  join c in context.ActivationCodes.Where(x => x.Code.Equals(code)) on p.Id equals c.Owner
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
                    person.Status = (byte)StatusAccount.Active;
                    context.People.AddOrUpdate(person);
                    context.ActivationCodes.Remove(activationCode);
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
        ///     Find doctor from database by using id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IList<DoctorViewModel>> FindDoctorAsync(int id)
        {
            var context = new OlivesHealthEntities();

            var results = from person in context.People.Where(x => x.Id == id)
                          join doctor in context.Doctors.Where(x => x.Id == id)
                          on person.Id equals doctor.Id
                          join specialty in context.Specialties on doctor.SpecialtyId equals specialty.Id
                          where person.Id == id
                          select new DoctorViewModel
                          {
                              Address = person.Address,
                              Birthday = person.Birthday,
                              Created = person.Created,
                              Email = person.Email,
                              FirstName = person.FirstName,
                              Gender = (Gender)person.Gender,
                              LastModified = person.LastModified,
                              LastName = person.LastName,
                              Money = 0,
                              Password = person.Password,
                              Phone = person.Phone,
                              Photo = person.Photo,
                              Rank = doctor.Rank ?? 0,
                              Role = person.Role,
                              Specialty = new SpecialtyViewModel()
                              {
                                  Id = specialty.Id,
                                  Name = specialty.Name
                              },
                              City = new CityViewModel()
                              {
                                  Id = doctor.City.Id,
                                  Name = doctor.City.Name,
                                  Country = new CountryViewModel()
                                  {
                                      Id = doctor.City.CountryId,
                                      Name = doctor.City.CountryName
                                  }
                              },
                              Status = (StatusAccount) person.Status,
                              Voters = doctor.Voters
                          };

            return await results.ToListAsync();
        }

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
            IQueryable<Person> people = context.People.Where(x => x.Role == (byte)Role.Doctor);

            // By default, take all doctors.
            IQueryable<Doctor> doctors = context.Doctors;

            #region People filter

            // Filter doctor by using email.
            if (!string.IsNullOrEmpty(filter.Email))
                people = people.Where(x => x.Email.Contains(filter.Email));

            // Filter doctor by using phone number.
            if (!string.IsNullOrEmpty(filter.Phone))
                people = people.Where(x => x.Phone.Contains(filter.Phone));

            // Filter by using first name
            if (!string.IsNullOrEmpty(filter.FirstName))
                people = people.Where(x => x.FirstName.Contains(filter.FirstName));

            // Filter by using last name.
            if (!string.IsNullOrEmpty(filter.LastName))
                people = people.Where(x => x.LastName.Contains(filter.LastName));
            
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
            var results = (from p in people
                           join d in doctors on p.Id equals d.Id
                           join s in specialties on d.SpecialtyId equals s.Id
                           select new
                           {
                               Person = p,
                               Doctor = d,
                               TrainedSpecialty = s
                           });
            
            var responseFilter = new ResponseDoctorFilter();
            responseFilter.Total = await results.CountAsync();

            // How many records should be skipped.
            var skippedRecords = filter.Page * filter.Records;
            var filteredResults = results
                .OrderBy(x => x.Person.Status)
                .Skip(skippedRecords)
                .Take(filter.Records)
                .Select(x => new DoctorViewModel
                {
                    Address = x.Person.Address,
                    Birthday = x.Person.Birthday,
                    Created = x.Person.Created,
                    Email = x.Person.Email,
                    FirstName = x.Person.FirstName,
                    Gender = (Gender) x.Person.Gender,
                    Id = x.Person.Id,
                    LastModified = x.Person.LastModified,
                    LastName = x.Person.LastName,
                    Money = x.Doctor.Money,
                    Rank = x.Doctor.Rank ?? 0,
                    Phone = x.Person.Phone,
                    Role = (byte)Role.Doctor,
                    Password = x.Person.Password,
                    Status = (StatusAccount)x.Person.Status,
                    Photo = x.Person.Photo,
                    Voters = x.Doctor.Voters,
                    Specialty = new SpecialtyViewModel()
                    {
                        Id = x.TrainedSpecialty.Id,
                        Name = x.TrainedSpecialty.Name
                    },
                    City = new CityViewModel()
                    {
                        Id = x.Doctor.City.Id,
                        Name = x.Doctor.City.Name,
                        Country = new CountryViewModel()
                        {
                            Id = x.Doctor.City.CountryId,
                            Name = x.Doctor.City.CountryName
                        }
                    }
                });

            responseFilter.Users = await filteredResults.ToListAsync();
            return responseFilter;
        }

        /// <summary>
        /// Initialize a doctor asynchronously.
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
        public async Task<Person> FindPersonAsync(int? id, string email, string password, byte? role, StatusAccount? status)
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
                var castedStatus = (byte)status;
                result = result.Where(x => x.Status == castedStatus);
            }

            return await result.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Find a person asynchronously by using activation code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<Person> FindPersonAsync(string code)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the person whose activation code matches the condition.
            var results = from p in context.People
                          join a in context.ActivationCodes.Where(x => x.Code.Equals(code)) on p.Id equals a.Owner
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
        public async Task<IList<StatusSummaryViewModel>> SummarizePersonRole(byte? role)
        {
            var context = new OlivesHealthEntities();
            IQueryable<Person> result = context.People;

            if (role != null)
                result = result.Where(x => x.Role == role);

            var filteredResult = result.GroupBy(x => new { x.Role, x.Status })
                .OrderBy(x => x.Key)
                .Select(x => new StatusSummaryViewModel
                {
                    Role = x.Key.Role,
                    Status = x.Key.Status,
                    Total = x.Count()
                });

            return await filteredResult.ToListAsync();
        }

        #endregion

        #region Relation

        /// <summary>
        /// Initialize a relationship to database.
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
        /// Find a relation by using specific information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<IList<Relation>> FindRelation(int? id, int? source, int? target, byte? type)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Condition has bee specified or not.
            var conditionSpecified = false;

            // Query result.
            IQueryable<Relation> results = context.Relations;

            #region Query

            // Id is specified.
            if (id != null)
            {
                results = context.Relations.Where(x => x.Id == id);
                conditionSpecified = true;
            }

            // Source is specified.
            if (source != null)
            {
                results = context.Relations.Where(x => x.Source == source);
                conditionSpecified = true;
            }

            // Target is specified.
            if (target != null)
            {
                results = context.Relations.Where(x => x.Target == target);
                conditionSpecified = true;
            }

            // Type is specified.
            if (type != null)
            {
                results = context.Relations.Where(x => x.Type == type);
                conditionSpecified = true;
            }

            #endregion

            // No condition is specified.
            if (!conditionSpecified)
                return null;

            return await results.ToListAsync();
        }

        /// <summary>
        /// Find the relation between 2 people.
        /// </summary>
        /// <param name="firstPerson"></param>
        /// <param name="secondPerson"></param>
        /// <returns></returns>
        public async Task<IList<Relation>> FindRelation(int firstPerson, int secondPerson)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            return await context.Relations.Where(
                    x =>
                        (x.Source == firstPerson && x.Target == secondPerson) ||
                        (x.Source == secondPerson || x.Target == firstPerson)).ToListAsync();
        }

        /// <summary>
        /// Find a relation whose id match with search condition and person is taking part in it.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="person"></param>
        /// <returns></returns>
        public async Task<IList<Relation>> FindRelationParticipation(int id, int person)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Find the relation whose id is matched and has the specific person takes part in.
            var result = context.Relations.Where(x => x.Id == id && (x.Source == id || x.Target == id));

            return await result.ToListAsync();
        }

        /// <summary>
        /// Delete a relation asynchronously.
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRelationAsync(Relation relation)
        {
            try
            {
                // Database context initialization.
                var context = new OlivesHealthEntities();

                // Find the relation whose id is matched and has the specific person takes part in.
                context.Relations.Remove(relation);

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
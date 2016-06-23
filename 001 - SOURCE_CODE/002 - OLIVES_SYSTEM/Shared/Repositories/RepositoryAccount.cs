﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels;
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
            var results = from person in context.People
                join patient in context.Patients on person.Email equals patient.Email
                where person.Id == id
                select new PatientViewModel
                {
                    Address = person.Address,
                    Birthday = person.Birthday,
                    Created = person.Created,
                    Email = person.Email,
                    FirstName = person.FirstName,
                    Gender = person.Gender,
                    LastModified = person.LastModified,
                    LastName = person.LastName,
                    Latitude = person.Latitude,
                    Longitude = person.Longitude,
                    Money = 0,
                    Password = person.Password,
                    Phone = person.Phone,
                    Photo = person.Photo,
                    Role = person.Role,
                    Status = person.Status,
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
                .Where(x => x.Role == AccountRole.Patient)
                .Join(context.Patients, p => p.Email, d => d.Email,
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
            var skippedRecords = filter.Page*filter.Records;
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
                    Gender = x.Person.Gender,
                    Id = x.Person.Id,
                    LastModified = x.Person.LastModified,
                    Latitude = x.Person.Latitude,
                    LastName = x.Person.LastName,
                    Money = x.Patient.Money,
                    Phone = x.Person.Phone,
                    Role = AccountRole.Doctor,
                    Longitude = x.Person.Longitude,
                    Password = x.Person.Password,
                    Status = x.Person.Status,
                    Photo = x.Person.Photo,
                    Height = x.Patient.Height,
                    Weight = x.Patient.Weight
                });

            responseFilter.Users = await filteredResults.ToListAsync();
            return responseFilter;
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

            var results = from person in context.People
                join doctor in context.Doctors on person.Email equals doctor.Email
                join specialty in context.Specialties on doctor.SpecialtyId equals specialty.Id
                where person.Id == id
                select new DoctorViewModel
                {
                    Address = person.Address,
                    Birthday = person.Birthday,
                    Created = person.Created,
                    Email = person.Email,
                    FirstName = person.FirstName,
                    Gender = person.Gender,
                    LastModified = person.LastModified,
                    LastName = person.LastName,
                    Latitude = person.Latitude,
                    Longitude = person.Longitude,
                    Money = 0,
                    Password = person.Password,
                    Phone = person.Phone,
                    Photo = person.Photo,
                    Rank = doctor.Rank ?? 0,
                    Role = person.Role,
                    Specialty = specialty.Name,
                    Status = person.Status,
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

            // Join the tables first.
            var results = (from p in context.People 
                          join d in context.Doctors on p.Email equals d.Email
                          join s in context.Specialties on d.SpecialtyId equals s.Id
                          select new
                          {
                              Person = p,
                              Doctor = d,
                              TrainedSpecialty = s
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
                results = results.Where(x => x.Doctor.Money >= filter.MinMoney);

            if (filter.MaxMoney != null)
                results = results.Where(x => x.Doctor.Money <= filter.MaxMoney);

            // Filter by created.
            if (filter.MinCreated != null)
                results = results.Where(x => x.Person.Created >= filter.MinCreated);

            if (filter.MaxCreated != null)
                results = results.Where(x => x.Person.Created <= filter.MaxCreated);

            // Filter by status.
            if (filter.Status != null)
                results = results.Where(x => x.Person.Status == filter.Status);

            // Filter by role.
            results = results.Where(x => x.Person.Role == AccountRole.Doctor);

            // Filter by rank.
            if (filter.MinRank != null) results = results.Where(x => x.Doctor.Rank >= filter.MinRank);
            if (filter.MaxRank != null) results = results.Where(x => x.Doctor.Rank <= filter.MaxRank);

            #endregion

            var responseFilter = new ResponseDoctorFilter();
            responseFilter.Total = await results.CountAsync();

            // How many records should be skipped.
            var skippedRecords = filter.Page*filter.Records;
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
                    Gender = x.Person.Gender,
                    Id = x.Person.Id,
                    LastModified = x.Person.LastModified,
                    Latitude = x.Person.Latitude,
                    LastName = x.Person.LastName,
                    Money = x.Doctor.Money,
                    Rank = x.Doctor.Rank ?? 0,
                    Phone = x.Person.Phone,
                    Role = AccountRole.Doctor,
                    Longitude = x.Person.Longitude,
                    Password = x.Person.Password,
                    Status = x.Person.Status,
                    Photo = x.Person.Photo,
                    Voters = x.Doctor.Voters,
                    Specialty = x.TrainedSpecialty.Name
                });

            responseFilter.Users = await filteredResults.ToListAsync();
            return responseFilter;
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
        /// <returns></returns>
        public async Task<Person> FindPersonAsync(int? id, string email, string password, byte? role)
        {
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

            return await result.FirstOrDefaultAsync();
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
            var person = await FindPersonAsync(id, null, null, null);

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

        #endregion
    }
}
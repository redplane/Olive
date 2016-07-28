using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.ViewModels;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Response;

namespace OlivesAdministration.Test.Repositories
{
    public class RepositoryAccount : IRepositoryAccount
    {
        #region Constructor

        /// <summary>
        ///     Initialize an instance of account with default settings.
        /// </summary>
        public RepositoryAccount()
        {
            // Initialize a list of account into system.
            People = new List<Person>();

            // Initialize a list of doctors into system.
            Doctors = new List<Doctor>();
        }

        #endregion

        public Task<Person> EditPersonProfileAsync(int id, Person info)
        {
            throw new NotImplementedException();
        }

        public Task<Person> EditPersonStatusAsync(int id, byte status)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDoctorFilter> FilterDoctorAsync(FilterDoctorViewModel filter)
        {
            IEnumerable<Person> people = new List<Person>(People);
            IEnumerable<Doctor> doctors = new List<Doctor>(Doctors);
            IEnumerable<Specialty> specialties = new List<Specialty>(Specialties);

                // Take all people from database.
            people = people.Where(x => x.Role == (byte)Role.Doctor);
            
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
            responseFilter.Total = results.Count();

            // Sort by status.
            results = results.OrderBy(x => x.Person.Status);

            // Record is defined.
            if (filter.Records != null)
            {
                results = results.Skip(filter.Page * filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            responseFilter.Doctors = results.ToList();

            return responseFilter;
        }

        public async Task<Doctor> FindDoctorAsync(int id, StatusAccount? status)
        {
            IEnumerable<Doctor> doctors = new List<Doctor>(Doctors);

            // Find doctors by using id.
            doctors = doctors.Where(x => x.Id == id);

            // Status is defined.
            if (status != null)
                doctors = doctors.Where(x => x.Person.Status == (byte) status);

            return doctors.FirstOrDefault();
        }

        public Task<Patient> FindPatientAsync(int id, byte? status)
        {
            throw new NotImplementedException();
        }

        public Person FindPerson(int? id, string email, string password, byte? role, StatusAccount? status)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Find a person with specific conditions asynchronously.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<Person> FindPersonAsync(string code)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Find a person with specific conditions asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<Person> FindPersonAsync(int? id, string email, string password, byte? role,
            StatusAccount? status)
        {
            // By default, take all people in database.
            IEnumerable<Person> result = new List<Person>(People);

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

            // Status is specified.
            if (status != null)
                result = result.Where(x => x.Status == (byte) status);

            return result.FirstOrDefault();
        }

        public Task<bool> InitializePatientActivation(string code)
        {
            throw new NotImplementedException();
        }

        public Task<Person> InitializePersonAsync(Person info)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Person>> LoginAsync(LoginViewModel info)
        {
            throw new NotImplementedException();
        }

        public Task<IList<StatusSummaryViewModel>> SummarizePersonRoleAsync(byte? role)
        {
            throw new NotImplementedException();
        }

        public Task<ResponsePatientFilter> FilterPatientAsync(FilterPatientViewModel filter, Person requester = null)
        {
            throw new NotImplementedException();
        }

        public Task<ResponsePatientFilter> FilterPatientAsync(FilterPatientViewModel filter)
        {
            throw new NotImplementedException();
        }

        #region Properties

        /// <summary>
        ///     List of account in system.
        /// </summary>
        public List<Person> People { get; set; }

        /// <summary>
        ///     List of doctors in system.
        /// </summary>
        public List<Doctor> Doctors { get; set; }
        
        /// <summary>
        /// List of specialties.
        /// </summary>
        public List<Specialty> Specialties { get; set; } 

        #endregion
    }
}
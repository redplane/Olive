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
        #region Properties

        /// <summary>
        /// List of account in system.
        /// </summary>
        public List<Person> People { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize an instance of account with default settings.
        /// </summary>
        public RepositoryAccount()
        {
            // Initialize a list of account into system.
            People = new List<Person>();
        }

        #endregion

        public Task<int> DeleteRelationAsync(int id, int? requester, RoleRelationship? role, StatusRelation? status)
        {
            throw new NotImplementedException();
        }

        public Task<Person> EditPersonProfileAsync(int id, Person info)
        {
            throw new NotImplementedException();
        }

        public Task<Person> EditPersonStatusAsync(int id, byte status)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDoctorFilter> FilterDoctorAsync(FilterDoctorViewModel filter)
        {
            throw new NotImplementedException();
        }

        public Task<ResponsePatientFilter> FilterPatientAsync(FilterPatientViewModel filter)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseRelatedDoctorFilter> FilterRelatedDoctorAsync(int requester, StatusRelation? status, int page, int records)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseRelationshipFilter> FilterRelationshipAsync(int requester, int? partner, RoleRelationship? role, StatusRelation? status, int page, int records)
        {
            throw new NotImplementedException();
        }

        public Task<Doctor> FindDoctorAsync(int id, StatusAccount? status)
        {
            throw new NotImplementedException();
        }

        public Task<Patient> FindPatientAsync(int id, byte? status)
        {
            throw new NotImplementedException();
        }

        public Person FindPerson(int? id, string email, string password, byte? role)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find a person with specific conditions asynchronously.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<Person> FindPersonAsync(string code)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find a person with specific conditions asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<Person> FindPersonAsync(int? id, string email, string password, byte? role, StatusAccount? status)
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

        public Task<IList<Relation>> FindRelationParticipation(int id, int person, byte? status)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Relation>> FindRelationshipAsync(int firstPerson, int secondPerson, byte? status)
        {
            throw new NotImplementedException();
        }

        public Task<Relation> FindRelationshipAsync(int id, int? person, RoleRelationship? role, StatusRelation? status)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InitializePatientActivation(string code)
        {
            throw new NotImplementedException();
        }

        public Task<Person> InitializePersonAsync(Person info)
        {
            throw new NotImplementedException();
        }

        public Task<Relation> InitializeRelationAsync(Relation relation)
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
    }
}
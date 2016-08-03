using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using OlivesAdministration.Enumerations;
using OlivesAdministration.Interfaces;
using OlivesAdministration.ViewModels.Filter;
using OlivesAdministration.ViewModels.Statistic;
using Shared.Enumerations;
using Shared.Models;
using Shared.Repositories;
using Shared.ViewModels.Response;

namespace OlivesAdministration.Repositories
{
    public class RepositoryAccountExtended : RepositoryAccount, IRepositoryAccountExtended
    {
        /// <summary>
        /// Filter doctors by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseDoctorFilter> FilterDoctorsAsync(FilterDoctorViewModel filter)
        {
            #region Data initialization

            // Database connection context initialization.
            var context = new OlivesHealthEntities();
            
            // By default, take all doctors.
            IQueryable<Doctor> doctors = context.Doctors;

            #endregion

            #region People filter

            // Filter doctor by using email.
            if (!string.IsNullOrEmpty(filter.Email))
                doctors = doctors.Where(x => x.Person.Email.Contains(filter.Email));

            // Filter doctor by using phone number.
            if (!string.IsNullOrEmpty(filter.Phone))
                doctors = doctors.Where(x => x.Person.Phone.Contains(filter.Phone));

            // Filter by using full name.
            if (!string.IsNullOrEmpty(filter.Name))
                doctors = doctors.Where(x => x.Person.FullName.Contains(filter.Name));

            // Filter by using birthday.
            if (filter.MinBirthday != null)
                doctors = doctors.Where(x => x.Person.Birthday >= filter.MinBirthday);
            if (filter.MaxBirthday != null)
                doctors = doctors.Where(x => x.Person.Birthday <= filter.MaxBirthday);

            // Filter by gender.
            if (filter.Gender != null)
                doctors = doctors.Where(x => x.Person.Gender == filter.Gender);

            // Filter by last modified.
            if (filter.MinLastModified != null)
                doctors = doctors.Where(x => x.Person.LastModified >= filter.MinLastModified);
            if (filter.MaxLastModified != null)
                doctors = doctors.Where(x => x.Person.LastModified <= filter.MaxLastModified);

            // Filter by created.
            if (filter.MinCreated != null)
                doctors = doctors.Where(x => x.Person.Created >= filter.MinCreated);
            if (filter.MaxCreated != null)
                doctors = doctors.Where(x => x.Person.Created <= filter.MaxCreated);

            // Filter by status.
            if (filter.Status != null)
                doctors = doctors.Where(x => x.Person.Status == (byte)filter.Status);

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

            #region Response initialization

            // Response initialization.
            var responseFilter = new ResponseDoctorFilter();

            // Total matched result.
            responseFilter.Total = await doctors.CountAsync();

            #endregion

            #region Result sort

            switch (filter.Direction)
            {
                case SortDirection.Decending:
                    switch (filter.Sort)
                    {
                        case FilterDoctorSort.Birthday:
                            doctors = doctors.OrderByDescending(x => x.Person.Birthday);
                            break;
                        case FilterDoctorSort.Created:
                            doctors = doctors.OrderByDescending(x => x.Person.Created);
                            break;
                        case FilterDoctorSort.FirstName:
                            doctors = doctors.OrderByDescending(x => x.Person.FirstName);
                            break;
                        case FilterDoctorSort.Gender:
                            doctors = doctors.OrderByDescending(x => x.Person.Gender);
                            break;
                        case FilterDoctorSort.LastModified:
                            doctors = doctors.OrderByDescending(x => x.Person.LastModified);
                            break;
                        case FilterDoctorSort.LastName:
                            doctors = doctors.OrderByDescending(x => x.Person.LastName);
                            break;
                        case FilterDoctorSort.Money:
                            doctors = doctors.OrderByDescending(x => x.Money);
                            break;
                        case FilterDoctorSort.Status:
                            doctors = doctors.OrderByDescending(x => x.Person.Status);
                            break;
                        case FilterDoctorSort.Rank:
                            doctors = doctors.OrderByDescending(x => x.Rank);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case FilterDoctorSort.Birthday:
                            doctors = doctors.OrderBy(x => x.Person.Birthday);
                            break;
                        case FilterDoctorSort.Created:
                            doctors = doctors.OrderBy(x => x.Person.Created);
                            break;
                        case FilterDoctorSort.FirstName:
                            doctors = doctors.OrderBy(x => x.Person.FirstName);
                            break;
                        case FilterDoctorSort.Gender:
                            doctors = doctors.OrderBy(x => x.Person.Gender);
                            break;
                        case FilterDoctorSort.LastModified:
                            doctors = doctors.OrderBy(x => x.Person.LastModified);
                            break;
                        case FilterDoctorSort.LastName:
                            doctors = doctors.OrderBy(x => x.Person.LastName);
                            break;
                        case FilterDoctorSort.Money:
                            doctors = doctors.OrderBy(x => x.Money);
                            break;
                        case FilterDoctorSort.Status:
                            doctors = doctors.OrderBy(x => x.Person.Status);
                            break;
                        case FilterDoctorSort.Rank:
                            doctors = doctors.OrderBy(x => x.Rank);
                            break;
                    }
                    break;
            }

            #endregion

            #region Result handling

            // Record is defined.
            if (filter.Records != null)
            {
                doctors = doctors.Skip(filter.Page * filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            responseFilter.Doctors = doctors;
            return responseFilter;

            #endregion
        }

        /// <summary>
        ///     Filter patient by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponsePatientFilter> FilterPatientsAsync(FilterPatientViewModel filter)
        {
            #region Record filter

            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Response initialization.
            var response = new ResponsePatientFilter();

            // By default, take all patients.
            IQueryable<Patient> patients = context.Patients;

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

            #endregion

            #region Result sorting

            switch (filter.Direction)
            {
                case SortDirection.Decending:
                    switch (filter.Sort)
                    {
                        case FilterPatientSort.FirstName:
                            patients = patients.OrderByDescending(x => x.Person.FirstName);
                            break;
                        case FilterPatientSort.LastName:
                            patients = patients.OrderByDescending(x => x.Person.LastName);
                            break;
                        case FilterPatientSort.Created:
                            patients = patients.OrderByDescending(x => x.Person.Created);
                            break;
                        case FilterPatientSort.LastModified:
                            patients = patients.OrderByDescending(x => x.Person.LastModified);
                            break;
                        case FilterPatientSort.Birthday:
                            patients = patients.OrderByDescending(x => x.Person.Birthday);
                            break;
                        case FilterPatientSort.Gender:
                            patients = patients.OrderByDescending(x => x.Person.Gender);
                            break;
                        case FilterPatientSort.Money:
                            patients = patients.OrderByDescending(x => x.Money);
                            break;
                        case FilterPatientSort.Status:
                            patients = patients.OrderByDescending(x => x.Person.Status);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case FilterPatientSort.FirstName:
                            patients = patients.OrderBy(x => x.Person.FirstName);
                            break;
                        case FilterPatientSort.LastName:
                            patients = patients.OrderBy(x => x.Person.LastName);
                            break;
                        case FilterPatientSort.Created:
                            patients = patients.OrderBy(x => x.Person.Created);
                            break;
                        case FilterPatientSort.LastModified:
                            patients = patients.OrderBy(x => x.Person.LastModified);
                            break;
                        case FilterPatientSort.Birthday:
                            patients = patients.OrderBy(x => x.Person.Birthday);
                            break;
                        case FilterPatientSort.Gender:
                            patients = patients.OrderBy(x => x.Person.Gender);
                            break;
                        case FilterPatientSort.Money:
                            patients = patients.OrderBy(x => x.Money);
                            break;
                        case FilterPatientSort.Status:
                            patients = patients.OrderBy(x => x.Person.Status);
                            break;
                    }
                    break;
            }

            #endregion

            #region Result handling

            // Record is specified.
            if (filter.Records != null)
            {
                patients = patients.Skip(filter.Page * filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            // Take the list of filtered patient.
            response.Patients = patients;
            return response;

            #endregion
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
        
    }
}
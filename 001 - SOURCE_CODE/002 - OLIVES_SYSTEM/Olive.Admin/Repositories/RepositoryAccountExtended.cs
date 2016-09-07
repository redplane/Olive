using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olive.Admin.Enumerations;
using Olive.Admin.Interfaces;
using Olive.Admin.ViewModels;
using Olive.Admin.ViewModels.Filter;
using Olive.Admin.ViewModels.Filter.Response;
using Olive.Admin.ViewModels.Statistic;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Repositories;
using Shared.ViewModels.Response;

namespace Olive.Admin.Repositories
{
    public class RepositoryAccountExtended : RepositoryAccount, IRepositoryAccountExtended
    {
        #region Properties

        private readonly IOliveDataContext _dataContext;

        #endregion

        #region Constructors

        public RepositoryAccountExtended(IOliveDataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Filter doctors by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponseFilterDoctorViewModel> FilterDoctorsAsync(FilterDoctorViewModel filter)
        {
            #region Data initialization

            // By default, take all doctors.
            var context = _dataContext.Context;

            // Take all the people in the system.
            IQueryable<Person> people = context.People;

            // Take all the doctors in the system.
            people = people.Where(x => x.Role == (byte)Role.Doctor);
            IQueryable<Doctor> doctors = context.Doctors;

            // Take all places.
            IQueryable<Place> places = context.Places;

            // Take all specialties information.
            IQueryable<Specialty> specialties = context.Specialties;

            #endregion

            #region People filter

            // Filter doctor by using email.
            if (!string.IsNullOrEmpty(filter.Email))
                people = people.Where(x => x.Email.Contains(filter.Email));

            // Filter doctor by using phone number.
            if (!string.IsNullOrEmpty(filter.Phone))
                people = people.Where(x => x.Phone.Contains(filter.Phone));

            // Filter by using birthday.
            if (filter.MinBirthday != null)
                people = people.Where(x => x.Birthday >= filter.MinBirthday);
            if (filter.MaxBirthday != null)
                people = people.Where(x => x.Birthday <= filter.MaxBirthday);

            // Filter by gender.
            //if (filter.Genders != null && filter.Genders.Count > 0)
            //    people = people.Where(x => x.Gender == (byte)filter.Gender);

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
            if (filter.Statuses != null)
            {
                var statuses = new List<byte>(filter.Statuses);
                people = from person in people
                         where statuses.Contains(person.Status)
                         select person;
            }

            #endregion

            #region Doctors

            // Filter doctor by city.
            if (!string.IsNullOrWhiteSpace(filter.City))
                places = places.Where(x => x.City.Contains(filter.City));

            // Filter doctor by country
            if (!string.IsNullOrWhiteSpace(filter.Country))
                places = places.Where(x => x.Country.Contains(filter.Country));

            // Filter by rank.
            if (filter.MinRank != null) doctors = doctors.Where(x => x.Rank >= filter.MinRank);
            if (filter.MaxRank != null) doctors = doctors.Where(x => x.Rank <= filter.MaxRank);

            // Filter by id of specialty.
            if (filter.Specialty != null)
                specialties = specialties.Where(x => x.Id == filter.Specialty.Value);

            #endregion

            #region Result sort

            switch (filter.Direction)
            {
                case SortDirection.Decending:
                    switch (filter.Sort)
                    {
                        case FilterDoctorSort.Birthday:
                            people = people.OrderByDescending(x => x.Birthday);
                            break;
                        case FilterDoctorSort.Created:
                            people = people.OrderByDescending(x => x.Created);
                            break;
                        case FilterDoctorSort.FirstName:
                            people = people.OrderByDescending(x => x.FirstName);
                            break;
                        case FilterDoctorSort.Gender:
                            people = people.OrderByDescending(x => x.Gender);
                            break;
                        case FilterDoctorSort.LastModified:
                            people = people.OrderByDescending(x => x.LastModified);
                            break;
                        case FilterDoctorSort.LastName:
                            people = people.OrderByDescending(x => x.LastName);
                            break;
                        case FilterDoctorSort.Status:
                            people = people.OrderByDescending(x => x.Status);
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
                            people = people.OrderBy(x => x.Birthday);
                            break;
                        case FilterDoctorSort.Created:
                            people = people.OrderBy(x => x.Created);
                            break;
                        case FilterDoctorSort.FirstName:
                            people = people.OrderBy(x => x.FirstName);
                            break;
                        case FilterDoctorSort.Gender:
                            people = people.OrderBy(x => x.Gender);
                            break;
                        case FilterDoctorSort.LastModified:
                            people = people.OrderBy(x => x.LastModified);
                            break;
                        case FilterDoctorSort.LastName:
                            people = people.OrderBy(x => x.LastName);
                            break;
                        case FilterDoctorSort.Status:
                            people = people.OrderBy(x => x.Status);
                            break;
                        case FilterDoctorSort.Rank:
                            doctors = doctors.OrderBy(x => x.Rank);
                            break;
                    }
                    break;
            }

            #endregion

            #region Join & handle results

            var filteredResult = from doctor in doctors
                                  from person in people
                                  from place in places
                                  from specialty in specialties
                                  where doctor.Id == person.Id && doctor.PlaceId == place.Id && doctor.SpecialtyId == specialty.Id
                                  select new DoctorViewModel
                                  {
                                      Id = person.Id,
                                      Email = person.Email,
                                      FirstName = person.FirstName,
                                      LastName = person.LastName,
                                      Birthday = person.Birthday,
                                      Phone = person.Phone,
                                      Gender = person.Gender ?? (byte)Gender.Male,
                                      Created = person.Created,
                                      Status = person.Status,
                                      Address = person.Address,
                                      PhotoUrl = person.PhotoUrl,
                                      Rank = doctor.Rank ?? 0,
                                      Specialty = new SpecialtyViewModel
                                      {
                                          Id = specialty.Id,
                                          Name = specialty.Name
                                      },
                                      Voters = doctor.Voters,
                                      Place = new PlaceViewModel
                                      {
                                          Id = place.Id,
                                          City = place.City,
                                          Country = place.Country
                                      },
                                      ProfileUrl = doctor.ProfileUrl
                                  };

            #endregion

            #region Response initialization

            // Response initialization.
            var responseFilterDoctorViewModel = new ResponseFilterDoctorViewModel();

            // Total matched result.
            responseFilterDoctorViewModel.Total = await filteredResult.CountAsync();

            #endregion
            
            #region Result handling

            // Record is defined.
            if (filter.Records != null)
            {
                filteredResult = filteredResult.Skip(filter.Page * filter.Records.Value)
                    .Take(filter.Records.Value);
            }

            responseFilterDoctorViewModel.Doctors = await filteredResult.ToListAsync();
            return responseFilterDoctorViewModel;

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

            // Response initialization.
            var response = new ResponsePatientFilter();

            var context = _dataContext.Context;
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
                patients = patients.Where(x => x.Person.Gender == (byte)filter.Gender);

            // Filter by created.
            if (filter.MinCreated != null)
                patients = patients.Where(x => x.Person.Created >= filter.MinCreated);

            if (filter.MaxCreated != null)
                patients = patients.Where(x => x.Person.Created <= filter.MaxCreated);

            // Filter by status.
            if (filter.Status != null)
                patients = patients.Where(x => x.Person.Status == (byte)filter.Status);

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
            response.Patients = await patients.ToListAsync();
            return response;

            #endregion
        }

        /// <summary>
        ///     Summary person by using role.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<StatusSummaryViewModel>> SummarizePersonRoleAsync(byte? role)
        {
            var context = _dataContext.Context;
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

        /// <summary>
        ///     Edit person profile asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<Person> EditPersonProfileAsync(int id, Person info)
        {
            // Information hasn't been specified.
            if (info == null)
                throw new Exception("Personal information is required.");

            // Keep the id.
            info.Id = id;

            var context = _dataContext.Context;
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Add or update information base on the primary key.
                    context.People.AddOrUpdate(info);

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
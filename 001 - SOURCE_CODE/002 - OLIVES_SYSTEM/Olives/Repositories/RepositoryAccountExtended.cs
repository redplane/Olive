using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using Olives.Enumerations.Filter;
using Olives.Interfaces;
using Olives.ViewModels.Filter;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Repositories;
using Shared.ViewModels.Response;

namespace Olives.Repositories
{
    public class RepositoryAccountExtended : RepositoryAccount, IRepositoryAccountExtended
    {
        /// <summary>
        /// Filter doctor asynchronously with specific conditions.
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
            
            // Filter by status.
            if (filter.Status != null)
                doctors = doctors.Where(x => x.Person.Status == (byte)filter.Status);

            #endregion

            #region Doctors

            // Filter doctor by place.
            if (!string.IsNullOrWhiteSpace(filter.City)) doctors = doctors.Where(x => x.Place.City.Contains(filter.City));
            if (!string.IsNullOrWhiteSpace(filter.Country))
                doctors = doctors.Where(x => x.Place.Country.Contains(filter.Country));
            
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
                        case FilterDoctorSort.FirstName:
                            doctors = doctors.OrderByDescending(x => x.Person.FirstName);
                            break;
                        case FilterDoctorSort.Gender:
                            doctors = doctors.OrderByDescending(x => x.Person.Gender);
                            break;
                        case FilterDoctorSort.LastName:
                            doctors = doctors.OrderByDescending(x => x.Person.LastName);
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
                        case FilterDoctorSort.FirstName:
                            doctors = doctors.OrderBy(x => x.Person.FirstName);
                            break;
                        case FilterDoctorSort.Gender:
                            doctors = doctors.OrderBy(x => x.Person.Gender);
                            break;
                        case FilterDoctorSort.LastName:
                            doctors = doctors.OrderBy(x => x.Person.LastName);
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
        /// Filter patients by using specific conditions
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<ResponsePatientFilter> FilterPatientsAsync(FilterPatientViewModel filter)
        {
            #region Result filter

            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Response initialization.
            var response = new ResponsePatientFilter();

            // Take all relations which requester takes part in.
            IQueryable<Relation> relationships = context.Relations;
            relationships = relationships.Where(x => x.Source == filter.Requester || x.Target == filter.Requester);
            relationships = relationships.Where(x => x.Status == (byte)StatusRelation.Active);

            // By default, take all patients.
            IQueryable<Patient> patients = context.Patients;
            patients = patients.Where(x => x.Id != filter.Requester);

            // Filter doctor by using email.
            if (!string.IsNullOrEmpty(filter.Email))
                patients = patients.Where(x => x.Person.Email.Contains(filter.Email));

            // Filter doctor by using phone number.
            if (!string.IsNullOrEmpty(filter.Phone))
                patients = patients.Where(x => x.Person.Phone.Contains(filter.Phone));

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

            // Only filter the active patient.
            patients = patients.Where(x => x.Person.Status == (byte)StatusAccount.Active);

            // Relationship connection.
            patients = from p in patients
                       from r in relationships
                       where (r.Source == p.Id && r.Target == filter.Requester) || (r.Source == filter.Requester && r.Target == p.Id)
                       select p;

            // Caculate the total matched results.
            response.Total = await patients.CountAsync();

            #endregion

            #region Result sort

            switch (filter.Direction)
            {
                case SortDirection.Decending:
                    switch (filter.Sort)
                    {
                            case PatientFilterSort.Email:
                            patients = patients.OrderByDescending(x => x.Person.Email);
                            break;
                            case PatientFilterSort.Phone:
                            patients = patients.OrderByDescending(x => x.Person.Phone);
                            break;
                        case PatientFilterSort.FirstName:
                            patients = patients.OrderByDescending(x => x.Person.FirstName);
                            break;
                        case PatientFilterSort.LastName:
                            patients = patients.OrderByDescending(x => x.Person.LastName);
                            break;
                        case PatientFilterSort.Birthday:
                            patients = patients.OrderByDescending(x => x.Person.Birthday);
                            break;
                        case PatientFilterSort.Gender:
                            patients = patients.OrderByDescending(x => x.Person.Gender);
                            break;
                    }
                    break;
                default:
                    switch (filter.Sort)
                    {
                        case PatientFilterSort.Email:
                            patients = patients.OrderBy(x => x.Person.Email);
                            break;
                        case PatientFilterSort.Phone:
                            patients = patients.OrderBy(x => x.Person.Phone);
                            break;
                        case PatientFilterSort.FirstName:
                            patients = patients.OrderBy(x => x.Person.FirstName);
                            break;
                        case PatientFilterSort.LastName:
                            patients = patients.OrderBy(x => x.Person.LastName);
                            break;
                        case PatientFilterSort.Birthday:
                            patients = patients.OrderBy(x => x.Person.Birthday);
                            break;
                        case PatientFilterSort.Gender:
                            patients = patients.OrderBy(x => x.Person.Gender);
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
            response.Patients = await patients
                .ToListAsync();

            return response;

            #endregion

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

    }
}
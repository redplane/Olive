using System.Linq;
using System.Threading.Tasks;
using ArangoDB.Client;
using OliveAdmin.Enumerations.Filter;
using OliveAdmin.Interfaces;
using OliveAdmin.ViewModels.Filter;
using Shared.Enumerations;
using Shared.Models.Vertexes;
using Shared.Repositories;
using Shared.ViewModels.Response;
using QueryableExtensions = System.Data.Entity.QueryableExtensions;

namespace OliveAdmin.Repositories
{
    public class RepositoryAccountExtended : RepositoryAccount, IRepositoryAccountExtended
    {
        #region Properties

        /// <summary>
        ///     Client connector to Arango database.
        /// </summary>
        private readonly ArangoDatabase _arangoClient;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialize an instance of repository.
        /// </summary>
        /// <param name="arangoClient"></param>
        public RepositoryAccountExtended(ArangoDatabase arangoClient) : base(arangoClient)
        {
            _arangoClient = arangoClient;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find the first admin account which matches with the specific conditions synchronously.
        /// </summary>
        /// <param name="filterAdminViewModel"></param>
        /// <returns></returns>
        public Admin FindAdmin(FilterAdminViewModel filterAdminViewModel)
        {
            // List of admins in database.
            IQueryable<Admin> admins = _arangoClient.Query<Admin>();

            // Filter admin account by using specific conditions.
            admins = FilterAdmins(admins, filterAdminViewModel);

            // Find the first result.
            return admins
                .Select(x => x)
                .FirstOrDefault();
        }

        /// <summary>
        ///     Find the first admin account which matches with the specific conditions asynchronously.
        /// </summary>
        /// <param name="filterAdminViewModel"></param>
        /// <returns></returns>
        public async Task<Admin> FindAdminAsync(FilterAdminViewModel filterAdminViewModel)
        {
            // List of admins in database.
            IQueryable<Admin> admins = _arangoClient.Query<Admin>();

            // Filter admin account by using specific conditions.
            admins = FilterAdmins(admins, filterAdminViewModel);

            // Find the first result.
            return await admins
                .Select(x => x)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Filter patients by using specific conditions asynchronously.
        /// </summary>
        /// <param name="filterPatientViewModel"></param>
        /// <returns></returns>
        public async Task<ResponsePatientFilter> FilterPatientsAsync(FilterPatientViewModel filterPatientViewModel)
        {
            // Take all patients from database.
            IQueryable<Patient> patients = _arangoClient.Query<Patient>();
            patients = FilterPatients(patients, filterPatientViewModel);

            // Do sorting.
            switch (filterPatientViewModel.SortDirection)
            {
                case SortDirection.Decending:
                    switch (filterPatientViewModel.SortProperty)
                    {
                        case FilterPatientSort.FirstName:
                            patients = patients.OrderByDescending(x => x.FirstName);
                            break;
                        case FilterPatientSort.LastName:
                            patients = patients.OrderByDescending(x => x.LastName);
                            break;
                        case FilterPatientSort.Created:
                            patients = patients.OrderByDescending(x => x.Created);
                            break;
                        case FilterPatientSort.LastModified:
                            patients = patients.OrderByDescending(x => x.LastModified);
                            break;
                        case FilterPatientSort.Birthday:
                            patients = patients.OrderByDescending(x => x.Birthday);
                            break;
                        case FilterPatientSort.Gender:
                            patients = patients.OrderByDescending(x => x.Gender);
                            break;
                        default:
                            patients = patients.OrderByDescending(x => x.Status);
                            break;
                    }
                    break;
                default:
                    switch (filterPatientViewModel.SortProperty)
                    {
                        case FilterPatientSort.FirstName:
                            patients = patients.OrderBy(x => x.FirstName);
                            break;
                        case FilterPatientSort.LastName:
                            patients = patients.OrderBy(x => x.LastName);
                            break;
                        case FilterPatientSort.Created:
                            patients = patients.OrderBy(x => x.Created);
                            break;
                        case FilterPatientSort.LastModified:
                            patients = patients.OrderBy(x => x.LastModified);
                            break;
                        case FilterPatientSort.Birthday:
                            patients = patients.OrderBy(x => x.Birthday);
                            break;
                        case FilterPatientSort.Gender:
                            patients = patients.OrderBy(x => x.Gender);
                            break;
                        default:
                            patients = patients.OrderBy(x => x.Status);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var responsePatientFilter = new ResponsePatientFilter();

            // Count the total records match the conditions.
            responsePatientFilter.Total = await QueryableExtensions.CountAsync(patients);

            // Throw results with pagination.
            if (filterPatientViewModel.Records != null)
                responsePatientFilter.Patients = await patients
                    .Skip(filterPatientViewModel.Page*filterPatientViewModel.Records.Value)
                    .Take(filterPatientViewModel.Records.Value)
                    .ToListAsync();

            responsePatientFilter.Patients = await patients.ToListAsync();
            return responsePatientFilter;
        }

        /// <summary>
        ///     Filter doctors by using specific conditions asychronously.
        /// </summary>
        /// <param name="filterDoctorViewModel"></param>
        /// <returns></returns>
        public async Task<ResponseDoctorFilter> FilterDoctorsAsync(FilterDoctorViewModel filterDoctorViewModel)
        {
            // Take all doctors from database.
            IQueryable<Doctor> doctors = _arangoClient.Query<Doctor>();
            doctors = FilterDoctors(doctors, filterDoctorViewModel);

            // Do sorting.
            switch (filterDoctorViewModel.SortDirection)
            {
                case SortDirection.Decending:
                    switch (filterDoctorViewModel.SortProperty)
                    {
                        case FilterDoctorSort.FirstName:
                            doctors = doctors.OrderByDescending(x => x.FirstName);
                            break;
                        case FilterDoctorSort.LastName:
                            doctors = doctors.OrderByDescending(x => x.LastName);
                            break;
                        case FilterDoctorSort.Created:
                            doctors = doctors.OrderByDescending(x => x.Created);
                            break;
                        case FilterDoctorSort.LastModified:
                            doctors = doctors.OrderByDescending(x => x.LastModified);
                            break;
                        case FilterDoctorSort.Birthday:
                            doctors = doctors.OrderByDescending(x => x.Birthday);
                            break;
                        case FilterDoctorSort.Gender:
                            doctors = doctors.OrderByDescending(x => x.Gender);
                            break;
                        case FilterDoctorSort.Status:
                            doctors = doctors.OrderByDescending(x => x.Status);
                            break;
                        default:
                            doctors = doctors.OrderByDescending(x => x.Rank);
                            break;
                    }
                    break;
                default:
                    switch (filterDoctorViewModel.SortProperty)
                    {
                        case FilterDoctorSort.FirstName:
                            doctors = doctors.OrderBy(x => x.FirstName);
                            break;
                        case FilterDoctorSort.LastName:
                            doctors = doctors.OrderBy(x => x.LastName);
                            break;
                        case FilterDoctorSort.Created:
                            doctors = doctors.OrderBy(x => x.Created);
                            break;
                        case FilterDoctorSort.LastModified:
                            doctors = doctors.OrderBy(x => x.LastModified);
                            break;
                        case FilterDoctorSort.Birthday:
                            doctors = doctors.OrderBy(x => x.Birthday);
                            break;
                        case FilterDoctorSort.Gender:
                            doctors = doctors.OrderBy(x => x.Gender);
                            break;
                        case FilterDoctorSort.Status:
                            doctors = doctors.OrderBy(x => x.Status);
                            break;
                        default:
                            doctors = doctors.OrderBy(x => x.Rank);
                            break;
                    }
                    break;
            }

            // Response initialization.
            var responseDoctorFilter = new ResponseDoctorFilter();

            // Count the total records match the conditions.
            responseDoctorFilter.Total = await QueryableExtensions.CountAsync(doctors);

            // Throw results with pagination.
            if (filterDoctorViewModel.Records != null)
            {
                // Do pagination.
                responseDoctorFilter.Doctors = await doctors
                    .Skip(filterDoctorViewModel.Page*filterDoctorViewModel.Records.Value)
                    .Take(filterDoctorViewModel.Records.Value)
                    .ToListAsync();

                return responseDoctorFilter;
            }

            responseDoctorFilter.Doctors = await doctors.ToListAsync();
            return responseDoctorFilter;
        }

        /// <summary>
        ///     Filter admins by using specific conditions.
        /// </summary>
        /// <param name="admins"></param>
        /// <param name="filterAdminViewModel"></param>
        /// <returns></returns>
        private IQueryable<Admin> FilterAdmins(IQueryable<Admin> admins, FilterAdminViewModel filterAdminViewModel)
        {
            // Name is specified.
            if (!string.IsNullOrWhiteSpace(filterAdminViewModel.Email))
                switch (filterAdminViewModel.EmailComparision)
                {
                    case TextComparision.Contain:
                        admins = admins.Where(x => AQL.Contains(x.Email, filterAdminViewModel.Email));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        admins = admins.Where(x => AQL.Upper(x.Email) == filterAdminViewModel.Email.ToUpper());
                        break;
                    default:
                        admins = admins.Where(x => x.Email == filterAdminViewModel.Email);
                        break;
                }

            // Password is specified.
            if (!string.IsNullOrWhiteSpace(filterAdminViewModel.Password))
                switch (filterAdminViewModel.PasswordComparision)
                {
                    case TextComparision.Contain:
                        admins = admins.Where(x => AQL.Contains(x.Password, filterAdminViewModel.Password));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        admins = admins.Where(x => AQL.Upper(x.Password) == filterAdminViewModel.Password.ToUpper());
                        break;
                    default:
                        admins = admins.Where(x => x.Password == filterAdminViewModel.Password);
                        break;
                }

            // Statuses are specified.
            if (filterAdminViewModel.Statuses != null)
                admins = admins.Where(x => AQL.In(x.Status, filterAdminViewModel.Statuses));

            return admins;
        }

        /// <summary>
        ///     Filter patients by using specific conditions.
        /// </summary>
        /// <param name="patients"></param>
        /// <param name="filterPatientViewModel"></param>
        /// <returns></returns>
        private IQueryable<Patient> FilterPatients(IQueryable<Patient> patients,
            FilterPatientViewModel filterPatientViewModel)
        {
            // Email is specified.
            if (!string.IsNullOrWhiteSpace(filterPatientViewModel.Email))
                switch (filterPatientViewModel.EmailComparision)
                {
                    case TextComparision.Contain:
                        patients = patients.Where(x => AQL.Contains(x.Email, filterPatientViewModel.Email));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        patients = patients.Where(x => AQL.Upper(x.Email) == filterPatientViewModel.Email.ToUpper());
                        break;
                    default:
                        patients = patients.Where(x => x.Email == filterPatientViewModel.Email);
                        break;
                }

            // First name is specified.
            if (!string.IsNullOrWhiteSpace(filterPatientViewModel.FirstName))
                switch (filterPatientViewModel.FirstNameComparision)
                {
                    case TextComparision.Contain:
                        patients = patients.Where(x => AQL.Contains(x.FirstName, filterPatientViewModel.FirstName));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        patients =
                            patients.Where(x => AQL.Upper(x.FirstName) == filterPatientViewModel.FirstName.ToUpper());
                        break;
                    default:
                        patients = patients.Where(x => x.FirstName == filterPatientViewModel.FirstName);
                        break;
                }

            // Last name is specified.
            if (!string.IsNullOrWhiteSpace(filterPatientViewModel.LastName))
                switch (filterPatientViewModel.LastNameComparision)
                {
                    case TextComparision.Contain:
                        patients = patients.Where(x => AQL.Contains(x.LastName, filterPatientViewModel.LastName));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        patients =
                            patients.Where(x => AQL.Upper(x.LastName) == filterPatientViewModel.LastName.ToUpper());
                        break;
                    default:
                        patients = patients.Where(x => x.LastName == filterPatientViewModel.LastName);
                        break;
                }

            // Birthday range is specified.
            if (filterPatientViewModel.MinBirthday != null)
                patients = patients.Where(x => x.Birthday >= filterPatientViewModel.MinBirthday.Value);

            if (filterPatientViewModel.MaxBirthday != null)
                patients = patients.Where(x => x.Birthday <= filterPatientViewModel.MaxBirthday.Value);

            // Phone is specified.
            if (!string.IsNullOrWhiteSpace(filterPatientViewModel.Phone))
                switch (filterPatientViewModel.PhoneComparision)
                {
                    case TextComparision.Contain:
                        patients = patients.Where(x => AQL.Contains(x.Phone, filterPatientViewModel.Phone));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        patients = patients.Where(x => AQL.Upper(x.Phone) == filterPatientViewModel.Phone.ToUpper());
                        break;
                    default:
                        patients = patients.Where(x => x.Phone == filterPatientViewModel.Phone);
                        break;
                }

            // Genders are specified.
            if ((filterPatientViewModel.Genders != null) && (filterPatientViewModel.Genders.Length > 0))
                patients = patients.Where(x => AQL.In(x.Gender, filterPatientViewModel.Genders));

            // Created is specified.
            if (filterPatientViewModel.MinCreated != null)
                patients = patients.Where(x => x.Created >= filterPatientViewModel.MinCreated.Value);

            if (filterPatientViewModel.MaxCreated != null)
                patients = patients.Where(x => x.Created <= filterPatientViewModel.MaxCreated.Value);

            // Last modified is specified.
            if (filterPatientViewModel.MinLastModified != null)
                patients =
                    patients.Where(
                        x => (x.LastModified != null) && (x.LastModified >= filterPatientViewModel.MinLastModified));

            if (filterPatientViewModel.MaxLastModified != null)
                patients =
                    patients.Where(
                        x => (x.LastModified != null) && (x.LastModified <= filterPatientViewModel.MinLastModified));

            return patients;
        }

        /// <summary>
        ///     Filter doctors by using specific conditions.
        /// </summary>
        /// <param name="doctors"></param>
        /// <param name="filterDoctorViewModel"></param>
        /// <returns></returns>
        private IQueryable<Doctor> FilterDoctors(IQueryable<Doctor> doctors, FilterDoctorViewModel filterDoctorViewModel)
        {
            // Email is specified.
            if (!string.IsNullOrWhiteSpace(filterDoctorViewModel.Email))
                switch (filterDoctorViewModel.EmailComparision)
                {
                    case TextComparision.Contain:
                        doctors = doctors.Where(x => AQL.Contains(x.Email, filterDoctorViewModel.Email));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        doctors = doctors.Where(x => AQL.Upper(x.Email) == filterDoctorViewModel.Email.ToUpper());
                        break;
                    default:
                        doctors = doctors.Where(x => x.Email == filterDoctorViewModel.Email);
                        break;
                }

            // First name is specified.
            if (!string.IsNullOrWhiteSpace(filterDoctorViewModel.FirstName))
                switch (filterDoctorViewModel.FirstNameComparision)
                {
                    case TextComparision.Contain:
                        doctors = doctors.Where(x => AQL.Contains(x.FirstName, filterDoctorViewModel.FirstName));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        doctors = doctors.Where(x => AQL.Upper(x.FirstName) == filterDoctorViewModel.FirstName.ToUpper());
                        break;
                    default:
                        doctors = doctors.Where(x => x.FirstName == filterDoctorViewModel.FirstName);
                        break;
                }

            // Last name is specified.
            if (!string.IsNullOrWhiteSpace(filterDoctorViewModel.LastName))
                switch (filterDoctorViewModel.LastNameComparision)
                {
                    case TextComparision.Contain:
                        doctors = doctors.Where(x => AQL.Contains(x.LastName, filterDoctorViewModel.LastName));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        doctors = doctors.Where(x => AQL.Upper(x.LastName) == filterDoctorViewModel.LastName.ToUpper());
                        break;
                    default:
                        doctors = doctors.Where(x => x.LastName == filterDoctorViewModel.LastName);
                        break;
                }

            // Rank is specified.
            if (filterDoctorViewModel.MinRank != null)
                doctors = doctors.Where(x => x.Rank >= filterDoctorViewModel.MinRank);
            if (filterDoctorViewModel.MaxRank != null)
                doctors = doctors.Where(x => x.Rank <= filterDoctorViewModel.MaxRank.Value);

            // Statuses are specified.
            if ((filterDoctorViewModel.Statuses != null) && (filterDoctorViewModel.Statuses.Length > 0))
                doctors = doctors.Where(x => AQL.In(x.Status, filterDoctorViewModel.Statuses));

            // Genders are specified.
            if ((filterDoctorViewModel.Genders != null) && (filterDoctorViewModel.Genders.Length > 0))
                doctors = doctors.Where(x => AQL.In(x.Gender, filterDoctorViewModel.Genders));

            // Phone number is specified.
            if (!string.IsNullOrWhiteSpace(filterDoctorViewModel.Phone))
                switch (filterDoctorViewModel.PhoneComparision)
                {
                    case TextComparision.Contain:
                        doctors = doctors.Where(x => AQL.Contains(x.Phone, filterDoctorViewModel.Phone));
                        break;
                    case TextComparision.EqualIgnoreCase:
                        doctors = doctors.Where(x => AQL.Upper(x.Phone) == filterDoctorViewModel.Phone.ToUpper());
                        break;
                    default:
                        doctors = doctors.Where(x => x.Phone == filterDoctorViewModel.Phone);
                        break;
                }

            // Birthday is specified.
            if (filterDoctorViewModel.MinBirthday != null)
                doctors = doctors.Where(x => x.Birthday >= filterDoctorViewModel.MinBirthday.Value);
            if (filterDoctorViewModel.MaxBirthday != null)
                doctors = doctors.Where(x => x.Birthday <= filterDoctorViewModel.MaxBirthday.Value);

            // Created is specified.
            if (filterDoctorViewModel.MinCreated != null)
                doctors = doctors.Where(x => x.Created >= filterDoctorViewModel.MinCreated.Value);
            if (filterDoctorViewModel.MaxCreated != null)
                doctors = doctors.Where(x => x.Created <= filterDoctorViewModel.MinCreated.Value);

            // LastModified is specified.
            if (filterDoctorViewModel.MinLastModified != null)
                doctors =
                    doctors.Where(
                        x =>
                            (x.LastModified != null) &&
                            (x.LastModified.Value >= filterDoctorViewModel.MinLastModified.Value));
            if (filterDoctorViewModel.MaxLastModified != null)
                doctors =
                    doctors.Where(
                        x =>
                            (x.LastModified != null) &&
                            (x.LastModified.Value <= filterDoctorViewModel.MinLastModified.Value));

            return doctors;
        }

        #endregion
    }
}
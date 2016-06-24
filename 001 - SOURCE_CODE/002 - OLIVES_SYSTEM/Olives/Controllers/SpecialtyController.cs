using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Resources;
using Shared.ViewModels;

namespace Olives.Controllers
{
    public class SpecialtyController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositorySpecialty"></param>
        /// <param name="log"></param>
        /// <param name="emailService"></param>
        public SpecialtyController(IRepositorySpecialty repositorySpecialty, ILog log, IEmailService emailService)
        {
            _repositorySpecialty = repositorySpecialty;
            _log = log;
            _emailService = emailService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find a specialty by using specialty id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/specialty")]
        [HttpGet]
        [OlivesAuthorize(new []{AccountRole.Doctor, AccountRole.Patient})]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            // Only filter and receive the first result.
            var specialtyFilter = new SpecialtyGetViewModel();
            specialtyFilter.Id = id;
            specialtyFilter.Page = 0;
            specialtyFilter.Records = 1;

            // Retrieve the results list.
            var results = await _repositorySpecialty.FilterSpecialty(specialtyFilter);

            // No result has been received.
            if (results == null || results.Specialties == null || results.Specialties.Count != 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] {Language.NoRecordHasBeenFound}
                });
            }

            // Retrieve the 1st queried result.
            var result = results.Specialties
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Specialty = result
            });
        }

        /// <summary>
        /// Filter specialties by using specific conditions.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/specialty/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] { AccountRole.Doctor, AccountRole.Patient })]
        public async Task<HttpResponseMessage> Filter([FromBody] SpecialtyGetViewModel info)
        {
            // Model hasn't been initialized.
            if (info == null)
            {
                _log.Error("Invalid specialties filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Errors = new [] {Language.InvalidRequestParameters}
                });
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid specialties filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve the results list.
            var results = await _repositorySpecialty.FilterSpecialty(info);

            // No result has been received.
            if (results == null || results.Specialties == null || results.Specialties.Count < 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] { Language.NoRecordHasBeenFound }
                });
            }

            // Retrieve the 1st queried result.
            var result = results.Specialties
                .Select(x => new
                {
                    x.Id,
                    x.Name
                });

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Specialties = result,
                results.Total
            });
        }
        
        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositorySpecialty _repositorySpecialty;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Service which is used for sending emails.
        /// </summary>
        private readonly IEmailService _emailService;

        #endregion
        
    }
}
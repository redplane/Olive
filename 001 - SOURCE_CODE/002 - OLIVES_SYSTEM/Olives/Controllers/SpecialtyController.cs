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
        ///     Find a specialty by using specialty id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/specialty")]
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            // Find the specialty by using specific id.
            var specialty = await _repositorySpecialty.FindSpecialtyAsync(id);

            // No result has been received.
            if (specialty == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Specialty = new
                {
                    specialty.Id,
                    specialty.Name
                }
            });
        }

        /// <summary>
        ///     Filter specialties by using specific conditions.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/specialty/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterSpecialtyViewModel info)
        {
            // Model hasn't been initialized.
            if (info == null)
            {
                info = new FilterSpecialtyViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Retrieve the results list.
            var results = await _repositorySpecialty.FilterSpecialtyAsync(info);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Specialties = results.Specialties
                    .Select(x => new
                    {
                        x.Id,
                        x.Name
                    }),
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
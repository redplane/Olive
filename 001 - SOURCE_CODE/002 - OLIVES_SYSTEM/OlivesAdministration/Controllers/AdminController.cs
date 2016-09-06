using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using log4net;
using Newtonsoft.Json;
using Olive.Admin.Attributes;
using Olive.Admin.Interfaces;
using Olive.Admin.Resources;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.ViewModels;

namespace Olive.Admin.Controllers
{
    [MvcAuthorize(new[] { Role.Admin})]
    public class AdminController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AdminController.
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="log"></param>
        /// <param name="timeService"></param>
        public AdminController(
            IRepositoryAccountExtended repositoryAccountExtended,
            ILog log,
            ITimeService timeService)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _log = log;
            _timeService = timeService;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// This function is for rendering login page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Requester is already logged in. Redirect him/her to home page.
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            
            return View();
        }

        /// <summary>
        ///     This function is for posting login information to service.
        ///     HttpStatusCode != 200 means login is not successful.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel loginViewModel)
        {
            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Errors sent to client");
                return View(loginViewModel);
            }

            // Requester is already logged in. Redirect him/her to home page.
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            // Find the hashed password from the original one.
            var accountHashedPassword = _repositoryAccountExtended.FindMd5Password(loginViewModel.Password);

            // Pass parameter to login function. 
            var admin =
                await
                    _repositoryAccountExtended.FindPersonAsync(null, loginViewModel.Email, accountHashedPassword,
                        (byte) Role.Admin, null);

            // If no result return, that means no account.
            if (admin == null)
            {
                _log.Error($"{loginViewModel.Email} is not found in database");
                    
                // Response to client.
                ModelState.AddModelError("Login", string.Format(Olive_Admin_Language.AccountIsNotFound, loginViewModel.Email));

                return View(loginViewModel);
            }

            // Account is pending.
            if (admin.Status == (byte) StatusAccount.Pending)
            {
                _log.Error($"{loginViewModel.Email} is pending");

                // Response to client.
                ModelState.AddModelError("Login", string.Format(Olive_Admin_Language.AccountIsNotFound, loginViewModel.Email));

                return View(loginViewModel);
            }

            // Account is disabled.
            if (admin.Status == (byte)StatusAccount.Inactive)
            {
                _log.Error($"{loginViewModel.Email} is disabled");

                // Response to client.
                ModelState.AddModelError("Login", string.Format(Olive_Admin_Language.AccountIsNotFound, loginViewModel.Email));

                return View(loginViewModel);
            }

            // Update the password with the hashed one
            loginViewModel.Password = accountHashedPassword;

            // Initialize form authentication ticket, encrypt and store it to cookie.
            var formAuthenticationTicket = new FormsAuthenticationTicket(1, FormsAuthentication.FormsCookieName, DateTime.Now, DateTime.Now.AddMinutes(30), true, JsonConvert.SerializeObject(loginViewModel));
                
            // Initialize cookie contain the authorization ticket.
            var httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(formAuthenticationTicket));
            Response.Cookies.Add(httpCookie);

            return RedirectToAction("Index", "Home");
            
        }

        ///// <summary>
        /////     Find a patient by using specific id.
        ///// </summary>
        ///// <returns></returns>
        //[System.Web.Http.HttpPut]
        //[OlivesAuthorize(new[] {Role.Admin})]
        //[System.Web.Http.Route("api/admin/profile")]
        //public async Task<HttpResponseMessage> EditAdminProfileAsync([FromBody] EditAdminProfileViewModel editor)
        //{
        //    #region Request parameters validation

        //    // Model hasn't been initialized.
        //    if (editor == null)
        //    {
        //        editor = new EditAdminProfileViewModel();
        //        Validate(editor);
        //    }

        //    // ModelState is invalid.
        //    if (!ModelState.IsValid)
        //    {
        //        _log.Error("Request parameters are invalid. Errors sent to client");
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
        //    }

        //    #endregion

        //    #region Information construction

        //    // Retrieve information of person who sent request.
        //    var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

        //    // First name is defined.
        //    if (!string.IsNullOrWhiteSpace(editor.FirstName))
        //        requester.FirstName = editor.FirstName;

        //    // Last name is defined.
        //    if (!string.IsNullOrWhiteSpace(editor.LastName))
        //        requester.LastName = editor.LastName;

        //    // Birthday is defined.
        //    if (editor.Birthday != null)
        //        requester.Birthday = editor.Birthday;

        //    // Password is defined.
        //    if (!string.IsNullOrWhiteSpace(editor.Password))
        //        requester.Password = editor.Password;

        //    // Gender is defined.
        //    if (editor.Gender != null)
        //        requester.Gender = (byte) editor.Gender;

        //    // Phone is defined.
        //    if (!string.IsNullOrWhiteSpace(editor.Phone))
        //        requester.Phone = editor.Phone;

        //    // Address is defined.
        //    if (!string.IsNullOrWhiteSpace(editor.Address))
        //        requester.Address = editor.Address;

        //    // Update person full name.
        //    requester.FullName = requester.FirstName + " " + requester.LastName;

        //    #endregion

        //    #region Result handling

        //    try
        //    {
        //        // Update the last modified.
        //        requester.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

        //        // Update the patient.
        //        requester = await _repositoryAccountExtended.EditPersonProfileAsync(requester.Id, requester);

        //        return Request.CreateResponse(HttpStatusCode.OK, new
        //        {
        //            User = new
        //            {
        //                requester.Id,
        //                requester.Email,
        //                requester.Password,
        //                requester.FirstName,
        //                requester.LastName,
        //                requester.Birthday,
        //                requester.Phone,
        //                requester.Gender,
        //                requester.Role,
        //                requester.Created,
        //                requester.LastModified,
        //                requester.Status,
        //                requester.Address,
        //                Photo = requester.PhotoUrl
        //            }
        //        });
        //    }
        //    catch (Exception exception)
        //    {
        //        _log.Error(exception.Message, exception);
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError);
        //    }

        //    #endregion
        //}

        #endregion

        #region Properties

        /// <summary>
        ///     Repository account.
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Instance for logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Service which is used to access time calculation functions.
        /// </summary>
        private readonly ITimeService _timeService;

        #endregion
    }
}
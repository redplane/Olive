using System.Web.Mvc;
using DotnetSignalR.Repository;
using Shared.Interfaces;

namespace DotnetSignalR.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRepositoryAccount _repositoryAccount;

        public AdminController(RepositoryAccount repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        
        public void Get()
        {
        }
    }
}
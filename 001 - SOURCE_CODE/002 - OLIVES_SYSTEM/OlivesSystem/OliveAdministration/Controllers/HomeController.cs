using System.Collections.Generic;
using System.Web.Mvc;
using DotnetSignalR.Interfaces;

namespace DotnetSignalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepositoryProduct _repositoryProduct;

        public HomeController(IRepositoryProduct repositoryProduct)
        {
            _repositoryProduct = repositoryProduct;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        [HttpGet]
        public ActionResult Get()
        {
            var lists = _repositoryProduct.GetProducts();

            return Json(lists, JsonRequestBehavior.AllowGet);
        }
    }
}
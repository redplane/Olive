using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ApiLayer.Context;
using ApiLayer.Models;

namespace ApiLayer.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            return View();

        }
    }
}
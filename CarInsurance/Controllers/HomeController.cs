using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurances.Controllers
{
    public class HomeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Admin()
        {
            ViewBag.Message = "Your Admin page.";

            return View(db.Insurees.ToList());
        }
    }
}
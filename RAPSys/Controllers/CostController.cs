using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RAPSys.Controllers
{
    public class CostController : Controller
    {
        // GET: Cost
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MarketSurvey() => View("~/Views/Error/PageUnderConstruction.cshtml");
        public ActionResult AnnualRate() => View("~/Views/Error/PageUnderConstruction.cshtml");
        public ActionResult RealCost() => View("~/Views/Error/PageUnderConstruction.cshtml");
    }
}
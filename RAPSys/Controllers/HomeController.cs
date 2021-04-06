using System.Web.Mvc;
using Repository;
using RapModel.ViewModel;

namespace RAPSys.Controllers
{
    public class HomeController : Controller
    {
        readonly HelpersRepository helpersRepository = new HelpersRepository();

        public static PersonViewModel person;

        public HomeController()
        {
            person = helpersRepository.GetLogInPerson();
        }

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
    }
}
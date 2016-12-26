using RiskApp.Calculations;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RiskApp.Controllers
{
    public class HomeController : Controller
    {
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

        [HttpPost]
        public async Task<ActionResult> Create(HttpPostedFileBase file)
        {
            return View("Result", await new Calculator().CalculateAsync(file));
        }

        public FileResult DownloadSample()
        {
            return File(Assembly.GetExecutingAssembly().GetManifestResourceStream("RiskApp.Риски1.xls"),
                System.Net.Mime.MediaTypeNames.Application.Octet, "Риски1.xls");
        }
    }
}
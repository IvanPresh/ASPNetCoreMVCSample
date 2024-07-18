using Microsoft.AspNetCore.Mvc;

namespace ASPNetCoreMVCSample.Controllers
{
    public class IvanController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Add()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View("~/Views/Ivan/Add.cshtml");
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    public class PrivacyController : Controller
    {
        // GET: /Privacy/
        public IActionResult Index()
        {
            return View();
        }
    }
}
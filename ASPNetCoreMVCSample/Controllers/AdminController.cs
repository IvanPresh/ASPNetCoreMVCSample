using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPNetCoreMVCSample.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles ="Admin,SuperAdmin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

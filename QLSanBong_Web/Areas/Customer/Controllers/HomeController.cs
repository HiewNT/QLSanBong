using Microsoft.AspNetCore.Mvc;

namespace QLSanBong_Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult SanBong()
        {
            return View();
        }
    }
}

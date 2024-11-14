using Microsoft.AspNetCore.Mvc;

namespace QLSanBong_Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class YeuCauController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

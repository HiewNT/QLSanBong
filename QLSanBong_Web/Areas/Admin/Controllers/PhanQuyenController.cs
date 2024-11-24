using Microsoft.AspNetCore.Mvc;

namespace QLSanBong_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhanQuyenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Role()
        {
            return View();
        }
        public IActionResult Auth()
        {
            return View();
        }
        // Phương thức trả về Partial View
        public IActionResult ActionList()
        {
            return PartialView("_ActionList"); // Tên của Partial View
        }
        public IActionResult Service()
        {
            return View();
        }
        // Phương thức trả về Partial View
        public IActionResult ServiceList()
        {
            return PartialView("_ServiceList"); // Tên của Partial View
        }
    }
}

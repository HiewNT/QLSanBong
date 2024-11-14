using Microsoft.AspNetCore.Mvc;

namespace QLSanBong_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HoaDonController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Phương thức trả về Partial View
        public IActionResult HoaDonList()
        {
            return PartialView("_HoaDonList"); // Tên của Partial View
        }
    }
}

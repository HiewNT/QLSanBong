using Microsoft.AspNetCore.Mvc;

namespace QLSanBong_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Phương thức trả về Partial View
        public IActionResult KhachHangList()
        {
            return PartialView("_KhachHangList"); // Tên của Partial View
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace QLSanBong_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhanVienController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Phương thức trả về Partial View
        public IActionResult NhanVienList()
        {
            return PartialView("_NhanVienList"); // Tên của Partial View
        }
    }
}

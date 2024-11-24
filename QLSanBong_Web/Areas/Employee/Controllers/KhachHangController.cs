using Microsoft.AspNetCore.Mvc;

namespace QLSanBong_Web.Areas.Employee.Controllers
{
    [Area("Employee")]
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

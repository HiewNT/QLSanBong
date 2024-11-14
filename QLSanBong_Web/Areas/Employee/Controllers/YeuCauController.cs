using Microsoft.AspNetCore.Mvc;

namespace QLSanBong_Web.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class YeuCauController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Phương thức trả về Partial View
        public IActionResult YeuCauList()
        {
            return PartialView("_YeuCauList"); // Tên của Partial View
        }
    }
}

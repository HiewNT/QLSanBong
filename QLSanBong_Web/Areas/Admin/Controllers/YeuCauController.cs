using Microsoft.AspNetCore.Mvc;

namespace QLSanBong_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
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

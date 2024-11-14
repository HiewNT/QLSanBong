using Microsoft.AspNetCore.Mvc;

namespace QLSanBong_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GiaGioThueController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Phương thức trả về Partial View
        public IActionResult GiaThueList()
        {
            return PartialView("_GiaThueList"); // Tên của Partial View
        }
    }
}

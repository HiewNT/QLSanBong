using Microsoft.AspNetCore.Mvc;

namespace QLSanBong_Web.Areas.Employee.Controllers
{
    [Area("Employee")]
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

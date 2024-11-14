using Microsoft.AspNetCore.Mvc;

namespace QLSanBong_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SanBongController : Controller
    {
        // Trang danh sách sân bóng
        public IActionResult Index()
        {
            return View();
        }

        // Trang chi tiết sân bóng
        public IActionResult Details()
        {
            return View();
        }

        // Trang chỉnh sửa sân bóng
        public IActionResult Edit()
        {
            return View();
        }
        // Trang thêm sân bóng
        public IActionResult Add()
        {
            return View();
        }
    }
}

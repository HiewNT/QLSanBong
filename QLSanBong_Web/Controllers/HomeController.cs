using Microsoft.AspNetCore.Mvc;
using QLSanBong_Web.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace QLSanBong_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: /Home/Index
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }
        // GET: /Home/Index
        public IActionResult Cart()
        {
            return View();
        }

        // POST: /Home/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Thông tin không hợp lệ." });
            }

            return Json(new { success = true, message = " Đăng nhập thành công!" }); // Trả về thông báo thành công để client xử lý
        }

        // Phương thức lấy vai trò từ token
        private string GetRoleFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            return roleClaim?.Value ?? "KhachHang";
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace QLSanBong_Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}

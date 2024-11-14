namespace QLSanBong_API.Models
{

    public class TaiKhoanVM
    {
        public string Password { get; set; } = null!;

        public string? Role { get; set; }

    }
    public partial class TaiKhoan : TaiKhoanVM
    {
        public string Username { get; set; } = null!;
    }
}

namespace QLSanBong_API.Models
{
    public class LoginResponse
    {
        public string Username { get; set; }        // Mã nhân viên
        public string Manguoidung { get; set; }        // Mã nhân viên
        public string Tennguoidung { get; set; }       // Tên nhân viên
        public string Sdt { get; set; }       // Tên nhân viên
        public string Chucvu { get; set; }      // Chức vụ
        public string Token { get; set; }       // JWT Token
    }
}

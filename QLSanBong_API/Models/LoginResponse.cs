namespace QLSanBong_API.Models
{
    public class LoginResponse
    {
        public string UserID { get; set; }        // Mã nhân viên
        public string Manguoidung { get; set; }        // Mã nhân viên
        public string Tennguoidung { get; set; }       // Tên nhân viên
        public string Sdt { get; set; }       // Tên nhân viên
        public string Chucvu { get; set; }      // Chức vụ
        public string Token { get; set; }       // JWT Token
        public List<string> Permissions { get; set; } // Thêm quyền vào đây
    }
}

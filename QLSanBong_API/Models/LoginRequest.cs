namespace QLSanBong_API.Models
{
    public class LoginRequest
    {
        public string Username { get; set; } // Tên đăng nhập
        public string Password { get; set; }     // Mật khẩu
    }
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

}

using QLSanBong_API.Models;

namespace QLSanBong_API.Services.IService
{
    public interface ILoginService
    {
        string Login(string Username, string Password);
        string GenerateJwtToken(TaiKhoan taiKhoan);
        LoginResponse DecodeJwtToken(string token);
        bool IsUsernameTaken(string username); // Thêm phương thức này
        bool IsSdtTaken(string sdt); // Thêm phương thức này

    }
}

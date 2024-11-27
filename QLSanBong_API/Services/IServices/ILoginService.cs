using QLSanBong_API.Models;

namespace QLSanBong_API.Services.IService
{
    public interface ILoginService
    {
        string Login(LoginRequest login);
        string GenerateJwtToken(User taiKhoan);
        LoginResponse DecodeJwtToken(string token);

        //Task<RefreshTokenResponse> RefreshTokens(string refreshToken);
        bool IsUsernameTaken(string username); // Thêm phương thức này
        bool IsSdtTaken(string sdt); // Thêm phương thức này

    }
}

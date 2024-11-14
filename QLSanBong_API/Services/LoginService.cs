using Microsoft.IdentityModel.Tokens;
using QLSanBong_API.Data;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QLSanBong_API.Services
{
    public class LoginService : ILoginService
    {
        private readonly QlsanBongContext _context;
        private readonly IConfiguration _configuration;

        public LoginService(QlsanBongContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Phương thức xác thực người dùng và tạo token
        public string Login(string username, string password)
        {
            // Tìm tài khoản trong cơ sở dữ liệu
            var taiKhoanData = _context.TaiKhoans.SingleOrDefault(x => x.Username == username);

            // Kiểm tra tài khoản có tồn tại và mật khẩu có khớp không
            if (taiKhoanData == null || !VerifyPassword(password, taiKhoanData.Password))
            {
                throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng");
            }

            // Chuyển đổi đối tượng từ Data.TaiKhoan sang Models.TaiKhoan
            var taiKhoan = new Models.TaiKhoan
            {
                Username = taiKhoanData.Username,
                Password = taiKhoanData.Password, // Có thể không cần lưu trữ mật khẩu
                Role = taiKhoanData.Role
            };

            // Tạo token cho người dùng đã xác thực
            return GenerateJwtToken(taiKhoan);
        }

        public string GenerateJwtToken(Models.TaiKhoan taiKhoan)
        {
            // Truy xuất thông tin Manguoidung và Tennguoidung từ KhachHang hoặc NhanVien dựa vào username
            var manguoidung = "";

            // Kiểm tra nếu username tồn tại trong bảng KhachHang hoặc NhanVien
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.Tendangnhap == taiKhoan.Username);
            if (khachHang != null)
            {
                manguoidung = khachHang.MaKh;
            }
            else
            {
                var nhanVien = _context.NhanViens.FirstOrDefault(nv => nv.Tendangnhap == taiKhoan.Username);
                if (nhanVien != null)
                {
                    manguoidung = nhanVien.MaNv;
                }
            }

            // Thông tin các claims để thêm vào token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, taiKhoan.Username),
                new Claim(ClaimTypes.Role, taiKhoan.Role ?? "KhachHang"),
                new Claim("Manguoidung", manguoidung)
            };

            // Lấy khóa bí mật từ cấu hình
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tạo token với thời gian hết hạn và các claims
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60), // Token có thời hạn 60 phút
                signingCredentials: creds
            );

            // Trả về chuỗi token đã ký
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    public LoginResponse DecodeJwtToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Truy xuất các claim từ token
        var username = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var manguoidung = jwtToken.Claims.FirstOrDefault(c => c.Type == "Manguoidung")?.Value;
        var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var tennguoidung = "";
        var sdt="";
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.Tendangnhap == username);
        if (khachHang != null)
        {
            tennguoidung = khachHang.TenKh;
            sdt=khachHang.Sdt;
        }
        else
        {
            var nhanVien = _context.NhanViens.FirstOrDefault(nv => nv.Tendangnhap == username);
            if (nhanVien != null)
            {
                tennguoidung = nhanVien.TenNv;
                sdt=nhanVien.Sdt;
            }
        }

        // Xác định chức vụ dựa trên role
        string chucvu;
        if (role == "NhanVien")
        {
            chucvu = "Nhân Viên";
        }
        else if (role == "Admin")
        {
            chucvu = "Quản Trị Viên";
        }
        else
        {
            chucvu = "Khách Hàng"; // Mặc định là khách hàng
        }

        // Trả về đối tượng LoginResponse
        return new LoginResponse
        {
            Username = username,
            Manguoidung = manguoidung,
            Tennguoidung = tennguoidung,
            Sdt=sdt,
            Chucvu = chucvu,
            Token = token
        };
    }


    public bool IsUsernameTaken(string username)
        {
            // Kiểm tra xem tên người dùng đã tồn tại trong cơ sở dữ liệu chưa
            return _context.TaiKhoans.Any(x => x.Username == username);
        }
        public bool IsSdtTaken(string sdt)
        {
            // Kiểm tra xem tên người dùng đã tồn tại trong cơ sở dữ liệu chưa
            return _context.KhachHangs.Any(x => x.Sdt == sdt);
        }
        // Phương thức để kiểm tra mật khẩu có khớp với mật khẩu đã mã hóa hay không
        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}

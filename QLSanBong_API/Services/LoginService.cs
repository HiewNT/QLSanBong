using Microsoft.IdentityModel.Tokens;
using QLSanBong_API.Data;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;

namespace QLSanBong_API.Services
{
    public class LoginService : ILoginService
    {
        private readonly QlsanBongContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginService> _logger;

        public LoginService(QlsanBongContext context, IConfiguration configuration, ILogger<LoginService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        // Phương thức xác thực người dùng và tạo token
        public string Login(string username, string password)
        {
            try
            {
                // Tìm tài khoản trong cơ sở dữ liệu
                var taiKhoanData = _context.Users.SingleOrDefault(x => x.Username == username);
                if (taiKhoanData == null)
                {
                    throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng.");
                }

                // Kiểm tra mật khẩu
                if (!VerifyPassword(password, taiKhoanData.Password))
                {
                    throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng.");
                }

                // Lấy RoleId của người dùng từ UserRoles
                var roleId = _context.UserRoles
                    .Where(ur => ur.UserId == taiKhoanData.UserId)
                    .Select(ur => ur.RoleId)
                    .FirstOrDefault();

                if (roleId == null)
                {
                    throw new UnauthorizedAccessException("Người dùng không có vai trò hợp lệ.");
                }

                // Lấy thông tin vai trò từ bảng Roles
                var role = _context.Roles.SingleOrDefault(r => r.RoleId == roleId);
                if (role == null)
                {
                    throw new UnauthorizedAccessException("Vai trò người dùng không tồn tại.");
                }

                // Chuyển đổi đối tượng từ Data sang Models
                var user = new Models.User
                {
                    Username = username,
                    RoleName = role.RoleName,
                    UserID = taiKhoanData.UserId // Đảm bảo có UserId ở đây
                };

                // Tạo JWT token
                return GenerateJwtToken(user);
            }
            catch (UnauthorizedAccessException)
            {
                // Ném lại lỗi xác thực mà không ghi đè thông tin lỗi
                throw;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết nếu cần thiết
                // Log.Error("Login error: ", ex);

                throw new InvalidOperationException("Đã xảy ra lỗi khi đăng nhập.");
            }
        }

        public string GenerateJwtToken(Models.User taiKhoan)
        {
            try
            {
                // Truy xuất thông tin Manguoidung từ KhachHang hoặc NhanVien dựa vào UserID
                var manguoidung = "";

                // Kiểm tra nếu UserID tồn tại trong KhachHang hoặc NhanVien
                var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.UserId == taiKhoan.UserID);
                if (khachHang != null)
                {
                    manguoidung = khachHang.MaKh;
                }
                else
                {
                    var nhanVien = _context.NhanViens.FirstOrDefault(nv => nv.UserId == taiKhoan.UserID);
                    if (nhanVien != null)
                    {
                        manguoidung = nhanVien.MaNv;
                    }
                }

                var roleid = _context.UserRoles
                            .Where(ro => ro.UserId == taiKhoan.UserID)
                            .Select(ro => ro.RoleId)
                            .FirstOrDefault();

                // Lấy danh sách AuthID từ bảng UserAuth dựa trên UserID
                var userAuths = _context.RoleAuths
                    .Where(ua => ua.RoleId == roleid)
                    .Select(ua => ua.AuthId)
                    .ToList();

                // Thêm quyền (permissions) vào claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, taiKhoan.UserID),
            new Claim(ClaimTypes.Role, taiKhoan.RoleName ?? "KhachHang"),
            new Claim("Manguoidung", manguoidung)
        };

                // Thêm từng quyền từ bảng RoleAuths vào claims
                foreach (var auth in userAuths)
                {
                    claims.Add(new Claim("Permission", auth.ToString()));
                }

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
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tạo token: {ex.Message}");
                throw new InvalidOperationException("Đã xảy ra lỗi khi tạo token.");
            }
        }


        public LoginResponse DecodeJwtToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException(nameof(token), "Token không hợp lệ.");
                }

                var handler = new JwtSecurityTokenHandler();

                // Kiểm tra nếu token không hợp lệ
                if (!handler.CanReadToken(token))
                {
                    throw new InvalidOperationException("Token không hợp lệ.");
                }

                // Giải mã token
                var jwtToken = handler.ReadJwtToken(token);

                // Truy xuất các claim từ token
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var manguoidung = jwtToken.Claims.FirstOrDefault(c => c.Type == "Manguoidung")?.Value;
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new InvalidOperationException("Không tìm thấy thông tin người dùng trong token.");
                }

                var tennguoidung = "";
                var sdt = "";

                // Kiểm tra xem user là khách hàng hay nhân viên
                var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.UserId == userId);
                if (khachHang != null)
                {
                    tennguoidung = khachHang.TenKh;
                    sdt = khachHang.Sdt;
                }
                else
                {
                    var nhanVien = _context.NhanViens.FirstOrDefault(nv => nv.UserId == userId);
                    if (nhanVien != null)
                    {
                        tennguoidung = nhanVien.TenNv;
                        sdt = nhanVien.Sdt;
                    }
                    else
                    {
                        throw new InvalidOperationException("Người dùng không tồn tại trong hệ thống.");
                    }
                }

                // Xác định chức vụ dựa trên role
                string chucvu = role switch
                {
                    "NhanVien" => "Nhân Viên",
                    "Admin" => "Quản Trị Viên",
                    _ => "Khách Hàng" // Mặc định là khách hàng
                };

                // Lấy quyền từ claims
                var permissions = jwtToken.Claims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .ToList();

                // Trả về đối tượng LoginResponse với quyền (permissions)
                return new LoginResponse
                {
                    UserID = userId,
                    Manguoidung = manguoidung,
                    Tennguoidung = tennguoidung,
                    Sdt = sdt,
                    Chucvu = chucvu,
                    Token = token,
                    Permissions = permissions // Thêm quyền vào response
                };
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết để dễ dàng truy vết
                _logger.LogError($"Lỗi khi giải mã token: {ex.Message}");
                throw new InvalidOperationException("Đã xảy ra lỗi khi giải mã token.");
            }
        }


        // Kiểm tra mật khẩu có khớp với mật khẩu đã mã hóa hay không
        private bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi kiểm tra mật khẩu: {ex.Message}");
                throw new InvalidOperationException("Đã xảy ra lỗi khi kiểm tra mật khẩu.");
            }
        }
        public bool IsUsernameTaken(string userid)
        {
            // Kiểm tra xem tên người dùng đã tồn tại trong cơ sở dữ liệu chưa
            return _context.Users.Any(x => x.UserId == userid);
        }
        public bool IsSdtTaken(string sdt)
        {
            // Kiểm tra xem tên người dùng đã tồn tại trong cơ sở dữ liệu chưa
            return _context.KhachHangs.Any(x => x.Sdt == sdt);
        }
        // Phương thức để kiểm tra mật khẩu có khớp với mật khẩu đã mã hóa hay không
    }
}

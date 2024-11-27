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
        public string Login(LoginRequest login)
        {
            try
            {
                // Tìm tài khoản trong cơ sở dữ liệu
                var taiKhoanData = _context.Users.SingleOrDefault(x => x.Username == login.Username);
                if (taiKhoanData == null)
                {
                    throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng.");
                }

                // Kiểm tra mật khẩu
                if (!VerifyPassword(login.Password, taiKhoanData.Password))
                {
                    throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng.");
                }

                // Lấy RoleId của người dùng từ UserRoles
                var userRole = _context.UserRoles
                    .FirstOrDefault(ur => ur.UserId == taiKhoanData.UserId);

                if (userRole == null)
                {
                    throw new UnauthorizedAccessException("Người dùng không có vai trò hợp lệ.");
                }

                // Lấy thông tin vai trò từ bảng Roles
                var role = _context.Roles.SingleOrDefault(r => r.RoleId == userRole.RoleId);
                if (role == null)
                {
                    throw new UnauthorizedAccessException("Vai trò người dùng không tồn tại.");
                }

                // Chuyển đổi đối tượng từ Data sang Models
                var user = new Models.User
                {
                    Username = login.Username,
                    UserID = taiKhoanData.UserId, // Đảm bảo có UserId ở đây
                    RoleVM = new List<RoleVM> // Khởi tạo danh sách RoleVM
                    {
                        new RoleVM
                        {
                            RoleName = role.RoleName, // Cập nhật thông tin vai trò
                            ThongTin = role.ThongTin // Cập nhật thông tin chi tiết của vai trò
                        }
                    }
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

                var user = _context.Users.FirstOrDefault(kh => kh.UserId == taiKhoan.UserID);

                // Lấy danh sách RoleId của người dùng từ UserRoles
                var roleIds = _context.UserRoles
                                        .Where(ro => ro.UserId == taiKhoan.UserID)
                                        .Select(ro => ro.RoleId)
                                        .ToList();

                // Lấy danh sách các vai trò từ bảng Roles dựa trên RoleIds
                var roles = _context.Roles
                                    .Where(r => roleIds.Contains(r.RoleId))
                                    .ToList();

                // Lấy quyền theo từng vai trò từ bảng RoleAuths
                var permissions = new Dictionary<string, List<string>>();
                foreach (var role in roles)
                {
                    var rolePermissions = _context.RoleAuths
                        .Where(ua => ua.RoleId == role.RoleId)
                        .Select(ua => ua.AuthId)
                        .ToList();

                    if (rolePermissions.Any())
                    {
                        permissions[role.RoleName] = rolePermissions;
                    }
                }

                // Thêm các claim cơ bản
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, taiKhoan.UserID),
            new Claim("Manguoidung", user.UserId),
            new Claim("Sdt", user.Sdt),
            new Claim("Tennguoidung", user.Ten)
        };

                // Thêm các vai trò vào claims
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.RoleName ?? "KhachHang"));
                }

                // Thêm quyền cho từng vai trò vào claims
                foreach (var permission in permissions)
                {
                    claims.Add(new Claim($"Permission_{permission.Key}", string.Join(",", permission.Value)));
                }

                // Lấy khóa bí mật từ cấu hình
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Tạo token với thời gian hết hạn và các claims
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60), // Token có thời gian sống 60 phút
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
                // Kiểm tra nếu token không hợp lệ
                if (string.IsNullOrEmpty(token))
                {
                    throw new ArgumentNullException(nameof(token), "Token không hợp lệ.");
                }

                var handler = new JwtSecurityTokenHandler();

                // Kiểm tra nếu token có thể được đọc
                if (!handler.CanReadToken(token))
                {
                    throw new InvalidOperationException("Token không hợp lệ.");
                }

                // Giải mã token
                var jwtToken = handler.ReadJwtToken(token);

                // Lấy thông tin từ token
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var manguoidung = jwtToken.Claims.FirstOrDefault(c => c.Type == "Manguoidung")?.Value;
                var sdt = jwtToken.Claims.FirstOrDefault(c => c.Type == "Sdt")?.Value;
                var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

                if (string.IsNullOrEmpty(userId))
                {
                    throw new InvalidOperationException("Không tìm thấy thông tin người dùng trong token.");
                }

                // Lấy các vai trò (roles)
                var roles = roleClaims.Select(c => c.Value).ToList();

                // Lấy quyền theo từng vai trò từ các claims
                var permissions = new Dictionary<string, List<string>>();
                foreach (var role in roles)
                {
                    var rolePermissions = jwtToken.Claims
                        .Where(c => c.Type == $"Permission_{role}")
                        .Select(c => c.Value)
                        .ToList();

                    if (rolePermissions.Any())
                    {
                        permissions[role] = rolePermissions;
                    }
                }

                // Lấy thông tin người dùng từ các claim
                var tennguoidung = jwtToken.Claims.FirstOrDefault(c => c.Type == "Tennguoidung")?.Value;

                // Trả về đối tượng LoginResponse với thông tin người dùng, quyền và roles
                return new LoginResponse
                {
                    UserID = userId,
                    Tennguoidung = tennguoidung,
                    Manguoidung = manguoidung,
                    Sdt = sdt,
                    Roles = roles,
                    Permissions = permissions, // Quyền của từng vai trò
                    Token = token
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
            return _context.Users.Any(x => x.Sdt == sdt);
        }
        // Phương thức để kiểm tra mật khẩu có khớp với mật khẩu đã mã hóa hay không
    }
}

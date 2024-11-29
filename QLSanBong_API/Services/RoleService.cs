using Microsoft.IdentityModel.Tokens;
using QLSanBong_API.Data;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace QLSanBong_API.Services
{
    public class RoleService : IRoleService
    {
        private readonly QlsanBongContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RoleService> _logger;

        public RoleService(QlsanBongContext context, IConfiguration configuration, ILogger<RoleService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        #region Role
        public string AddRole(RoleVM roleVM)
        {
            // Kiểm tra xem vai trò đã tồn tại hay chưa
            if (_context.Roles.Any(r => r.RoleName == roleVM.RoleName))
            {
                throw new InvalidOperationException("Vai trò đã tồn tại.");
            }

            // Tạo vai trò mới
            var newRole = new Data.Role
            {
                RoleId = Guid.NewGuid().ToString(), // Tạo RoleId là Guid
                RoleName = roleVM.RoleName, // Lấy tên vai trò từ RoleVM
                ThongTin = roleVM.ThongTin // Lấy tên vai trò từ RoleVM
            };

            _context.Roles.Add(newRole);
            _context.SaveChanges();

            return newRole.RoleId; // Trả về RoleId dưới dạng chuỗi
        }
        // Lấy danh sách tất cả các vai trò
        public List<Models.Role> GetAll()
        {
            return _context.Roles.Select(r => new Models.Role
            {
                RoleID = r.RoleId,
                RoleName = r.RoleName,
                ThongTin = r.ThongTin
            }).ToList();
        }

        // Xóa vai trò theo ID
        public bool DeleteRole(string roleId)
        {
            var role = _context.Roles.FirstOrDefault(r => r.RoleId == roleId);
            if (role == null)
            {
                throw new KeyNotFoundException("Vai trò không tồn tại.");
            }

            _context.Roles.Remove(role);
            _context.SaveChanges();
            return true;
        }

        // Cập nhật tên vai trò
        public bool UpdateRole(string roleId, RoleVM newRole)
        {
            var role = _context.Roles.FirstOrDefault(r => r.RoleId == roleId);
            if (role == null)
            {
                throw new KeyNotFoundException("Vai trò không tồn tại.");
            }

            if (_context.Roles.Any(r => r.RoleName == newRole.RoleName))
            {
                throw new InvalidOperationException("Tên vai trò mới đã tồn tại.");
            }

            role.RoleName = newRole.RoleName;
            role.ThongTin = newRole.ThongTin;
            _context.SaveChanges();
            return true;
        }

        #endregion

        #region User

        public List<Models.User> GetAllUser()
        {
            var users = (from user in _context.Users
                         join userRole in _context.UserRoles on user.UserId equals userRole.UserId into userRoles
                         from userRole in userRoles.DefaultIfEmpty() // Left Join: Lấy tất cả User kể cả không có Role
                         join role in _context.Roles on userRole.RoleId equals role.RoleId into roles
                         from role in roles.DefaultIfEmpty() // Left Join: Lấy tất cả User kể cả không có Role
                         select new
                         {
                             user.UserId,
                             user.Username,
                             user.Password,
                             user.Ten,
                             user.Sdt,
                             user.Diachi,
                             RoleID = role.RoleId,
                             RoleName = role != null ? role.RoleName : null,
                             ThongTin = role != null ? role.ThongTin : null
                         })
                         .GroupBy(u => u.UserId) // Nhóm theo UserId
                         .Select(g => new Models.User
                         {
                             UserID = g.Key,
                             Username = g.FirstOrDefault().Username,
                             Password = g.FirstOrDefault().Password,
                             Name = g.FirstOrDefault().Ten, // Fixed missing Name assignment
                             SDT = g.FirstOrDefault().Sdt, // Fixed missing SDT assignment
                             Diachi = g.FirstOrDefault().Diachi, // Fixed missing Diachi assignment
                             Role = g.Where(x => x.RoleName != null) // Chỉ lấy các role có giá trị
                                       .Select(x => new Models.Role
                                       {
                                           RoleID=x.RoleID,
                                           RoleName = x.RoleName,
                                           ThongTin = x.ThongTin
                                       }).ToList()
                         })
                         .ToList();

            return users;
        }



        public void AddUser(UserAddVM user)
        {
            try
            {
                // Kiểm tra dữ liệu người dùng hợp lệ
                if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                {
                    throw new InvalidOperationException("Dữ liệu người dùng không hợp lệ");
                }

                // Kiểm tra xem tài khoản đã tồn tại chưa
                var existingUser = _context.Users
                    .FirstOrDefault(ur => ur.Username == user.Username);

                if (existingUser != null)
                {
                    throw new InvalidOperationException("Tài khoản đã tồn tại.");
                }

                // Kiểm tra số điện thoại đã tồn tại chưa
                var existingPhone = _context.Users
                    .FirstOrDefault(ur => ur.Sdt == user.Sdt);

                if (existingPhone != null)
                {
                    throw new InvalidOperationException("Số điện thoại đã tồn tại.");
                }

                // Tạo người dùng mới
                var newUser = new Data.User
                {
                    UserId = Guid.NewGuid().ToString(),
                    Username = user.Username,
                    Password = HashPassword(user.Password), // Mã hóa mật khẩu
                    Ten = user.Ten,
                    Sdt = user.Sdt,
                    Diachi = user.Diachi
                };

                // Thêm vào cơ sở dữ liệu
                _context.Users.Add(newUser);
                _context.SaveChanges(); // Lưu vào cơ sở dữ liệu
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu có
                _logger.LogError($"Lỗi khi thêm người dùng: {ex.Message}", ex);
                throw; // Ném lại ngoại lệ để controller có thể xử lý
            }
        }



        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool DeleteUser(string userID)
        {
            try
            {
                // Tìm UserRole theo userId
                var users = _context.Users.Where(ur => ur.UserId.ToString() == userID).ToList();

                if (!users.Any())
                {
                    return false; // Không tìm thấy userId trong UserRole
                }

                // Xóa tất cả UserRole liên quan đến userId
                _context.Users.RemoveRange(users);
                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                return true;
            }
            catch (Exception)
            {
                return false; // Xóa thất bại
            }
        }
        public List<Models.UserRole> GetUserByRole(string RoleID)
        {
            var userbyrole= _context.UserRoles
                .Where(r => r.RoleId == RoleID)
                .Select(r => new Models.UserRole
            {
                UserID = r.UserId,
                Username = r.User.Username,
                Name = _context.Users
                            .Where(kh => kh.UserId == r.UserId)
                            .Select(kh => kh.Ten)
                            .FirstOrDefault(),
                SDT = _context.Users
                            .Where(kh => kh.UserId == r.UserId)
                            .Select(kh => kh.Sdt)
                            .FirstOrDefault(),
                Role = new List<Models.Role>
                {
                    new Models.Role
                    {
                        RoleID = r.RoleId,
                        RoleName = r.Role.RoleName,
                        ThongTin = r.Role.ThongTin
                    }

                }
                }).ToList();

            return userbyrole;
        }
        public List<Models.UserRole> GetAllUserRole()
        {
            // Lấy danh sách người dùng và vai trò, nhóm theo UserId
            var userRoles = _context.UserRoles
                .GroupBy(r => r.UserId)
                .Select(group => new Models.UserRole
                {
                    UserID = group.Key,
                    Username = group.FirstOrDefault().User.Username,
                    Name = _context.Users
                                .Where(kh => kh.UserId == group.Key)
                                .Select(kh => kh.Ten)
                                .FirstOrDefault(),
                    SDT = _context.Users
                                .Where(kh => kh.UserId == group.Key)
                                .Select(kh => kh.Sdt)
                                .FirstOrDefault(),
                    Role = group.Select(r => new Models.Role
                    {
                        RoleID = r.RoleId,
                        RoleName = r.Role.RoleName,
                        ThongTin = r.Role.ThongTin
                    }).ToList()
                }).ToList();

            return userRoles;
        }


        public bool DeleteUserRole(UserRoleAdd request)
        {
            try
            {
                // Tìm UserRole theo userId
                var userRoles = _context.UserRoles.Where(ur => ur.UserId == request.UserID && ur.RoleId == request.RoleID).ToList();

                if (!userRoles.Any())
                {
                    return false; // Không tìm thấy userId trong UserRole
                }

                // Xóa tất cả UserRole liên quan đến userId
                _context.UserRoles.RemoveRange(userRoles);
                _context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                return true;
            }
            catch (Exception ex)
            {
                return false; // Xóa thất bại
            }
        }

        public string AddUserRole(UserRoleAdd request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserID) || string.IsNullOrWhiteSpace(request.RoleID))
                {
                    return "Dữ liệu UserRole không hợp lệ.";
                }

                // Kiểm tra xem UserRole đã tồn tại chưa
                var existingUserRole = _context.UserRoles
                    .FirstOrDefault(ur => ur.UserId == request.UserID && ur.RoleId == request.RoleID);

                if (existingUserRole != null)
                {
                    return "UserRole đã tồn tại.";
                }

                // Thêm mới UserRole
                var newUserRole = new Data.UserRole
                {
                    UserId = request.UserID,
                    RoleId = request.RoleID,
                    GhiChu = "" // Ghi chú mặc định rỗng
                };

                _context.UserRoles.Add(newUserRole);
                _context.SaveChanges(); // Lưu vào cơ sở dữ liệu

                return "Thêm UserRole thành công.";
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                _logger.LogError($"Lỗi khi thêm UserRole: {ex.Message}", ex); // Sử dụng ILogger để ghi log thay vì Console.WriteLine
                return "Có lỗi xảy ra khi thêm UserRole.";
            }
        }


        #endregion

        #region RoleAuth

        public List<Models.RoleAuth> GetAllRoleAuth()
        {
            return _context.RoleAuths.Select(r => new Models.RoleAuth
            {
                RoleID = r.RoleId,
                AuthID = r.AuthId,
                RoleName = r.Role.RoleName,
                AuthInfo = new Models.AuthInfo
                {
                    ActionId=r.Auth.ActionId,
                    ServiceId=r.Auth.ServiceId,
                    ActionName=r.Auth.Action.ActionName,
                    ServiceName=r.Auth.Service.ServiceName
                }
            }).ToList();
        }
        public List<Models.AuthByRole> GetAuthByRole(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return new List<Models.AuthByRole>();
            }

            return _context.RoleAuths
                .Where(ra => ra.RoleId == roleId)  // Lọc theo RoleId
                .Select(ra => new Models.AuthByRole
                {
                    RoleID = ra.RoleId,
                    AuthID = ra.AuthId,
                    AuthInfo = new Models.AuthInfo
                    {
                        ActionId = ra.Auth.ActionId,
                        ServiceId = ra.Auth.ServiceId,
                        ActionName = ra.Auth.Action.ActionName,  // Lấy tên hành động từ bảng Action
                        ServiceName = ra.Auth.Service.ServiceName  // Lấy tên dịch vụ từ bảng Service
                    }
                })
                .ToList();
        }


        public async Task AddRoleAuthAsync(string roleId, string authId)
        {
            // Kiểm tra xem Role có tồn tại hay không
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                throw new ArgumentException("Role không tồn tại.");
            }

            // Kiểm tra xem AuthID có tồn tại trong ActionService hay không
            var auth = await _context.ActionServices.FindAsync(authId);
            if (auth == null)
            {
                throw new ArgumentException("AuthID không tồn tại.");
            }

            // Kiểm tra nếu RoleAuth đã tồn tại để tránh trùng lặp
            var existingRoleAuth = await _context.RoleAuths
                .FirstOrDefaultAsync(ra => ra.RoleId == roleId && ra.AuthId == authId);
            if (existingRoleAuth != null)
            {
                throw new InvalidOperationException("RoleAuth này đã tồn tại.");
            }

            // Tạo mới RoleAuth
            var roleAuth = new Data.RoleAuth
            {
                RoleId = roleId,
                AuthId = authId, // Sử dụng AuthID từ ActionService
                GhiChu = ""
            };

            // Thêm vào DB
            _context.RoleAuths.Add(roleAuth);
            await _context.SaveChangesAsync();
        }

        // Phương thức xóa RoleAuth dựa trên RoleID và AuthID
    public async Task DeleteRoleAuthAsync(string roleId, string authId)
    {
        // Tìm RoleAuth cần xóa
        var roleAuth = await _context.RoleAuths
            .FirstOrDefaultAsync(ra => ra.RoleId == roleId && ra.AuthId == authId);

        if (roleAuth == null)
        {
            throw new InvalidOperationException("RoleAuth không tồn tại.");
        }

        // Xóa RoleAuth
        _context.RoleAuths.Remove(roleAuth);
        await _context.SaveChangesAsync();
    }

        #endregion
    }
}

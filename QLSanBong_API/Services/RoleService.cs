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
                         join userRole in _context.UserRoles on user.UserId equals userRole.UserId
                         join role in _context.Roles on userRole.RoleId equals role.RoleId
                         select new Models.User
                         {
                             UserID = user.UserId,
                             Username = user.Username,
                             Password = user.Password,
                             RoleName = role.RoleName  // Lấy RoleName từ bảng Roles
                         }).ToList();

            return users;
        }


        public List<Models.UserRoleVM> GetUserByRole(string RoleID)
        {
            var userbyrole= _context.UserRoles
                .Where(r => r.RoleId == RoleID)
                .Select(r => new UserRoleVM
            {
                Username = r.User.Username,
                Name = _context.KhachHangs
                            .Where(kh => kh.UserId == r.UserId)
                            .Select(kh => kh.TenKh)
                            .FirstOrDefault()
                        ?? _context.NhanViens
                            .Where(nv => nv.UserId == r.UserId)
                            .Select(nv => nv.TenNv)
                            .FirstOrDefault(),
                SDT = _context.KhachHangs
                            .Where(kh => kh.UserId == r.UserId)
                            .Select(kh => kh.Sdt)
                            .FirstOrDefault()
                        ?? _context.NhanViens
                            .Where(nv => nv.UserId == r.UserId)
                            .Select(nv => nv.Sdt)
                            .FirstOrDefault(),
                RoleName = r.Role.RoleName,
                ThongTin = r.Role.ThongTin
            }).ToList();

            return userbyrole;
        }
        public List<Models.UserRole> GetAllUserRole()
        {
            return _context.UserRoles.Select(r => new Models.UserRole
            {
                UserID = r.UserId,
                RoleID = r.RoleId,
                Username = r.User.Username,
                Name = _context.KhachHangs
                            .Where(kh => kh.UserId == r.UserId)
                            .Select(kh => kh.TenKh)
                            .FirstOrDefault()
                        ?? _context.NhanViens
                            .Where(nv => nv.UserId == r.UserId)
                            .Select(nv => nv.TenNv)
                            .FirstOrDefault(),
                SDT = _context.KhachHangs
                            .Where(kh => kh.UserId == r.UserId)
                            .Select(kh => kh.Sdt)
                            .FirstOrDefault()
                        ?? _context.NhanViens
                            .Where(nv => nv.UserId == r.UserId)
                            .Select(nv => nv.Sdt)
                            .FirstOrDefault(),
                RoleName = r.Role.RoleName,
                ThongTin = r.Role.ThongTin
            }).ToList();
        }

        public bool DeleteUserRole(string userId, string roleId)
        {
            try
            {
                // Tìm UserRole theo userId
                var userRoles = _context.UserRoles.Where(ur => ur.UserId == userId && ur.RoleId == roleId).ToList();

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

        public string AddUserRole(string userId, string roleId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
                {
                    return "Dữ liệu UserRole không hợp lệ.";
                }

                // Kiểm tra xem UserRole đã tồn tại chưa
                var existingUserRole = _context.UserRoles
                    .FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (existingUserRole != null)
                {
                    return "UserRole đã tồn tại.";
                }

                // Thêm mới UserRole
                var newUserRole = new Data.UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                };

                _context.UserRoles.Add(newUserRole);
                _context.SaveChanges(); // Lưu vào cơ sở dữ liệu

                return "Thêm UserRole thành công.";
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                Console.WriteLine(ex.Message);
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

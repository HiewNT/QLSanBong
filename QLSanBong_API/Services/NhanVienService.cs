using QLSanBong_API.Data;
using QLSanBong_API.Models;

namespace QLSanBong_API.Services
{
    public class NhanVienService : INhanVienService
    {
        private readonly QlsanBongContext _context;

        public NhanVienService(QlsanBongContext context)
        {
            _context = context;
        }

        public List<Models.NhanVien> GetAll()
        {
            var nhanViens = _context.Users
                .Join(_context.UserRoles,
                    user => user.UserId,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole })
                .Join(_context.Roles,
                    combined => combined.userRole.RoleId,
                    role => role.RoleId,
                    (combined, role) => new { combined.user, role })
                .Where(x => x.role.RoleName == "NhanVienDS")
                .Select(x => new Models.NhanVien
                {
                    UserID = x.user.UserId,
                    TenNv = x.user.Ten, // Giả sử `Username` đại diện cho tên nhân viên
                    Chucvu = x.role.RoleName,
                    Diachi = x.user.Diachi,
                    Sdt = x.user.Sdt, // Thêm trường `Phone` trong Users nếu cần
                    User = new Models.User
                    {
                        UserID = x.user.UserId,
                        Username = x.user.Username,
                        Password = x.user.Password,
                        RoleVM = _context.UserRoles
                            .Where(ur => ur.UserId == x.user.UserId)
                            .Join(
                                _context.Roles,
                                ur => ur.RoleId,
                                r => r.RoleId,
                                (ur, r) => new RoleVM
                                {
                                    RoleName = r.RoleName,
                                    ThongTin = r.ThongTin
                                })
                            .ToList()
                    }
                })
                .ToList();

            return nhanViens;
        }

        public Models.NhanVien? GetById(string id)
        {
            var nhanVien = _context.Users
                .Join(_context.UserRoles,
                      u => u.UserId,
                      ur => ur.UserId,
                      (u, ur) => new { User = u, UserRole = ur })
                .Join(_context.Roles,
                      ur => ur.UserRole.RoleId,
                      r => r.RoleId,
                      (ur, r) => new { ur.User, Role = r })
                .FirstOrDefault(ur => ur.User.UserId == id && ur.Role.RoleName == "NhanVienDS");

            if (nhanVien == null) return null;

            return new Models.NhanVien
            {
                UserID = nhanVien.User.UserId,
                TenNv = nhanVien.User.Ten,
                Sdt = nhanVien.User.Sdt,
                Diachi = nhanVien.User.Diachi,
                User = new Models.User
                {
                    UserID = nhanVien.User.UserId,
                    Username = nhanVien.User.Username,
                    Password = nhanVien.User.Password,
                    RoleVM = new List<RoleVM>
                    {
                        new RoleVM
                        {
                            RoleName = nhanVien.Role.RoleName,
                            ThongTin = nhanVien.Role.ThongTin
                        }
                    }
                }
            };
        }

        public void Add(NhanVienVM nhanVienVM)
        {
            if (_context.Users.Any(tk => tk.Username == nhanVienVM.User.Username))
                throw new InvalidOperationException("Tài khoản đã tồn tại.");

            if (_context.Users.Any(kh => kh.Sdt == nhanVienVM.Sdt))
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");

            var newUserId = Guid.NewGuid().ToString();

            // Tạo người dùng mới
            var userEntity = new Data.User
            {
                UserId = newUserId,
                Username = nhanVienVM.User.Username,
                Password = HashPassword(nhanVienVM.User?.Password ?? string.Empty),
                Ten = nhanVienVM.TenNv,
                Sdt = nhanVienVM.Sdt,
                Diachi = nhanVienVM.Diachi
            };

            // Gán vai trò NhanVien
            var roleEntity = _context.Roles.SingleOrDefault(r => r.RoleName == "NhanVienDS");
            if (roleEntity == null)
                throw new InvalidOperationException("Không tìm thấy vai trò NhanVienDS.");

            var userRoleEntity = new Data.UserRole
            {
                UserId = newUserId,
                RoleId = roleEntity.RoleId
            };

            _context.Users.Add(userEntity);
            _context.UserRoles.Add(userRoleEntity);
            _context.SaveChanges();
        }

        public void Update(string id, NhanVienVM nhanVienVM)
        {
            var nhanVien = _context.Users.SingleOrDefault(kh => kh.UserId == id);
            if (nhanVien == null)
                throw new KeyNotFoundException("Khách hàng không tồn tại.");

            // Kiểm tra trùng lặp tài khoản (username), ngoại trừ chính khách hàng này
            if (_context.Users.Any(u => u.Username == nhanVienVM.User.Username && u.UserId != id))
                throw new InvalidOperationException("Tài khoản đã tồn tại.");

            // Kiểm tra trùng lặp số điện thoại, ngoại trừ chính khách hàng này
            if (_context.Users.Any(u => u.Sdt == nhanVienVM.Sdt && u.UserId != id))
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");

            // Cập nhật thông tin khách hàng
            nhanVien.Ten = nhanVienVM.TenNv;
            nhanVien.Sdt = nhanVienVM.Sdt;
            nhanVien.Diachi = nhanVienVM.Diachi;
            nhanVien.Username = nhanVienVM.User.Username;

            // Nếu có mật khẩu mới, băm mật khẩu và cập nhật
            if (!string.IsNullOrEmpty(nhanVienVM.User?.Password))
                nhanVien.Password = HashPassword(nhanVienVM.User.Password);

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserId == id);
            if (user == null)
            {
                throw new KeyNotFoundException("Nhân viên không tồn tại.");
            }

            var userRoles = _context.UserRoles.Where(ur => ur.UserId == id).ToList();
            _context.UserRoles.RemoveRange(userRoles);

            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using QLSanBong_API.Data;
using QLSanBong_API.Models;

namespace QLSanBong_API.Services
{
    public class KhachHangService : IKhachHangService
    {
        private readonly QlsanBongContext _context;

        public KhachHangService(QlsanBongContext context)
        {
            _context = context;
        }

        // Lấy danh sách khách hàng (RoleName là KhachHang)
        public List<Models.KhachHang> GetAll()
        {
            var khachHangData = _context.Users
                .Join(_context.UserRoles,
                      u => u.UserId,
                      ur => ur.UserId,
                      (u, ur) => new { User = u, UserRole = ur })
                .Join(_context.Roles,
                      ur => ur.UserRole.RoleId,
                      r => r.RoleId,
                      (ur, r) => new { ur.User, Role = r })
                .Where(ur => ur.Role.RoleName == "KhachHang") // Chỉ lấy khách hàng
                .ToList();

            var khachHangModels = khachHangData.Select(kh => new Models.KhachHang
            {
                UserID = kh.User.UserId,
                TenKh = kh.User.Ten,
                Sdt = kh.User.Sdt,
                Diachi = kh.User.Diachi,
                User = new Models.User
                {
                    UserID = kh.User.UserId,
                    Username = kh.User.Username,
                    Password = kh.User.Password,
                    RoleVM = new List<RoleVM>
                    {
                        new RoleVM
                        {
                            RoleName = kh.Role.RoleName,
                            ThongTin = kh.Role.ThongTin
                        }
                    }
                }
            }).ToList();

            return khachHangModels;
        }

        // Lấy thông tin khách hàng theo ID
        public Models.KhachHang? GetById(string id)
        {
            var khachHang = _context.Users
                .Join(_context.UserRoles,
                      u => u.UserId,
                      ur => ur.UserId,
                      (u, ur) => new { User = u, UserRole = ur })
                .Join(_context.Roles,
                      ur => ur.UserRole.RoleId,
                      r => r.RoleId,
                      (ur, r) => new { ur.User, Role = r })
                .FirstOrDefault(ur => ur.User.UserId == id && ur.Role.RoleName == "KhachHang");

            if (khachHang == null) return null;

            return new Models.KhachHang
            {
                UserID = khachHang.User.UserId,
                TenKh = khachHang.User.Ten,
                Sdt = khachHang.User.Sdt,
                Diachi = khachHang.User.Diachi,
                User = new Models.User
                {
                    UserID = khachHang.User.UserId,
                    Username = khachHang.User.Username,
                    Password = khachHang.User.Password,
                    RoleVM = new List<RoleVM>
                    {
                        new RoleVM
                        {
                            RoleName = khachHang.Role.RoleName,
                            ThongTin = khachHang.Role.ThongTin
                        }
                    }
                }
            };
        }

        // Thêm khách hàng mới
        public void Add(KhachHangVM khachHangVM)
        {
            if (_context.Users.Any(tk => tk.Username == khachHangVM.User.Username))
                throw new InvalidOperationException("Tài khoản đã tồn tại.");

            if (_context.Users.Any(kh => kh.Sdt == khachHangVM.Sdt))
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");

            var newUserId = Guid.NewGuid().ToString();

            // Tạo người dùng mới
            var userEntity = new Data.User
            {
                UserId = newUserId,
                Username = khachHangVM.User.Username,
                Password = HashPassword(khachHangVM.User?.Password ?? string.Empty),
                Ten = khachHangVM.TenKh,
                Sdt = khachHangVM.Sdt,
                Diachi = khachHangVM.Diachi
            };

            // Gán vai trò KhachHang
            var roleEntity = _context.Roles.SingleOrDefault(r => r.RoleName == "KhachHang");
            if (roleEntity == null)
                throw new InvalidOperationException("Không tìm thấy vai trò KhachHang.");

            var userRoleEntity = new Data.UserRole
            {
                UserId = newUserId,
                RoleId = roleEntity.RoleId
            };

            _context.Users.Add(userEntity);
            _context.UserRoles.Add(userRoleEntity);
            _context.SaveChanges();
        }

        public void Update(string id, KhachHangVM khachHangVM)
        {
            var khachHang = _context.Users.SingleOrDefault(kh => kh.UserId == id);
            if (khachHang == null)
                throw new KeyNotFoundException("Khách hàng không tồn tại.");

            // Kiểm tra trùng lặp tài khoản (username), ngoại trừ chính khách hàng này
            if (_context.Users.Any(u => u.Username == khachHangVM.User.Username && u.UserId != id))
                throw new InvalidOperationException("Tài khoản đã tồn tại.");

            // Kiểm tra trùng lặp số điện thoại, ngoại trừ chính khách hàng này
            if (_context.Users.Any(u => u.Sdt == khachHangVM.Sdt && u.UserId != id))
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");

            // Cập nhật thông tin khách hàng
            khachHang.Ten = khachHangVM.TenKh;
            khachHang.Sdt = khachHangVM.Sdt;
            khachHang.Diachi = khachHangVM.Diachi;
            khachHang.Username = khachHangVM.User.Username;

            // Nếu có mật khẩu mới, băm mật khẩu và cập nhật
            if (!string.IsNullOrEmpty(khachHangVM.User?.Password))
                khachHang.Password = HashPassword(khachHangVM.User.Password);

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();
        }


        // Xóa khách hàng
        public void Delete(string id)
        {
            var khachHang = _context.Users.SingleOrDefault(kh => kh.UserId == id);
            if (khachHang == null)
                throw new KeyNotFoundException("Khách hàng không tồn tại.");

            var userRole = _context.UserRoles.SingleOrDefault(ur => ur.UserId == id);
            if (userRole != null)
                _context.UserRoles.Remove(userRole);

            _context.Users.Remove(khachHang);
            _context.SaveChanges();
        }

        // Hàm băm mật khẩu
        private string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    }
}

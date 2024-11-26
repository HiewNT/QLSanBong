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

        public List<Models.KhachHang> GetAll()
        {
            var khachHangsData = _context.KhachHangs.ToList();

            var khachHangsModels = khachHangsData.Select(kh => new Models.KhachHang
            {
                MaKh = kh.MaKh,
                TenKh = kh.TenKh,
                Sdt = kh.Sdt,
                Gioitinh = kh.Gioitinh,
                Diachi = kh.Diachi,
                UserID = kh.UserId,
                User = _context.Users
                .Where(u => u.UserId == kh.UserId)
                .Select(u => new Models.User
                {
                    UserID = u.UserId,
                    Username = u.Username,
                    Password = u.Password,
                    RoleVM = _context.UserRoles
                                .Where(ur => ur.UserId == u.UserId)
                                .Join(
                                    _context.Roles,
                                    ur => ur.RoleId,
                                    r => r.RoleId,
                                    (ur, r) => new RoleVM
                                    {
                                        RoleName = r.RoleName,  // Lấy tên vai trò
                                        ThongTin = r.ThongTin  // Lấy thông tin thêm nếu cần
                                    })
                                .ToList()  // Lấy tất cả vai trò của người dùng
                })
                .FirstOrDefault()
            }).ToList();


            return khachHangsModels;
        }


        public Models.KhachHang? GetById(string id)
        {
            var KhachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == id);
            if (KhachHang == null)
            {
                return null;
            }

            return new Models.KhachHang
            {
                MaKh = KhachHang.MaKh,
                TenKh = KhachHang.TenKh,
                Sdt = KhachHang.Sdt,
                Gioitinh = KhachHang.Gioitinh,
                Diachi = KhachHang.Diachi,
                UserID = KhachHang.UserId,
                User = _context.Users
                    .Where(u => u.UserId == KhachHang.UserId)
                    .Select(u => new Models.User
                    {
                        UserID = u.UserId,
                        Username = u.Username,
                        Password = u.Password,
                        RoleVM = _context.UserRoles
                                    .Where(ur => ur.UserId == u.UserId)
                                    .Join(
                                        _context.Roles,
                                        ur => ur.RoleId,
                                        r => r.RoleId,
                                        (ur, r) => new RoleVM
                                        {
                                            RoleName = r.RoleName,  // Lấy tên vai trò
                                            ThongTin = r.ThongTin  // Lấy thông tin thêm nếu cần
                                        })
                                    .ToList()  // Lấy tất cả vai trò của người dùng
                    })
                    .FirstOrDefault()
            };

        }


        public void Add(KhachHangVM KhachHangVM)
        {
            if (_context.Users.Any(tk => tk.Username == KhachHangVM.UserID))
            {
                throw new InvalidOperationException("Tài khoản đã tồn tại.");
            }

            if (_context.KhachHangs.Any(kh => kh.Sdt == KhachHangVM.Sdt))
            {
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");
            }

            string newMaKh = "KH00001";
            if (_context.KhachHangs.Any())
            {
                var maxMaKh = _context.KhachHangs
                    .Where(m => m.MaKh.StartsWith("KH"))
                    .OrderByDescending(m => m.MaKh)
                    .Select(m => m.MaKh)
                    .FirstOrDefault();

                if (maxMaKh != null)
                {
                    int currentNumber = int.Parse(maxMaKh.Substring(2));
                    newMaKh = $"KH{(currentNumber + 1):D5}";
                }
            }

            // Tạo User mới
            var newUserId = Guid.NewGuid().ToString();
            var userEntity = new Data.User
            {
                UserId = newUserId,
                Username = KhachHangVM.User?.Username,
                Password = HashPassword(KhachHangVM.User?.Password ?? string.Empty)
            };

            // Tìm RoleID cho RoleName "KhachHang"
            var roleEntity = _context.Roles.SingleOrDefault(r => r.RoleName == "KhachHang");
            if (roleEntity == null)
            {
                throw new InvalidOperationException("Không tìm thấy vai trò KhachHang.");
            }

            // Tạo UserRole mới
            var userRoleEntity = new Data.UserRole
            {
                UserId = newUserId,
                RoleId = roleEntity.RoleId
            };

            // Tạo KhachHang mới
            var KhachHang = new Data.KhachHang
            {
                MaKh = newMaKh,
                TenKh = KhachHangVM.TenKh,
                Sdt = KhachHangVM.Sdt,
                Gioitinh = KhachHangVM.Gioitinh,
                Diachi = KhachHangVM.Diachi,
                UserId = newUserId
            };

            _context.Users.Add(userEntity);
            _context.UserRoles.Add(userRoleEntity);
            _context.KhachHangs.Add(KhachHang);
            _context.SaveChanges();
        }


        public void Update(string id, KhachHangVM KhachHangVM)
        {
            var KhachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == id);
            if (KhachHang == null)
            {
                throw new KeyNotFoundException("Khách hàng không tồn tại.");
            }

            if (_context.Users.Any(tk => tk.Username == KhachHangVM.UserID && tk.Username != KhachHang.UserId))
            {
                throw new InvalidOperationException("Tài khoản đã tồn tại.");
            }

            if (_context.KhachHangs.Any(kh => kh.Sdt == KhachHangVM.Sdt && kh.MaKh != id))
            {
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");
            }

            KhachHang.TenKh = KhachHangVM.TenKh;
            KhachHang.Sdt = KhachHangVM.Sdt;
            KhachHang.Gioitinh = KhachHangVM.Gioitinh;
            KhachHang.Diachi = KhachHangVM.Diachi;

            var taiKhoan = _context.Users.FirstOrDefault(tk => tk.UserId == KhachHang.UserId);
            if (taiKhoan != null && KhachHangVM.User != null)
            {
                taiKhoan.Username = KhachHangVM.UserID;
                if (!string.IsNullOrEmpty(KhachHangVM.User.Password))
                {
                    taiKhoan.Password = HashPassword(KhachHangVM.User.Password);
                }

                // Cập nhật RoleName qua UserRole
                var roleEntity = _context.Roles.SingleOrDefault(r => r.RoleName == "KhachHang");
                if (roleEntity == null)
                {
                    throw new InvalidOperationException("Không tìm thấy vai trò KhachHang.");
                }

                var userRole = _context.UserRoles.FirstOrDefault(ur => ur.UserId == taiKhoan.UserId);
                if (userRole != null)
                {
                    userRole.RoleId = roleEntity.RoleId;
                }
                else
                {
                    // Nếu UserRole không tồn tại, tạo mới
                    _context.UserRoles.Add(new Data.UserRole
                    {
                        UserId = taiKhoan.UserId,
                        RoleId = roleEntity.RoleId
                    });
                }
            }

            _context.SaveChanges();
        }


        public void Delete(string id)
        {
            // Tìm khách hàng theo mã
            var khachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == id);
            if (khachHang == null)
            {
                throw new KeyNotFoundException("Khách hàng không tồn tại.");
            }

            // Xóa khách hàng
            _context.KhachHangs.Remove(khachHang);

            // Tìm tài khoản liên kết và xóa nếu tồn tại
            var taiKhoan = _context.Users.SingleOrDefault(tk => tk.UserId == khachHang.UserId);
            if (taiKhoan != null)
            {
                _context.Users.Remove(taiKhoan);
            }
            var tkrole = _context.UserRoles.SingleOrDefault(tr => tr.UserId == khachHang.UserId);
            if (tkrole != null){
                _context.UserRoles.Remove(tkrole);
            }
            // Lưu thay đổi
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Không thể xóa khách hàng do lỗi cơ sở dữ liệu.", ex);
            }
        }


        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

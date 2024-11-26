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
            var nhanViensData = _context.NhanViens.ToList();

            var nhanViensModels = nhanViensData.Select(nv => new Models.NhanVien
            {
                MaNv = nv.MaNv,
                TenNv = nv.TenNv,
                Chucvu = nv.Chucvu,
                Sdt = nv.Sdt,
                UserID = nv.UserId,
                User = _context.Users
                .Where(u => u.UserId == nv.UserId)
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
                                        RoleName = r.RoleName,
                                        ThongTin = r.ThongTin // Gán giá trị ThongTin từ bảng Roles
                                    })
                                .ToList() // Lấy tất cả các vai trò cho người dùng
                })
                .FirstOrDefault()
            }).ToList();


            return nhanViensModels;
        }

        public Models.NhanVien? GetById(string id)
        {
            var NhanVien = _context.NhanViens.SingleOrDefault(nv => nv.MaNv == id);
            if (NhanVien == null)
            {
                return null;
            }

            return new Models.NhanVien
            {
                MaNv = NhanVien.MaNv,
                TenNv = NhanVien.TenNv,
                Chucvu = NhanVien.Chucvu,
                Sdt = NhanVien.Sdt,
                UserID = NhanVien.UserId,
                User = _context.Users
                    .Where(u => u.UserId == NhanVien.UserId)
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
                                            ThongTin = r.ThongTin  // Thêm thông tin về vai trò nếu cần
                                        })
                                    .ToList()  // Lấy tất cả vai trò của người dùng
                    })
                    .FirstOrDefault()
            };

        }

        public void Add(NhanVienVM NhanVienVM)
        {
            if (_context.Users.Any(tk => tk.UserId == NhanVienVM.UserID))
            {
                throw new InvalidOperationException("Tài khoản đã tồn tại.");
            }

            string newMaNv = "NV00001";
            if (_context.NhanViens.Any())
            {
                var maxMaNv = _context.NhanViens
                    .Where(m => m.MaNv.StartsWith("NV"))
                    .OrderByDescending(m => m.MaNv)
                    .Select(m => m.MaNv)
                    .FirstOrDefault();

                if (maxMaNv != null)
                {
                    int currentNumber = int.Parse(maxMaNv.Substring(2));
                    newMaNv = $"NV{(currentNumber + 1):D5}";
                }
            }

            // Tạo User mới
            var newUserId = Guid.NewGuid().ToString();
            var userEntity = new Data.User
            {
                UserId = newUserId,
                Username = NhanVienVM.User?.Username,
                Password = HashPassword(NhanVienVM.User?.Password ?? string.Empty)
            };

            // Tìm RoleID cho RoleName "NhanVien"
            var roleEntity = _context.Roles.SingleOrDefault(r => r.RoleName == "NhanVienDS");
            if (roleEntity == null)
            {
                throw new InvalidOperationException("Không tìm thấy vai trò NhanVien.");
            }

            // Tạo UserRole mới
            var userRoleEntity = new Data.UserRole
            {
                UserId = newUserId,
                RoleId = roleEntity.RoleId
            };

            // Tạo NhanVien mới
            var NhanVien = new Data.NhanVien
            {
                MaNv = newMaNv,
                TenNv = NhanVienVM.TenNv,
                Chucvu = NhanVienVM.Chucvu,
                Sdt = NhanVienVM.Sdt,
                UserId = newUserId
            };

            _context.Users.Add(userEntity);
            _context.UserRoles.Add(userRoleEntity);
            _context.NhanViens.Add(NhanVien);
            _context.SaveChanges();
        }

        public void Update(string id, NhanVienVM NhanVienVM)
        {
            // Tìm nhân viên trong cơ sở dữ liệu
            var NhanVien = _context.NhanViens.SingleOrDefault(nv => nv.MaNv == id);
            if (NhanVien == null)
            {
                throw new KeyNotFoundException("Nhân viên không tồn tại.");
            }

            // Cập nhật thông tin nhân viên: tên, chức vụ, số điện thoại
            NhanVien.TenNv = NhanVienVM.TenNv;
            NhanVien.Chucvu = NhanVienVM.Chucvu;
            NhanVien.Sdt = NhanVienVM.Sdt;

            // Cập nhật thông tin tài khoản người dùng
            var taiKhoan = _context.Users.FirstOrDefault(tk => tk.UserId == NhanVien.UserId);
            if (taiKhoan != null && NhanVienVM.User != null)
            {
                // Cập nhật tên đăng nhập nếu có thay đổi
                if (NhanVienVM.User.Username != taiKhoan.Username)
                {
                    taiKhoan.Username = NhanVienVM.User.Username;
                }

                // Cập nhật mật khẩu nếu có thay đổi
                if (!string.IsNullOrEmpty(NhanVienVM.User.Password))
                {
                    taiKhoan.Password = HashPassword(NhanVienVM.User.Password);
                }
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();
        }


        public void Delete(string id)
        {
            var NhanVien = _context.NhanViens.SingleOrDefault(nv => nv.MaNv == id);
            if (NhanVien == null)
            {
                throw new KeyNotFoundException("Nhân viên không tồn tại.");
            }

            _context.NhanViens.Remove(NhanVien);

            var taiKhoan = _context.Users.SingleOrDefault(tk => tk.UserId == NhanVien.UserId);
            if (taiKhoan != null)
            {
                _context.Users.Remove(taiKhoan);
            }
            var tkrole = _context.UserRoles.SingleOrDefault(tr => tr.UserId == NhanVien.UserId);
            if (tkrole != null)
            {
                _context.UserRoles.Remove(tkrole);
            }

            _context.SaveChanges();
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

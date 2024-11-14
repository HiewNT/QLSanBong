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
            // Lấy tất cả nhân viên từ cơ sở dữ liệu
            var nhanViensData = _context.NhanViens
                                 .Where(nv => nv.MaNv != "NV000") // Điều kiện lọc
                                 .ToList();

            // Chuyển đổi sang Models.NhanVien
            var nhanViensModels = nhanViensData.Select(nv => new Models.NhanVien
            {
                MaNv = nv.MaNv,
                TenNv = nv.TenNv,
                Chucvu = nv.Chucvu,
                Sdt = nv.Sdt,
                Tendangnhap = nv.Tendangnhap,
                TaiKhoan = _context.TaiKhoans
                                .Where(tk => tk.Username == nv.Tendangnhap)
                                .Select(tk => new Models.TaiKhoan
                                {
                                    Username = tk.Username,
                                    Password = tk.Password,
                                    Role = tk.Role
                                })
                                .FirstOrDefault()
            }).ToList();

            return nhanViensModels; // Trả về danh sách nhân viên trong Models
        }



        public Models.NhanVien? GetById(string id)
        {
            var nhanVien = _context.NhanViens.SingleOrDefault(nv => nv.MaNv == id);
            if (nhanVien == null)
            {
                return null;
            }

            return new Models.NhanVien
            {
                MaNv = nhanVien.MaNv,
                TenNv = nhanVien.TenNv,
                Chucvu = nhanVien.Chucvu,
                Sdt = nhanVien.Sdt,
                Tendangnhap = nhanVien.Tendangnhap,
                TaiKhoan = _context.TaiKhoans
                            .Where(tk => tk.Username == nhanVien.Tendangnhap)
                            .Select(tk => new Models.TaiKhoan
                            {
                                Username = tk.Username,
                                Password = tk.Password,
                                Role = tk.Role
                            })
                            .FirstOrDefault()

            };
        }

        public void Add(NhanVienVM nhanVienVM)
        {
            // Kiểm tra xem tên đăng nhập đã tồn tại hay chưa
            if (_context.TaiKhoans.Any(tk => tk.Username == nhanVienVM.Tendangnhap))
            {
                throw new InvalidOperationException("Tài khoản đã tồn tại."); // Hoặc có thể trả về một mã lỗi thích hợp
            }
            // Kiểm tra xem tên đăng nhập đã tồn tại hay chưa
            if (_context.NhanViens.Any(nv => nv.Sdt == nhanVienVM.Sdt))
            {
                throw new InvalidOperationException("Số điện thoại đã tồn tại."); // Hoặc có thể trả về một mã lỗi thích hợp
            }
            // Lấy mã nhân viên lớn nhất hiện có trong CSDL
            string newMaNv = "NV000"; // Mã mặc định nếu chưa có nhân viên nào

            if (_context.NhanViens.Any())
            {
                var maxMaNv = _context.NhanViens
                    .Where(m => m.MaNv.StartsWith("NV"))
                    .OrderByDescending(m => m.MaNv)
                    .Select(m => m.MaNv)
                    .FirstOrDefault();

                if (maxMaNv != null)
                {
                    int currentNumber = int.Parse(maxMaNv.Substring(2)); // Bỏ ký tự 'NV'
                    newMaNv = $"NV{(currentNumber + 1).ToString("D3")}";
                }
            }

            // Tạo mới đối tượng NhanVien
            var nhanVien = new Data.NhanVien
            {
                MaNv = newMaNv,
                TenNv = nhanVienVM.TenNv,
                Chucvu = nhanVienVM.Chucvu,
                Sdt = nhanVienVM.Sdt,
                Tendangnhap = nhanVienVM.Tendangnhap,
            };


            // Tạo tài khoản cho nhân viên
            var taiKhoan = new TaiKhoanVM
            {
                Password = HashPassword(nhanVienVM.TaiKhoan?.Password), // Mã hóa mật khẩu
                Role = nhanVienVM.TaiKhoan?.Role ?? "NhanVien"
            };

            // Giả sử bạn có đối tượng Models.TaiKhoan taiKhoan
            var taiKhoanEntity = new Data.TaiKhoan
            {
                Username = nhanVienVM.Tendangnhap,
                Password = taiKhoan.Password,
                Role = taiKhoan.Role
            };

            _context.TaiKhoans.Add(taiKhoanEntity);
            _context.NhanViens.Add(nhanVien);
            _context.SaveChanges();

        }

        public void Update(string id, NhanVienVM nhanVienVM)
        {
            var nhanVien = _context.NhanViens.SingleOrDefault(nv => nv.MaNv == id);
            if (nhanVien == null)
            {
                throw new KeyNotFoundException("Nhân viên không tồn tại.");
            }

            // Kiểm tra xem tên đăng nhập đã tồn tại hay chưa (ngoại trừ tên đăng nhập của nhân viên hiện tại)
            if (_context.TaiKhoans.Any(tk => tk.Username == nhanVienVM.Tendangnhap && tk.Username != nhanVien.Tendangnhap))
            {
                throw new InvalidOperationException("Tài khoản đã tồn tại.");
            }

            // Kiểm tra xem số điện thoại đã tồn tại hay chưa (ngoại trừ số điện thoại của nhân viên hiện tại)
            if (_context.NhanViens.Any(nv => nv.Sdt == nhanVienVM.Sdt && nv.MaNv != id))
            {
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");
            }

            // Cập nhật thông tin nhân viên
            nhanVien.TenNv = nhanVienVM.TenNv;
            nhanVien.Chucvu = nhanVienVM.Chucvu;
            nhanVien.Sdt = nhanVienVM.Sdt;

            // Lấy tài khoản hiện tại của nhân viên
            var taiKhoanCu = _context.TaiKhoans.FirstOrDefault(tk => tk.Username == nhanVien.Tendangnhap);

            // Nếu tên đăng nhập mới khác với tên đăng nhập cũ
            if (taiKhoanCu != null && taiKhoanCu.Username != nhanVienVM.Tendangnhap)
            {
                // Tạo tài khoản mới
                var newTaiKhoan = new Data.TaiKhoan
                {
                    Username = nhanVienVM.Tendangnhap,
                    Password = HashPassword(nhanVienVM.TaiKhoan.Password), // Mã hóa mật khẩu
                    Role = nhanVienVM.TaiKhoan.Role
                };

                // Lưu tài khoản mới vào cơ sở dữ liệu và cập nhật tên đăng nhập nhân viên
                _context.TaiKhoans.Add(newTaiKhoan);
                nhanVien.Tendangnhap = nhanVienVM.Tendangnhap;

                // Xóa tài khoản cũ
                _context.TaiKhoans.Remove(taiKhoanCu);
            }
            else if (taiKhoanCu != null) // Nếu tài khoản đã tồn tại và không đổi tên đăng nhập
            {
                // Cập nhật thông tin tài khoản hiện có
                taiKhoanCu.Role = nhanVienVM.TaiKhoan.Role;
                if (!string.IsNullOrEmpty(nhanVienVM.TaiKhoan.Password))
                {
                    taiKhoanCu.Password = HashPassword(nhanVienVM.TaiKhoan.Password);
                }
            }

            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var nhanVien = _context.NhanViens.SingleOrDefault(nv => nv.MaNv == id);
            if (nhanVien.MaNv == "NV000")
            {
                throw new InvalidOperationException("Không thể xóa quản trị viên");
            }
            if (nhanVien == null)
            {
                throw new KeyNotFoundException("Nhân viên không tồn tại.");
            }

            _context.NhanViens.Remove(nhanVien);

            // Xóa tài khoản của nhân viên
            var taiKhoan = _context.TaiKhoans.SingleOrDefault(tk => tk.Username == nhanVien.Tendangnhap);
            if (taiKhoan != null)
            {
                _context.TaiKhoans.Remove(taiKhoan);
            }

            _context.SaveChanges();
        }

        // Mã hóa mật khẩu
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}

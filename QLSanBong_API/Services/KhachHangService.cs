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

        /*public List<Models.KhachHang> GetAll(string search, string sortBy, int page = 1, int pageSize = 5)
        {
            // Bắt đầu truy vấn từ danh sách khách hàng
            var allKhachHang = _context.KhachHangs.AsQueryable();

            // Lọc theo tên khách hàng hoặc số điện thoại nếu có tham số search
            if (!string.IsNullOrEmpty(search))
            {
                allKhachHang = allKhachHang.Where(kh =>
                    kh.MaKh.Contains(search) ||
                    kh.TenKh.Contains(search) ||
                    kh.Sdt.Contains(search) ||
                    kh.Diachi.Contains(search)); // Có thể thêm điều kiện lọc khác nếu cần
            }

            // Sắp xếp theo sortBy nếu có
            switch (sortBy)
            {
                case "tenkh_desc":
                    allKhachHang = allKhachHang.OrderByDescending(kh => kh.TenKh);
                    break;
                case "tenkh_asc":
                    allKhachHang = allKhachHang.OrderBy(kh => kh.TenKh);
                    break;
                default: // Mặc định sắp xếp theo mã khách hàng
                    allKhachHang = allKhachHang.OrderBy(kh => kh.MaKh);
                    break;
            }

            // Thực hiện phân trang
            var paginatedList = PaginatedList<Data.KhachHang>.Create(allKhachHang, page, pageSize);

            // Chọn dữ liệu để trả về, bao gồm thông tin tài khoản
            return paginatedList.Select(kh => new Models.KhachHang
            {
                MaKh = kh.MaKh,
                TenKh = kh.TenKh,
                Sdt = kh.Sdt,
                Gioitinh = kh.Gioitinh,
                Diachi = kh.Diachi,
                Tendangnhap = kh.Tendangnhap,
                TaiKhoan = _context.TaiKhoans
                                .Where(tk => tk.Username == kh.Tendangnhap)
                                .Select(tk => new Models.TaiKhoan
                                {
                                    Username = tk.Username,
                                    Password = tk.Password,
                                    Role = tk.Role
                                })
                                .FirstOrDefault()
            }).ToList();
        }
        */

        public List<Models.KhachHang> GetAll()
        {
            // Lấy tất cả nhân viên từ cơ sở dữ liệu
            var khachHangsData = _context.KhachHangs.ToList();

            // Chuyển đổi sang Models.NhanVien
            var khachHangsModels = khachHangsData.Select(kh => new Models.KhachHang
            {
                MaKh = kh.MaKh,
                TenKh = kh.TenKh,
                Sdt = kh.Sdt,
                Gioitinh = kh.Gioitinh,
                Diachi = kh.Diachi,
                Tendangnhap = kh.Tendangnhap,
                TaiKhoan = _context.TaiKhoans
                                .Where(tk => tk.Username == kh.Tendangnhap)
                                .Select(tk => new Models.TaiKhoan
                                {
                                    Username = tk.Username,
                                    Password = tk.Password,
                                    Role = tk.Role
                                })
                                .FirstOrDefault()
            }).ToList();

            return khachHangsModels; // Trả về danh sách nhân viên trong Models
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
                Tendangnhap = KhachHang.Tendangnhap,
                TaiKhoan = _context.TaiKhoans
                            .Where(tk => tk.Username == KhachHang.Tendangnhap)
                            .Select(tk => new Models.TaiKhoan
                            {
                                Username = KhachHang.Tendangnhap,
                                Password = tk.Password,
                                Role = tk.Role
                            })
                            .FirstOrDefault()

            };
        }

        public void Add(KhachHangVM KhachHangVM)
        {
            // Kiểm tra xem tên đăng nhập đã tồn tại hay chưa
            if (_context.TaiKhoans.Any(tk => tk.Username == KhachHangVM.Tendangnhap))
            {
                throw new InvalidOperationException("Tài khoản đã tồn tại."); // Hoặc có thể trả về một mã lỗi thích hợp
            }
            // Kiểm tra xem tên đăng nhập đã tồn tại hay chưa
            if (_context.KhachHangs.Any(kh => kh.Sdt == KhachHangVM.Sdt))
            {
                throw new InvalidOperationException("Số điện thoại đã tồn tại."); // Hoặc có thể trả về một mã lỗi thích hợp
            }
            // Lấy mã Khách hàng lớn nhất hiện có trong CSDL
            string newMaKh = "KH00001"; // Mã mặc định nếu chưa có Khách hàng nào

            if (_context.KhachHangs.Any())
            {
                var maxMaKh = _context.KhachHangs
                    .Where(m => m.MaKh.StartsWith("KH"))
                    .OrderByDescending(m => m.MaKh)
                    .Select(m => m.MaKh)
                    .FirstOrDefault();

                if (maxMaKh != null)
                {
                    int currentNumber = int.Parse(maxMaKh.Substring(2)); // Bỏ ký tự 'kh'
                    newMaKh = $"KH{(currentNumber + 1).ToString("D5")}";
                }
            }

            // Tạo mới đối tượng KhachHang
            var KhachHang = new Data.KhachHang
            {
                MaKh = newMaKh,
                TenKh = KhachHangVM.TenKh,
                Sdt = KhachHangVM.Sdt,
                Gioitinh = KhachHangVM.Gioitinh,
                Diachi = KhachHangVM.Diachi,
                Tendangnhap = KhachHangVM.Tendangnhap
            };


            // Tạo tài khoản cho Khách hàng
            var taiKhoan = new Models.TaiKhoan
            {
                Username = KhachHangVM.Tendangnhap,
                Password = HashPassword(KhachHangVM.TaiKhoan?.Password), // Mã hóa mật khẩu
                Role = KhachHangVM.TaiKhoan?.Role ?? "KhachHang"
            };

            // Giả sử bạn có đối tượng Models.TaiKhoan taiKhoan
            var taiKhoanEntity = new Data.TaiKhoan
            {
                Username = taiKhoan.Username,
                Password = taiKhoan.Password,
                Role = taiKhoan.Role
            };

            _context.TaiKhoans.Add(taiKhoanEntity);
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
            // Kiểm tra xem tên đăng nhập đã tồn tại hay chưa (ngoại trừ tên đăng nhập của nhân viên hiện tại)
            if (_context.TaiKhoans.Any(tk => tk.Username == KhachHangVM.Tendangnhap && tk.Username != KhachHang.Tendangnhap))
            {
                throw new InvalidOperationException("Tài khoản đã tồn tại.");
            }

            // Kiểm tra xem số điện thoại đã tồn tại hay chưa (ngoại trừ số điện thoại của nhân viên hiện tại)
            if (_context.KhachHangs.Any(kh => kh.Sdt == KhachHangVM.Sdt && kh.MaKh != id))
            {
                throw new InvalidOperationException("Số điện thoại đã tồn tại.");
            }

            // Cập nhật thông tin Khách hàng
            KhachHang.TenKh = KhachHangVM.TenKh;
            KhachHang.Sdt = KhachHangVM.Sdt;
            KhachHang.Gioitinh = KhachHangVM.Gioitinh;
            KhachHang.Diachi = KhachHangVM.Diachi;
            KhachHang.Tendangnhap = KhachHangVM.Tendangnhap;

            // Cập nhật thông tin tài khoản
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(tk => tk.Username == KhachHang.Tendangnhap);
            if (taiKhoan != null && KhachHangVM.TaiKhoan != null)
            {
                taiKhoan.Username = KhachHangVM.Tendangnhap;
                taiKhoan.Role = KhachHangVM.TaiKhoan.Role;
                if (!string.IsNullOrEmpty(KhachHangVM.TaiKhoan.Password))
                {
                    taiKhoan.Password = HashPassword(KhachHangVM.TaiKhoan.Password);
                }
            }

            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var KhachHang = _context.KhachHangs.SingleOrDefault(kh => kh.MaKh == id);
            if (KhachHang == null)
            {
                throw new KeyNotFoundException("Khách hàng không tồn tại.");
            }

            _context.KhachHangs.Remove(KhachHang);

            // Xóa tài khoản của Khách hàng
            var taiKhoan = _context.TaiKhoans.SingleOrDefault(tk => tk.Username == KhachHang.Tendangnhap);
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


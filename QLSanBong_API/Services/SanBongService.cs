using QLSanBong_API.Data;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;

namespace QLSanBong_API.Services
{
    public class SanBongService : ISanBongService
    {
        private readonly QlsanBongContext _context;

        public SanBongService(QlsanBongContext context)
        {
            _context = context;
        }

        public IEnumerable<Models.SanBong> GetAll()
        {
            return _context.SanBongs.Select(sb => new Models.SanBong
            {
                MaSb = sb.MaSb,
                TenSb = sb.TenSb,
                Dientich = sb.Dientich,
                Ghichu = sb.Ghichu,
                Hinhanh = ConvertImageToBase64(sb.Hinhanh), // Chuyển đổi hình ảnh sang Base64
                DiaChi = sb.DiaChi,
            }).ToList();
        }

        // Lấy sân bóng theo ID
        public Models.SanBong GetById(string id)
        {
            var sanBong = _context.SanBongs.SingleOrDefault(sb => sb.MaSb == id);

            if (sanBong == null)
            {
                return null;
            }

            return new Models.SanBong
            {
                MaSb = sanBong.MaSb,
                TenSb = sanBong.TenSb,
                Dientich = sanBong.Dientich,
                Ghichu = sanBong.Ghichu,
                Hinhanh = ConvertImageToBase64(sanBong.Hinhanh), // Chuyển đổi hình ảnh sang Base64
                DiaChi = sanBong.DiaChi,
            };
        }

        public List<DSSanTrong> GetSanTrong(SanDaDatResult result)
        {
            // Gán giá trị mặc định nếu Ngaysudung, Giobatdau, Gioketthuc là null hoặc rỗng
            result.Ngaysudung = string.IsNullOrEmpty(result.Ngaysudung)
                ? DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd")
                : result.Ngaysudung;

            // Kiểm tra xem ngaysudung có phải là định dạng hợp lệ không
            try
            {
                DateOnly.Parse(result.Ngaysudung); // Kiểm tra định dạng ngày
            }
            catch (FormatException)
            {
                throw new ArgumentException("Định dạng ngày không hợp lệ.");
            }

            result.Giobatdau = string.IsNullOrEmpty(result.Giobatdau) ? "06:00:00" : result.Giobatdau;
            result.Gioketthuc = string.IsNullOrEmpty(result.Gioketthuc) ? "22:30:00" : result.Gioketthuc;

            TimeOnly giobatdauTime;
            TimeOnly gioketthucTime;

            try
            {
                giobatdauTime = TimeOnly.Parse(result.Giobatdau); // Chuyển đổi giờ bắt đầu
                gioketthucTime = TimeOnly.Parse(result.Gioketthuc); // Chuyển đổi giờ kết thúc

                // Kiểm tra giờ bắt đầu có nhỏ hơn giờ kết thúc không
                if (giobatdauTime >= gioketthucTime)
                {
                    throw new ArgumentException("Giờ bắt đầu phải nhỏ hơn giờ kết thúc.");
                }
            }
            catch (FormatException)
            {
                throw new ArgumentException("Định dạng giờ không hợp lệ.");
            }

            DateOnly ngaysudungDate = DateOnly.Parse(result.Ngaysudung); // Chuyển ngày thành DateOnly

            // Lấy danh sách tất cả sân bóng có thời gian giá thuê
            var sanallQuery = (
                  from sb in _context.SanBongs
                  from ggt in _context.GiaGioThues
                  where ggt.Giobatdau >= giobatdauTime
                      && ggt.Gioketthuc <= gioketthucTime
                      && (string.IsNullOrEmpty(result.MaSb) || sb.MaSb == result.MaSb)
                select new DSSanTrongVM
                {
                    MaSb = sb.MaSb,
                    Ngaysudung = result.Ngaysudung,
                    Magio = ggt.MaGio
                }
            ).Distinct();

            // Lấy danh sách sân đã đặt từ ChiTietYcds
            var sanDaDatQuery1 = (
                from yc in _context.ChiTietYcds
                join sb in _context.SanBongs on yc.MaSb equals sb.MaSb
                join ggt in _context.GiaGioThues on yc.Magio equals ggt.MaGio
                where yc.Ngaysudung == ngaysudungDate // Chuyển đổi DateOnly thành string
                      && ggt.Giobatdau >= giobatdauTime
                      && ggt.Gioketthuc <= gioketthucTime
                      && yc.TrangThai != "Đã hủy"
                select new DSSanTrongVM
                {
                    MaSb = yc.MaSb,
                    Ngaysudung = result.Ngaysudung,
                    Magio = yc.Magio
                }
            ).Distinct();

            // Lấy danh sách sân đã đặt từ ChiTietYcds
            var sanDaDatQuery2 = (
                from yc in _context.ChiTietYcds
                join sb in _context.SanBongs on yc.MaSb equals sb.MaSb
                join ggt in _context.GiaGioThues on yc.Magio equals ggt.MaGio
                where yc.Ngaysudung == ngaysudungDate // Chuyển đổi DateOnly thành string
                      && ggt.Giobatdau >= giobatdauTime
                      && ggt.Gioketthuc <= gioketthucTime
                      && yc.TrangThai != "Đã xác nhận"
                select new DSSanTrongVM
                {
                    MaSb = yc.MaSb,
                    Ngaysudung = result.Ngaysudung,
                    Magio = yc.Magio
                }
            ).Distinct();


            // Kết hợp các truy vấn đã đặt
            var sanDaDatQuery = sanDaDatQuery1.Union(sanDaDatQuery2).Distinct();

            // Lọc danh sách sân trống
            var sansTrong = sanallQuery.Except(sanDaDatQuery)
                .Select(result => new DSSanTrong
                {
                    MaSb = result.MaSb,
                    Ngaysudung = result.Ngaysudung,
                    Magio = result.Magio,

                    GiaGioThueVM1 = _context.GiaGioThues
                        .Where(g => g.MaGio == result.Magio)
                        .Select(g => new GiaGioThueVM1
                        {
                            Giobatdau = g.Giobatdau.HasValue ? g.Giobatdau.Value.ToString("HH:mm:ss") : null,
                            Gioketthuc = g.Gioketthuc.HasValue ? g.Gioketthuc.Value.ToString("HH:mm:ss") : null,
                            Dongia = g.Dongia
                        }).FirstOrDefault(),
                    SanBongVM1 = _context.SanBongs
                        .Where(sb => sb.MaSb == result.MaSb)
                        .Select(sb => new SanBongVM1
                        {
                            TenSb = sb.TenSb,
                            DiaChi = sb.DiaChi
                        }).FirstOrDefault()
                }).ToList();

            return sansTrong;
        }


        public void Add(SanBongVM sanBongVM, IFormFile imageFile)
        {
            string newMaSb = GenerateNewMaSb();

            string imagePath = null;

            // Kiểm tra xem có hình ảnh mới không
            if (imageFile != null && imageFile.Length > 0)
            {
                // Lưu đường dẫn hình ảnh
                imagePath = GetImagePath(imageFile.FileName);
            }

            var sanBong = new Data.SanBong
            {
                MaSb = newMaSb,
                TenSb = sanBongVM.TenSb,
                Dientich = sanBongVM.Dientich,
                Ghichu = sanBongVM.Ghichu,
                Hinhanh = imagePath, // Lưu đường dẫn hình ảnh
                DiaChi = sanBongVM.DiaChi,
            };

            _context.SanBongs.Add(sanBong);
            _context.SaveChanges();
        }

        public void Update(string id, SanBongVM sanBongVM, IFormFile imageFile)
        {
            var sanBong = _context.SanBongs.SingleOrDefault(sb => sb.MaSb == id);
            if (sanBong == null)
            {
                throw new KeyNotFoundException("Sân bóng không tồn tại.");
            }

            // Cập nhật thông tin sân bóng
            sanBong.TenSb = sanBongVM.TenSb;
            sanBong.Dientich = sanBongVM.Dientich;
            sanBong.Ghichu = sanBongVM.Ghichu;
            sanBong.DiaChi = sanBongVM.DiaChi;

            // Kiểm tra xem có hình ảnh mới không
            if (imageFile != null && imageFile.Length > 0)
            {
                // Nếu có hình ảnh mới, cập nhật đường dẫn hình ảnh
                sanBong.Hinhanh = GetImagePath(imageFile.FileName);
            }

            _context.SaveChanges();
        }


        // Xóa sân bóng
        public void Delete(string id)
        {
            var sanBong = _context.SanBongs.SingleOrDefault(sb => sb.MaSb == id);
            if (sanBong == null)
            {
                throw new KeyNotFoundException("Sân bóng không tồn tại.");
            }

            _context.SanBongs.Remove(sanBong);
            _context.SaveChanges();
        }

        // Phương thức chuyển đổi hình ảnh sang định dạng Base64
        private static string ConvertImageToBase64(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return null;
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));

            if (!File.Exists(fullPath))
            {
                return null;
            }

            byte[] imageBytes = File.ReadAllBytes(fullPath);
            return Convert.ToBase64String(imageBytes);
        }

        private string GenerateNewMaSb()
        {
            string newMaSb = "S001"; // Mã mặc định nếu chưa có sân bóng nào

            if (_context.SanBongs.Any())
            {
                var maxMaSb = _context.SanBongs
                    .Where(m => m.MaSb.StartsWith("S"))
                    .OrderByDescending(m => m.MaSb)
                    .Select(m => m.MaSb)
                    .FirstOrDefault();

                if (maxMaSb != null)
                {
                    int currentNumber = int.Parse(maxMaSb.Substring(1));
                    newMaSb = $"S{(currentNumber + 1).ToString("D3")}";
                }
            }

            return newMaSb;
        }

        private string? GetImagePath(string fileName)
        {
            // Đường dẫn tới thư mục chứa hình ảnh
            string uploadPath = Path.Combine("D:\\Kì 1 năm 5\\CNWeb\\QLSanBong\\QLSanBong_Web\\wwwroot\\images\\sanbongs");

            // Trả về đường dẫn đầy đủ đến tệp hình ảnh
            return Path.Combine(uploadPath, fileName);
        }

    }
}

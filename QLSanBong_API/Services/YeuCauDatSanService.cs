using Microsoft.EntityFrameworkCore;
using QLSanBong_API.Data; // Namespace chứa DbContext và các lớp Data
using QLSanBong_API.Models; // Namespace chứa các lớp Models
using QLSanBong_API.Services.IService;

namespace QLSanBong_API.Services
{
    public class YeuCauDatSanService : IYeuCauDatSanService
    {
        private readonly QlsanBongContext _context;

        public YeuCauDatSanService(QlsanBongContext context)
        {
            _context = context;
        }
        public IEnumerable<Models.YeuCauDatSan> GetAll()
        {
            var ycdsList = _context.YeuCauDatSans
                .Include(ycds => ycds.ChiTietYcds)
                .ToList();

            var ycdsData = ycdsList.Select(ycds =>
            {
                // Retrieve customer data
                var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == ycds.MaKh);
                if (khachHang == null)
                {
                    throw new Exception($"Không tìm thấy khách hàng với MaKH: {ycds.MaKh}");
                }

                return new Models.YeuCauDatSan
                {
                    Stt = ycds.Stt,
                    MaKh = ycds.MaKh,
                    Thoigiandat = ycds.Thoigiandat?.ToString("yyyy-MM-dd HH:mm:ss"),
                    GhiChu = ycds.GhiChu,
                    TongTien = ycds.TongTien,
                    ChiTietYcds = ycds.ChiTietYcds.Select(ct =>
                    {
                        var giaGioThue = _context.GiaGioThues.FirstOrDefault(g => g.MaGio == ct.Magio);
                        if (giaGioThue == null)
                        {
                            throw new Exception($"Không tìm thấy GiaGioThue với MaGio: {ct.Magio}");
                        }

                        return new ChiTietYcds
                        {
                            stt = ct.Stt,
                            MaSb = ct.MaSb,
                            Magio = ct.Magio,
                            Ngaysudung = ct.Ngaysudung.ToString("yyyy-MM-dd"),
                            TrangThai = ct.TrangThai,
                            GhiChu = ct.GhiChu,
                            GiaGioThueVM1 = new GiaGioThueVM1
                            {
                                Giobatdau = giaGioThue.Giobatdau?.ToString("HH:mm:ss"),
                                Gioketthuc = giaGioThue.Gioketthuc?.ToString("HH:mm:ss"),
                                Dongia = giaGioThue.Dongia
                            }
                        };
                    }).ToList(),
                    KhachHangDS = new KhachHangDS
                    {
                        TenKh = khachHang.TenKh,
                        Sdt = khachHang.Sdt,
                        Gioitinh = khachHang.Gioitinh,
                        Diachi = khachHang.Diachi
                    }
                };

            }).ToList();

            return ycdsData;
        }

        public Models.YeuCauDatSan GetByID(int id)
        {
            // Lấy yêu cầu đặt sân kèm theo chi tiết từ cơ sở dữ liệu
            var ycdsList = _context.YeuCauDatSans
                .Include(ycds => ycds.ChiTietYcds)
                .SingleOrDefault(yc => yc.Stt == id);

            if (ycdsList == null)
            {
                return null; // Hoặc throw exception nếu cần
            }

            // Retrieve customer data
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == ycdsList.MaKh);
            if (khachHang == null)
            {
                throw new Exception($"Không tìm thấy khách hàng với MaKH: {ycdsList.MaKh}");
            }

            // Ánh xạ dữ liệu từ ycdsList sang đối tượng Models.YeuCauDatSan
            var ycdsData = new Models.YeuCauDatSan
            {
                MaKh = ycdsList.MaKh,
                Stt = ycdsList.Stt,
                Thoigiandat = ycdsList.Thoigiandat?.ToString("yyyy-MM-dd HH:mm:ss"),
                GhiChu = ycdsList.GhiChu,
                TongTien = ycdsList.TongTien,
                ChiTietYcds = ycdsList.ChiTietYcds.Select(ct =>
                {
                    var giaGioThue = _context.GiaGioThues.FirstOrDefault(g => g.MaGio == ct.Magio);
                    if (giaGioThue == null)
                    {
                        throw new Exception($"Không tìm thấy GiaGioThue với MaGio: {ct.Magio}");
                    }

                    return new ChiTietYcds
                    {
                        stt = ct.Stt,
                        MaSb = ct.MaSb,
                        Magio = ct.Magio,
                        Ngaysudung = ct.Ngaysudung.ToString("yyyy-MM-dd"), // Kiểm tra null
                        TrangThai = ct.TrangThai,
                        GhiChu = ct.GhiChu,
                        GiaGioThueVM1 = new GiaGioThueVM1
                        {
                            Giobatdau = giaGioThue.Giobatdau?.ToString("HH:mm:ss"),
                            Gioketthuc = giaGioThue.Gioketthuc?.ToString("HH:mm:ss"),
                            Dongia = giaGioThue.Dongia
                        }
                    };
                }).ToList(),
                KhachHangDS = new KhachHangDS
                {
                    TenKh = khachHang.TenKh,
                    Sdt = khachHang.Sdt,
                    Gioitinh = khachHang.Gioitinh,
                    Diachi = khachHang.Diachi
                }
            };

            return ycdsData; // Trả về đối tượng YeuCauDatSan
        }
        public IEnumerable<Models.YeuCauDatSan> GetByKH(string makh)
        {
            // Lấy yêu cầu đặt sân kèm theo chi tiết từ cơ sở dữ liệu
            var ycdsList = _context.YeuCauDatSans
                .Include(ycds => ycds.ChiTietYcds)
                .Where(yc => yc.MaKh == makh).ToList();

            if (ycdsList == null)
            {
                return null; // Hoặc throw exception nếu cần
            }

            var ycdsData = ycdsList.Select(ycds =>
            {
                // Retrieve customer data
                var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == ycds.MaKh);
                if (khachHang == null)
                {
                    throw new Exception($"Không tìm thấy khách hàng với MaKH: {ycds.MaKh}");
                }

                return new Models.YeuCauDatSan
                {
                    Stt = ycds.Stt,
                    MaKh = ycds.MaKh,
                    Thoigiandat = ycds.Thoigiandat?.ToString("yyyy-MM-dd HH:mm:ss"),
                    GhiChu = ycds.GhiChu,
                    TongTien = ycds.TongTien,
                    ChiTietYcds = ycds.ChiTietYcds.Select(ct =>
                    {
                        var giaGioThue = _context.GiaGioThues.FirstOrDefault(g => g.MaGio == ct.Magio);
                        if (giaGioThue == null)
                        {
                            throw new Exception($"Không tìm thấy GiaGioThue với MaGio: {ct.Magio}");
                        }

                        return new ChiTietYcds
                        {
                            stt = ct.Stt,
                            MaSb = ct.MaSb,
                            Magio = ct.Magio,
                            Ngaysudung = ct.Ngaysudung.ToString("yyyy-MM-dd"),
                            TrangThai = ct.TrangThai,
                            GhiChu = ct.GhiChu,
                            GiaGioThueVM1 = new GiaGioThueVM1
                            {
                                Giobatdau = giaGioThue.Giobatdau?.ToString("HH:mm:ss"),
                                Gioketthuc = giaGioThue.Gioketthuc?.ToString("HH:mm:ss"),
                                Dongia = giaGioThue.Dongia
                            }
                        };
                    }).ToList(),
                    KhachHangDS = new KhachHangDS
                    {
                        TenKh = khachHang.TenKh,
                        Sdt = khachHang.Sdt,
                        Gioitinh = khachHang.Gioitinh,
                        Diachi = khachHang.Diachi
                    }
                };

            }).ToList();

            return ycdsData; // Trả về đối tượng YeuCauDatSan
        }
        public Models.YeuCauDatSan GetBy(GetYCDSRequest request)
        {
            // Chuyển đổi Ngaysudung thành DateOnly trước khi thực hiện truy vấn
            DateOnly? parsedDate = null;
            if (!string.IsNullOrEmpty(request.Ngaysudung))
            {
                if (!DateOnly.TryParse(request.Ngaysudung, out var tempDate))
                {
                    throw new Exception($"Ngày sử dụng không hợp lệ: {request.Ngaysudung}");
                }
                parsedDate = tempDate;
            }

            // Lấy yêu cầu đặt sân kèm theo chi tiết từ cơ sở dữ liệu
            var ycdsList = _context.YeuCauDatSans
                .Include(ycds => ycds.ChiTietYcds)
                .SingleOrDefault(yc => yc.Stt == request.Id
                                       && yc.ChiTietYcds.Any(ct => ct.MaSb == request.MaSb
                                                                  && ct.Magio == request.Magio
                                                                  && ct.Ngaysudung == parsedDate));

            if (ycdsList == null)
            {
                throw new Exception("Không tìm thấy yêu cầu đặt sân với các chi tiết phù hợp");
            }

            // Retrieve customer data
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == ycdsList.MaKh);
            if (khachHang == null)
            {
                throw new Exception($"Không tìm thấy khách hàng với MaKH: {ycdsList.MaKh}");
            }

            // Ánh xạ dữ liệu từ ycdsList sang đối tượng Models.YeuCauDatSan và chỉ lấy ChiTietYcds phù hợp
            var ycdsData = new Models.YeuCauDatSan
            {
                MaKh = ycdsList.MaKh,
                Stt = ycdsList.Stt,
                Thoigiandat = ycdsList.Thoigiandat?.ToString("yyyy-MM-dd HH:mm:ss"),
                GhiChu = ycdsList.GhiChu,
                TongTien = ycdsList.TongTien,
                ChiTietYcds = ycdsList.ChiTietYcds
                    .Where(ct => ct.MaSb == request.MaSb && ct.Magio == request.Magio && ct.Ngaysudung == parsedDate)
                    .Select(ct =>
                    {
                        var giaGioThue = _context.GiaGioThues.FirstOrDefault(g => g.MaGio == ct.Magio);
                        if (giaGioThue == null)
                        {
                            throw new Exception($"Không tìm thấy GiaGioThue với MaGio: {ct.Magio}");
                        }

                        return new ChiTietYcds
                        {
                            stt = ct.Stt,
                            MaSb = ct.MaSb,
                            Magio = ct.Magio,
                            Ngaysudung = ct.Ngaysudung.ToString("yyyy-MM-dd"),
                            TrangThai = ct.TrangThai,
                            GhiChu = ct.GhiChu,
                            GiaGioThueVM1 = new GiaGioThueVM1
                            {
                                Giobatdau = giaGioThue.Giobatdau?.ToString("HH:mm:ss"),
                                Gioketthuc = giaGioThue.Gioketthuc?.ToString("HH:mm:ss"),
                                Dongia = giaGioThue.Dongia
                            }
                        };
                    }).ToList(),
                KhachHangDS = new KhachHangDS
                {
                    TenKh = khachHang.TenKh,
                    Sdt = khachHang.Sdt,
                    Gioitinh = khachHang.Gioitinh,
                    Diachi = khachHang.Diachi
                }
            };

            return ycdsData; // Trả về đối tượng YeuCauDatSan
        }

        public void Add(YeuCauDatSanAdd yeuCauDatSanAdd)
        {
            // Chuyển đổi thời gian đặt sân
            DateTime thoigiandat = DateTime.Now;

            // Tạo đối tượng YeuCauDatSan và thêm vào DbContext để tự động tạo Stt
            var yeuCauDatSan = new Data.YeuCauDatSan
            {
                MaKh = yeuCauDatSanAdd.MaKh,
                Thoigiandat = thoigiandat,
                GhiChu = yeuCauDatSanAdd.GhiChu,
                TongTien = 0,
                ChiTietYcds = new List<ChiTietYcd>()
            };

            decimal tongTien = 0;

            foreach (var ct in yeuCauDatSanAdd.ChiTietYcdsVM)
            {
                // Tìm GiaGioThue theo mã giờ
                var giaGioThue = _context.GiaGioThues.FirstOrDefault(g => g.MaGio == ct.Magio);

                if (giaGioThue == null)
                {
                    throw new Exception($"Không tìm thấy GiaGioThue với MaGio: {ct.Magio}");
                }

                var giaTien = giaGioThue.Dongia ?? 0;

                tongTien += giaTien;

                var chiTietYcd = new ChiTietYcd
                {
                    MaSb = ct.MaSb,
                    Magio = ct.Magio,
                    Ngaysudung = DateOnly.TryParse(ct.Ngaysudung, out var ngaysudung) ? ngaysudung : default(DateOnly),
                    TrangThai = "Chờ xác nhận",
                    GhiChu = ct.GhiChu,
                    Stt = yeuCauDatSan.Stt // Gán Stt từ YeuCauDatSan ngay khi thêm
                };

                // Thêm ChiTietYcd vào danh sách của YeuCauDatSan
                yeuCauDatSan.ChiTietYcds.Add(chiTietYcd);
            }

            // Cập nhật tổng tiền
            yeuCauDatSan.TongTien = tongTien;

            // Thêm YeuCauDatSan vào DbContext và lưu tất cả thay đổi
            _context.YeuCauDatSans.Add(yeuCauDatSan);
            _context.SaveChanges(); // Chỉ gọi SaveChanges một lần
        }
        public void Update(UpdateYCDSRequest request)
        {
            // Chuyển đổi Ngaysudung thành DateOnly trước khi thực hiện truy vấn
            DateOnly? parsedDate = null;
            if (!string.IsNullOrEmpty(request.Ngaysudung))
            {
                if (!DateOnly.TryParse(request.Ngaysudung, out var tempDate))
                {
                    throw new Exception($"Ngày sử dụng không hợp lệ: {request.Ngaysudung}");
                }
                parsedDate = tempDate;
            }

            // Lấy chi tiết yêu cầu đặt sân từ cơ sở dữ liệu
            var chiTietYcd = _context.ChiTietYcds
                .SingleOrDefault(y => y.Stt == request.Id && y.MaSb == request.MaSb && y.Magio == request.Magio && y.Ngaysudung == parsedDate);

            if (chiTietYcd == null)
            {
                throw new Exception($"Không tìm thấy chi tiết yêu cầu đặt sân phù hợp");
            }

            // Cập nhật trạng thái nếu có trong chi tiết cập nhật
            if (request.trangThai != null)
            {
                chiTietYcd.TrangThai = request.trangThai;
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var yeuCauDatSan = _context.YeuCauDatSans.SingleOrDefault(ycds => ycds.Stt == id);
            var chiTietycds = _context.ChiTietYcds.SingleOrDefault(ycds => ycds.Stt == id);
            if (yeuCauDatSan == null)
            {
                throw new KeyNotFoundException("Yêu cầu đặt sân không tồn tại.");
            }

            _context.ChiTietYcds.Remove(chiTietycds);
            _context.YeuCauDatSans.Remove(yeuCauDatSan);
            _context.SaveChanges();
        }
    }
}

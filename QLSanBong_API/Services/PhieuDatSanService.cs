using Microsoft.EntityFrameworkCore;
using QLSanBong_API.Data; // Namespace chứa DbContext và các lớp Data
using QLSanBong_API.Models; // Namespace chứa các lớp Models
using QLSanBong_API.Services.IService;

namespace QLSanBong_API.Services
{
    public class PhieuDatSanService : IPhieuDatSanService
    {
        private readonly QlsanBongContext _context;

        public PhieuDatSanService(QlsanBongContext context)
        {
            _context = context;
        }

        public IEnumerable<Models.PhieuDatSan> GetAll()
        {
            // Lấy tất cả phiếu đặt sân từ cơ sở dữ liệu
            var pdsList = _context.PhieuDatSans
                .Include(pds => pds.ChiTietPds)
                .ToList();

            var pdsData = pdsList.Select(pds =>
            {
                // Retrieve customer data
                var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == pds.MaKh);
                if (khachHang == null)
                {
                    throw new Exception($"Không tìm thấy khách hàng với MaKH: {pds.MaKh}");
                }

                return new Models.PhieuDatSan
                {
                    MaPds = pds.MaPds,
                    MaKh = pds.MaKh,
                    MaNv = pds.MaNv,
                    Ngaylap = pds.Ngaylap?.ToString("dd/MM/yyyy") ?? string.Empty,
                    GhiChu = pds.GhiChu,
                    TongTien = pds.TongTien.ToString(),
                    Sttds = pds.Sttds,
                    ChiTietPds = pds.ChiTietPds.Select(ct =>
                    {
                        var giaGioThue = _context.GiaGioThues.FirstOrDefault(g => g.MaGio == ct.MaGio);
                        if (giaGioThue == null)
                        {
                            throw new Exception($"Không tìm thấy GiaGioThue với MaGio: {ct.MaGio}");
                        }

                        return new ChiTietPds
                        {
                            MaPds = ct.MaPds,
                            MaSb = ct.MaSb,
                            MaGio = ct.MaGio,
                            Ngaysudung = ct.Ngaysudung.ToString("yyyy-MM-dd"),
                            Ghichu = ct.Ghichu,
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

            return pdsData;
        }

        public IEnumerable<Models.PhieuDatSan> GetByKH(string maKh)
        {
            // Lấy tất cả phiếu đặt sân của khách hàng từ cơ sở dữ liệu theo MaKh
            var pdsList = _context.PhieuDatSans
                .Where(pds => pds.MaKh == maKh)
                .Include(pds => pds.ChiTietPds)
                .ToList();

            var pdsData = pdsList.Select(pds =>
            {
                // Retrieve customer data (No need to check if MaKh exists as we filter by MaKh)
                var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == pds.MaKh);
                if (khachHang == null)
                {
                    throw new Exception($"Không tìm thấy khách hàng với MaKH: {pds.MaKh}");
                }

                return new Models.PhieuDatSan
                {
                    MaPds = pds.MaPds,
                    MaKh = pds.MaKh,
                    MaNv = pds.MaNv,
                    Ngaylap = pds.Ngaylap?.ToString("dd/MM/yyyy") ?? string.Empty,
                    GhiChu = pds.GhiChu,
                    TongTien = pds.TongTien.ToString(),
                    Sttds = pds.Sttds,
                    ChiTietPds = pds.ChiTietPds.Select(ct =>
                    {
                        var giaGioThue = _context.GiaGioThues.FirstOrDefault(g => g.MaGio == ct.MaGio);
                        if (giaGioThue == null)
                        {
                            throw new Exception($"Không tìm thấy GiaGioThue với MaGio: {ct.MaGio}");
                        }

                        return new ChiTietPds
                        {
                            MaPds = ct.MaPds,
                            MaSb = ct.MaSb,
                            MaGio = ct.MaGio,
                            Ngaysudung = ct.Ngaysudung.ToString("yyyy-MM-dd"),
                            Ghichu = ct.Ghichu,
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

            return pdsData;
        }
        public IEnumerable<Models.PhieuDatSan> GetByNV(string maNv)
        {
            var pdsList = _context.PhieuDatSans
                .Where(pds => pds.MaNv == maNv)
                .Include(pds => pds.ChiTietPds)
                .ToList();

            var pdsData = pdsList.Select(pds =>
            {
                var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == pds.MaKh);
                if (khachHang == null)
                {
                    throw new Exception($"Không tìm thấy khách hàng với MaNV: {pds.MaNv}");
                }

                return new Models.PhieuDatSan
                {
                    MaPds = pds.MaPds,
                    MaKh = pds.MaKh,
                    MaNv = pds.MaNv,
                    Ngaylap = pds.Ngaylap?.ToString("dd/MM/yyyy") ?? string.Empty,
                    GhiChu = pds.GhiChu,
                    TongTien = pds.TongTien.ToString(),
                    Sttds = pds.Sttds,
                    ChiTietPds = pds.ChiTietPds.Select(ct =>
                    {
                        var giaGioThue = _context.GiaGioThues.FirstOrDefault(g => g.MaGio == ct.MaGio);
                        if (giaGioThue == null)
                        {
                            throw new Exception($"Không tìm thấy GiaGioThue với MaGio: {ct.MaGio}");
                        }

                        return new ChiTietPds
                        {
                            MaPds = ct.MaPds,
                            MaSb = ct.MaSb,
                            MaGio = ct.MaGio,
                            Ngaysudung = ct.Ngaysudung.ToString("yyyy-MM-dd"),
                            Ghichu = ct.Ghichu,
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

            return pdsData;
        }

        public Models.PhieuDatSan GetById(string id)
        {
            // Lấy phiếu đặt sân theo ID từ cơ sở dữ liệu
            var phieuDatSan = _context.PhieuDatSans
                .Include(pds => pds.ChiTietPds)
                .SingleOrDefault(pds => pds.MaPds == id);

            if (phieuDatSan == null)
            {
                throw new Exception($"Không tìm thấy phiếu đặt sân với MaPds: {id}");
            }

            // Lấy thông tin khách hàng
            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.MaKh == phieuDatSan.MaKh);
            if (khachHang == null)
            {
                throw new Exception($"Không tìm thấy khách hàng với MaKh: {phieuDatSan.MaKh}");
            }

            // Xây dựng đối tượng PhieuDatSan với thông tin chi tiết
            return new Models.PhieuDatSan
            {
                MaPds = phieuDatSan.MaPds,
                MaKh = phieuDatSan.MaKh,
                MaNv = phieuDatSan.MaNv,
                Ngaylap = phieuDatSan.Ngaylap?.ToString("yyyy-MM-dd"),
                GhiChu = phieuDatSan.GhiChu,
                TongTien = phieuDatSan.TongTien.HasValue ? phieuDatSan.TongTien.Value.ToString("N0") : null,
                Sttds = phieuDatSan.Sttds,
                ChiTietPds = phieuDatSan.ChiTietPds.Select(ct =>
                {
                    var giaGioThue = _context.GiaGioThues.FirstOrDefault(g => g.MaGio == ct.MaGio);
                    if (giaGioThue == null)
                    {
                        throw new Exception($"Không tìm thấy GiaGioThue với MaGio: {ct.MaGio}");
                    }

                    return new ChiTietPds
                    {
                        MaPds = ct.MaPds,
                        MaSb = ct.MaSb,
                        MaGio = ct.MaGio,
                        Ngaysudung = ct.Ngaysudung.ToString("yyyy-MM-dd"),
                        Ghichu = ct.Ghichu,
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
        }

        public void Add(PDSAdd pDSAdd)
        {
            // Kiểm tra xem đã tồn tại PhieuDatSan với Sttds đã cho hay chưa
            var phieuDatSan = _context.PhieuDatSans.FirstOrDefault(p => p.Sttds == pDSAdd.Sttds);

            // Nếu chưa tồn tại, tạo một PhieuDatSan mới
            if (phieuDatSan == null)
            {
                phieuDatSan = new Data.PhieuDatSan
                {
                    MaPds = GenerateMaPds(), // Giả sử bạn có một phương thức để tạo mã phiếu đặt sân
                    Ngaylap = DateTime.Parse(pDSAdd.Ngaylap), // Chuyển đổi string thành DateTime
                    TongTien = 0,
                    GhiChu = pDSAdd.GhiChu,
                    Sttds = pDSAdd.Sttds,
                    MaNv = pDSAdd.MaNv,
                    MaKh = pDSAdd.MaKh
                };

                _context.PhieuDatSans.Add(phieuDatSan);
            }

            decimal tongTien = (decimal)phieuDatSan.TongTien;

            foreach (var ct in pDSAdd.ChiTietPdsVM)
            {
                // Tìm GiaGioThue theo mã giờ
                var giaGioThue = _context.GiaGioThues.FirstOrDefault(g => g.MaGio == ct.MaGio);

                if (giaGioThue == null)
                {
                    throw new Exception($"Không tìm thấy GiaGioThue với MaGio: {ct.MaGio}");
                }

                var giaTien = giaGioThue.Dongia ?? 0;
                tongTien += giaTien;

                var chiTietPds = new ChiTietPd
                {
                    MaSb = ct.MaSb,
                    MaGio = ct.MaGio,
                    Ngaysudung = DateOnly.TryParse(ct.Ngaysudung, out var ngaysudung) ? ngaysudung : default(DateOnly),
                    Ghichu = ct.Ghichu
                };

                // Thêm ChiTietPds vào danh sách của PhieuDatSan
                phieuDatSan.ChiTietPds.Add(chiTietPds);
            }

            // Cập nhật tổng tiền
            phieuDatSan.TongTien = tongTien;

            _context.SaveChanges();
        }


        public void Update(string id, PDSAdd pDSAdd)
        {
            var phieuDatSan = _context.PhieuDatSans.SingleOrDefault(pds => pds.MaPds == id);
            if (phieuDatSan == null)
            {
                throw new KeyNotFoundException("Phiếu đặt sân không tồn tại.");
            }

            phieuDatSan.Ngaylap = DateTime.Parse(pDSAdd.Ngaylap);
            phieuDatSan.GhiChu = pDSAdd.GhiChu;
            phieuDatSan.Sttds = pDSAdd.Sttds;
            phieuDatSan.MaNv = pDSAdd.MaNv;
            phieuDatSan.MaKh = pDSAdd.MaKh;

            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var phieuDatSan = _context.PhieuDatSans.SingleOrDefault(pds => pds.MaPds == id);
            if (phieuDatSan == null)
            {
                throw new KeyNotFoundException("Phiếu đặt sân không tồn tại.");
            }

            _context.PhieuDatSans.Remove(phieuDatSan);
            _context.SaveChanges();
        }

        private string GenerateMaPds()
        {
            string newMaPds = "PDS00001"; // Mã mặc định nếu chưa có sân bóng nào

            if (_context.PhieuDatSans.Any())
            {
                var maxMaPds = _context.PhieuDatSans
                    .Where(m => m.MaPds.StartsWith("PDS"))
                    .OrderByDescending(m => m.MaPds)
                    .Select(m => m.MaPds)
                    .FirstOrDefault();

                if (maxMaPds != null)
                {
                    int currentNumber = int.Parse(maxMaPds.Substring(3));
                    newMaPds = $"PDS{(currentNumber + 1).ToString("D5")}";
                }
            }

            return newMaPds;
        }
    }
}

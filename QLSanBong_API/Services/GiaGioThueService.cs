using QLSanBong_API.Data;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;
namespace QLSanBong_API.Services
{

    public class GiaGioThueService : IGiaGioThueService
    {
        private readonly QlsanBongContext _context;

        public GiaGioThueService(QlsanBongContext context)
        {
            _context = context;
        }

        /*
        public static int PAGE_SIZE { get; set; } = 5;
        public List<GiaGioThueVM> GetAll(string search, string? from, string? to, string sortBy, int page = 1)
        {
            var allGiaGioThue = _context.GiaGioThues.AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                allGiaGioThue = allGiaGioThue.Where(g => g.Ghichu.Contains(search));
            }
            if (!string.IsNullOrEmpty(from))
            {
                allGiaGioThue = allGiaGioThue.Where(g => string.Compare(g.Giobatdau.ToString(), from) >= 0);
            }
            if (!string.IsNullOrEmpty(to))
            {
                TimeOnly toTime = TimeOnly.Parse(to); // Chuyển đổi to thành TimeOnly
                allGiaGioThue = allGiaGioThue.Where(g => g.Gioketthuc <= toTime);
            }

            #endregion

            #region Sorting
            // Default sort by start time (Giobatdau)
            allGiaGioThue = allGiaGioThue.OrderBy(g => g.Giobatdau);

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "giobatdau_desc":
                        allGiaGioThue = allGiaGioThue.OrderByDescending(g => g.Giobatdau);
                        break;
                    case "dongia_asc":
                        allGiaGioThue = allGiaGioThue.OrderBy(g => g.Dongia);
                        break;
                    case "dongia_desc":
                        allGiaGioThue = allGiaGioThue.OrderByDescending(g => g.Dongia);
                        break;
                }
            }
            #endregion

            var result = PaginatedList<GiaGioThue>.Create(allGiaGioThue, page, PAGE_SIZE);

            return result.Select(g => new GiaGioThueVM
            {
                MaGio = g.MaGio,
                Giobatdau = g.Giobatdau.ToString(), // Chuyển đổi thành string
                Gioketthuc = g.Gioketthuc.ToString(), // Chuyển đổi thành string
                Dongia = g.Dongia,
                Ghichu = g.Ghichu
            }).ToList();
        }
        */


        public List<GiaGioThueVM> GetAll()
        {
            var giaThueList = _context.GiaGioThues.ToList();
            var giaThueData = giaThueList.Select(ggt => new GiaGioThueVM
            {
                MaGio = ggt.MaGio,
                Giobatdau = ggt.Giobatdau.HasValue ? ggt.Giobatdau.Value.ToString("HH:mm") : null,
                Gioketthuc = ggt.Gioketthuc.HasValue ? ggt.Gioketthuc.Value.ToString("HH:mm") : null,
                Dongia = ggt.Dongia,
            }).ToList();
            return giaThueData;
        }

        public GiaGioThueVM? GetById(string id)
        {
            var giaGioThue = _context.GiaGioThues
                .FirstOrDefault(g => g.MaGio == id);

            if (giaGioThue == null)
            {
                return null;
            }

            return new GiaGioThueVM
            {
                MaGio = giaGioThue.MaGio,
                Giobatdau = giaGioThue.Giobatdau?.ToString(), // Chuyển đổi thành string
                Gioketthuc = giaGioThue.Gioketthuc?.ToString(), // Chuyển đổi thành string
                Dongia = giaGioThue.Dongia
            };
        }


        public void Add(GiaGioThueVM giaGioThueVM)
        {
            var newGiaGioThue = new Data.GiaGioThue
            {
                MaGio = giaGioThueVM.MaGio,
                Giobatdau = TimeOnly.Parse(giaGioThueVM.Giobatdau ?? "00:00"), // Chuyển đổi string thành TimeOnly
                Gioketthuc = TimeOnly.Parse(giaGioThueVM.Gioketthuc ?? "00:00"), // Chuyển đổi string thành TimeOnly
                Dongia = giaGioThueVM.Dongia
            };

            _context.GiaGioThues.Add(newGiaGioThue);
            _context.SaveChanges();
        }


        public void Update(string maGio, GiaGioThueVM1 giaGioThueVM1)
        {
            var existingGiaGioThue = _context.GiaGioThues.SingleOrDefault(g => g.MaGio == maGio);
            if (existingGiaGioThue == null)
            {
                throw new KeyNotFoundException("Giờ thuê không tồn tại.");
            }

            existingGiaGioThue.Giobatdau = TimeOnly.Parse(giaGioThueVM1.Giobatdau ?? "00:00"); // Chuyển đổi string thành TimeOnly
            existingGiaGioThue.Gioketthuc = TimeOnly.Parse(giaGioThueVM1.Gioketthuc ?? "00:00"); // Chuyển đổi string thành TimeOnly
            existingGiaGioThue.Dongia = giaGioThueVM1.Dongia;

            _context.SaveChanges();
        }

        public void Delete(string maGio)
        {
            var giaGioThue = _context.GiaGioThues.SingleOrDefault(g => g.MaGio == maGio);
            if (giaGioThue == null)
            {
                throw new KeyNotFoundException("Giờ thuê không tồn tại.");
            }

            _context.GiaGioThues.Remove(giaGioThue);
            _context.SaveChanges();
        }
    }
}

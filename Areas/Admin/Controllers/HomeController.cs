using Microsoft.AspNetCore.Mvc;
using QLSanBong.Models;
using QLSanBong.Data;
using System.Diagnostics;

namespace QLSanBong.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            if (HttpContext.Session.GetString("user") == null)
            {
                return Redirect("~/Login/Index");
            }
            ViewBag.Configuration = configuration;

            var today = DateTime.Today;
            var firstDayOfThisMonth = new DateTime(today.Year, today.Month, 1);
            ViewBag.thanght = firstDayOfThisMonth.Month;
            var firstDayOfLastMonth = firstDayOfThisMonth.AddMonths(-1);

            //Doanh thu nhá
            decimal tongDoanhThu = db.PhieuDatSans.Sum(hd => hd.TongTien ?? 0);
            var hoadonThisMonth = db.PhieuDatSans
        .Where(hd => hd.Ngaylap >= firstDayOfThisMonth && hd.Ngaylap < firstDayOfThisMonth.AddMonths(1))
        .ToList();
            var hoadonLastMonth = db.PhieuDatSans
                .Where(hd => hd.Ngaylap >= firstDayOfLastMonth && hd.Ngaylap < firstDayOfThisMonth)
                .ToList();
            decimal tongDoanhThuThisMonth = hoadonThisMonth.Sum(hd => hd.TongTien ?? 0);
            decimal tongDoanhThuLastMonth = hoadonLastMonth.Sum(hd => hd.TongTien ?? 0);
            ViewBag.TongdoanhthuThisMonth = tongDoanhThuThisMonth.ToString("N0");
            ViewBag.TongdoanhthuLastMonth = tongDoanhThuLastMonth.ToString("N0");

            decimal phanTramThayDoi = 0;
            if (tongDoanhThuLastMonth != 0)
            {
                phanTramThayDoi = ((tongDoanhThuThisMonth - tongDoanhThuLastMonth) / tongDoanhThuLastMonth) * 100;
            }
            ViewBag.phantram = phanTramThayDoi.ToString("0.##") + "%";
            ViewBag.Tongdoanhthu = tongDoanhThu.ToString("N0");

            //Số hóa đơn nè
            int tongdondat = db.ChiTietPds.Count(hd => hd.MaPds !=null);
            int hdthang = (from cthd in db.ChiTietPds
                           join pds in db.PhieuDatSans
                              on cthd.MaPds equals pds.MaPds
                           where(pds.Ngaylap >= firstDayOfThisMonth && pds.Ngaylap < firstDayOfThisMonth.AddMonths(1))
                           select cthd)
                           .Count(hd=>hd.MaPds!=null);
            ViewBag.Tonghoadon=tongdondat;
            ViewBag.hdthang=hdthang;


            var doanhThuTheoThang = db.PhieuDatSans
            .Where(hd => hd.Ngaylap.HasValue && hd.Ngaylap.Value.Year == DateTime.Now.Year)
            .GroupBy(hd => hd.Ngaylap.Value.Month)
            .Select(group => new DoanhThuThang
            {
                Thang = group.Key,
                DoanhThu = group.Sum(hd => hd.TongTien ?? 0)
            })
            .OrderBy(result => result.Thang)
            .ToList();
            ViewBag.DoanhThuTheoThang = doanhThuTheoThang;

            var luotDatSanList = (from sb in db.SanBongs
                                  join ctpds in db.ChiTietPds on sb.MaSb equals ctpds.MaSb
                                  group ctpds by new { sb.MaSb, sb.TenSb } into groupSb
                                  select new LuotDatSan
                                  {
                                      MaSB = groupSb.Key.MaSb,
                                      TenSB = groupSb.Key.TenSb,
                                      SoLuotDat = groupSb.Count()
                                  }).ToList();
            ViewBag.luotdatsan = luotDatSanList;
            // Kết quả sẽ là danh sách các đối tượng LuotDatSan


            return View();
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Login/Index");
        }
    }
}
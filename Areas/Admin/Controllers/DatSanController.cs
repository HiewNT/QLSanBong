using Microsoft.AspNetCore.Mvc;
using PagedList;
using QLSanBong.Data;
using QLSanBong.Models;
using System.Linq;

namespace QLSanBong.Controllers
{
    [Area("Admin")]
    public class DatSanController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index(string searchname, string searchphone, DateTime? searchdatedat, DateTime? searchdatesd, int pagecho = 1, int pageall = 1, int pagexn = 1, int pageht = 1, int pagehuy = 1, int pageSize = 5)
        {
            string userName = HttpContext.Session.GetString("user");
            var tenNguoiDung = (
            from nv in db.NhanViens
            join tk in db.TaiKhoans on nv.Tendangnhap equals tk.Username
            where tk.Username == userName
            select nv.TenNv
            ).FirstOrDefault();
            ViewBag.ten = tenNguoiDung;

            var queryall = (from yc in db.YeuCauDatSans
                           join ct in db.ChiTietYcds on yc.Stt equals ct.Stt
                           where (string.IsNullOrEmpty(searchname) || yc.Tennguoidat.Contains(searchname))
                              && (string.IsNullOrEmpty(searchphone) || yc.Sdt.Contains(searchphone))
                              && (!searchdatedat.HasValue || yc.Thoigiandat.HasValue && yc.Thoigiandat.Value.Date == searchdatedat.Value.Date)
                              && (!searchdatesd.HasValue || ct.Ngaysudung.HasValue && ct.Ngaysudung.Value.Date == searchdatesd.Value.Date)
                           select new YCDSViewModel
                           {
                               Stt = yc.Stt,
                               Tennguoidat = yc.Tennguoidat,
                               Sdt = yc.Sdt,

                               Thoigiandat = yc.Thoigiandat,
                               Ngaysudung = ct.Ngaysudung,
                               Magio = ct.Magio,
                               Giobatdau = ct.Giobatdau,
                               Gioketthuc = ct.Gioketthuc,
                               MaSb = ct.MaSb,
                               Phuongthuctt = yc.Phuongthuctt,
                               TrangThai = ct.TrangThai
                           }).OrderByDescending(qr=>qr.Thoigiandat);

            var querycho = queryall.Where(x => x.TrangThai == "Đang chờ");
            var queryxn = queryall.Where(x => x.TrangThai == "Đã xác nhận");
            var queryht = queryall.Where(x => x.TrangThai == "Đã hoàn thành");
            var queryhuy = queryall.Where(x => x.TrangThai == "Đã hủy");

            // Thực hiện phân trang
            var totalItemCountall = queryall.Count();
            var totalItemCountcho = querycho.Count();
            var totalItemCountxn = queryxn.Count();
            var totalItemCountht = queryht.Count();
            var totalItemCounthuy = queryhuy.Count();
            var modelall = queryall.ToList();
            var modelcho = querycho.ToList();
            var modelxn = queryxn.ToList();
            var modelht = queryht.ToList();
            var modelhuy = queryhuy.ToList();

            var pagedListall = modelall.ToPagedList(pageall, pageSize);
            var pagedListcho = modelcho.ToPagedList(pagecho, pageSize);
            var pagedListxn = modelxn.ToPagedList(pagexn, pageSize);
            var pagedListht = modelht.ToPagedList(pageht, pageSize);
            var pagedListhuy = modelhuy.ToPagedList(pagehuy, pageSize);
            ViewBag.pagedall = pagedListall;
            ViewBag.pagedcho = pagedListcho;
            ViewBag.pagedxn = pagedListxn;
            ViewBag.pagedht = pagedListht;
            ViewBag.pagedhuy = pagedListhuy;

            // Truyền dữ liệu phân trang, kết quả tìm kiếm và các thông tin tùy chọn vào view
            ViewBag.PageStartItemall = (pageall - 1) * pageSize + 1;
            ViewBag.PageStartItemcho = (pagecho - 1) * pageSize + 1;
            ViewBag.PageStartItemxn = (pagexn - 1) * pageSize + 1;
            ViewBag.PageStartItemht = (pageht - 1) * pageSize + 1;
            ViewBag.PageStartItemhuy = (pagehuy - 1) * pageSize + 1;
            ViewBag.PageEndItemall = Math.Min(pageall * pageSize, totalItemCountall);
            ViewBag.PageEndItemcho = Math.Min(pagecho * pageSize, totalItemCountcho);
            ViewBag.PageEndItemxn = Math.Min(pagexn * pageSize, totalItemCountxn);
            ViewBag.PageEndItemht = Math.Min(pageht * pageSize, totalItemCountht);
            ViewBag.PageEndItemhuy = Math.Min(pagehuy * pageSize, totalItemCounthuy);
            ViewBag.Pageall = pageall;
            ViewBag.Pagecho = pagecho;
            ViewBag.Pagexn = pagexn;
            ViewBag.Pageht = pageht;
            ViewBag.Pagehuy = pagehuy;
            ViewBag.TotalItemCountall = totalItemCountall;
            ViewBag.TotalItemCountcho = totalItemCountcho;
            ViewBag.TotalItemCountxn = totalItemCountxn;
            ViewBag.TotalItemCountht = totalItemCountht;
            ViewBag.TotalItemCounthuy = totalItemCounthuy;

            int currentCount = (pageall - 1) * pageSize + 1;
            ViewBag.CurrentCount = currentCount;

            return View();
        }
    }
}
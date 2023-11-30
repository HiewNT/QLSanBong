using Microsoft.AspNetCore.Mvc;
using PagedList;
using QLSanBong.Data;
using QLSanBong.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QLSanBong.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
       
        [HttpGet]
        public IActionResult Index(string tensb, string loaisan, string dc, int page = 1, int pageSize = 3)
        {
            string userName = HttpContext.Session.GetString("user");
            var tenNguoiDung = (
            from kh in db.KhachHangs
            join tk in db.TaiKhoans on kh.Tendangnhap equals tk.Username
            where tk.Username == userName
            select kh.TenKh
            ).FirstOrDefault();
            ViewBag.ten = tenNguoiDung;
            if (HttpContext.Session.GetString("user") == null)
            {
                return Redirect("~/Login/Index");
            }

            ViewBag.KHDonHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLDH").Select(q => q.MaCn).ToList();
            ViewBag.KHKhachHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLKH").Select(q => q.MaCn).ToList();
            ViewBag.KHSanBongCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLS").Select(q => q.MaCn).ToList();

            var query = (from sb in db.SanBongs
                         where (string.IsNullOrEmpty(tensb) || sb.TenSb.Contains(tensb))
                         && (string.IsNullOrEmpty(dc) || sb.DiaChi.Contains(dc))
                         select sb).ToList();

            // Thực hiện phân trang
            var totalItemCount = query.Count();
            var model = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (model.Count == 0)
            {
                // Nếu không tìm thấy kết quả, điều hướng đến trang thông báo
                return View();
            }

            var pagedList = new StaticPagedList<SanBong>(model, page, pageSize, totalItemCount);

            // Truyền dữ liệu phân trang, kết quả tìm kiếm và các thông tin tùy chọn vào view
            ViewBag.PageStartItem = (page - 1) * pageSize + 1;
            ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
            ViewBag.Page = page;
            ViewBag.TotalItemCount = totalItemCount;

            int currentCount = (page - 1) * pageSize + 1;
            ViewBag.CurrentCount = currentCount;

            return View(pagedList);
            //var totalItemCount = sbQuery.Count();
            //var model = sbQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            //var pagedList = new StaticPagedList<SanBong>(model, page, pageSize, totalItemCount);

            //// Truyền dữ liệu phân trang, kết quả tìm kiếm và các thông tin tùy chọn vào view
            //ViewBag.PageStartItem = (page - 1) * pageSize + 1;
            //ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
            //ViewBag.Page = page;
            //ViewBag.TotalItemCount = totalItemCount;
            //return View(pagedList);
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Login/Index");
        }
        public ActionResult ChiTiet()
        {
            string username = HttpContext.Session.GetString("user");
            var id = (
            from kh in db.KhachHangs
            where kh.Tendangnhap == username
            select kh.MaKh
            ).FirstOrDefault();
            var nv = db.KhachHangs.Find(id);

            ViewBag.KHDonHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLDH").Select(q => q.MaCn).ToList();
            ViewBag.KHKhachHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLKH").Select(q => q.MaCn).ToList();
            ViewBag.KHSanBongCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLS").Select(q => q.MaCn).ToList();

            return View(nv);
        }
        public ActionResult Xoa()
        {
            return View();
        }

        [HttpGet]
        public PartialViewResult AllST(DateTime? dayOrder, TimeSpan timeStart, TimeSpan timeEnd)
        {

            var sanallQuery = (
                 from sb in db.SanBongs
                 from ggt in db.GiaGioThues
                 where ggt.Giobatdau >= timeStart
                       && ggt.Gioketthuc <= timeEnd
                 select new
                 {
                     sb.MaSb,
                     dayOrder,
                     ggt.Magio,
                     ggt.Giobatdau,
                     ggt.Gioketthuc,
                     sb.Trangthai
                 }
             ).Distinct();

            var sanDaDatQuery = (
                from sb in db.SanBongs
                join ctpds in db.ChiTietPds on sb.MaSb equals ctpds.MaSb
                join ggt in db.GiaGioThues on ctpds.Magio equals ggt.Magio
                where ctpds.Ngaysudung == dayOrder &&
                      ggt.Giobatdau >= timeStart &&
                      ggt.Gioketthuc <= timeEnd
                select new
                {
                    sb.MaSb,
                    dayOrder,
                    ctpds.Magio,
                    ggt.Giobatdau,
                    ggt.Gioketthuc,
                    sb.Trangthai
                }
            ).Distinct();

            var sansTrong = sanallQuery.Except(sanDaDatQuery)
                .Select(result => new Santrong
                {
                    MaSb = result.MaSb,
                    Ngaydatsan = dayOrder,
                    Magio = result.Magio,
                    Giobatdau = result.Giobatdau,
                    Gioketthuc = result.Gioketthuc,
                    Trangthai = result.Trangthai
                })
                .ToList();

            return PartialView("_santrong", sansTrong);

        }


        // đưa ra thông tin sân chọn để đặt
        public IActionResult ChonSan(string id)
        {
            var sb = db.SanBongs.Find(id);
            return View(sb);
        }
        // từ thời gian lấy ra danh sách sân chưa ai đặt trên sân đó
        [HttpGet]
        public PartialViewResult AllSTbyID(string id, DateTime? dayOrder, TimeSpan timeStart, TimeSpan timeEnd)
        {

            var sanallQuery = (
                 from sb in db.SanBongs
                 from ggt in db.GiaGioThues
                 where ggt.Giobatdau >= timeStart
                       && ggt.Gioketthuc <= timeEnd
                       && sb.MaSb == id
                 select new
                 {
                     sb.MaSb,
                     dayOrder,
                     ggt.Magio,
                     ggt.Giobatdau,
                     ggt.Gioketthuc,
                     ggt.Dongia,
                     sb.Trangthai
                 }
             ).Distinct();

            var sanDaDatQuery = (
                from sb in db.SanBongs
                join ctpds in db.ChiTietYcds on sb.MaSb equals ctpds.MaSb
                join ggt in db.GiaGioThues on ctpds.Magio equals ggt.Magio
                where ctpds.Ngaysudung == dayOrder &&
                      ggt.Giobatdau >= timeStart &&
                      ggt.Gioketthuc <= timeEnd
                select new
                {
                    sb.MaSb,
                    dayOrder,
                    ctpds.Magio,
                    ggt.Giobatdau,
                    ggt.Gioketthuc,
                    ggt.Dongia,
                    sb.Trangthai
                }
            ).Distinct();

            var sansTrong = sanallQuery.Except(sanDaDatQuery)
                .Select(result => new Santrong2
                {
                    MaSb = result.MaSb,
                    Ngaydatsan = dayOrder,
                    Magio = result.Magio,
                    Giobatdau = result.Giobatdau,
                    Gioketthuc = result.Gioketthuc,
                    Trangthai = result.Trangthai,
                    Giatien = (decimal)result.Dongia,
                })
                .ToList();
            

            return PartialView("_santrongkh", sansTrong);

        }

        public IActionResult Error()
        {
            string userName = HttpContext.Session.GetString("user");
            var tenNguoiDung = (
            from kh in db.KhachHangs
            join tk in db.TaiKhoans on kh.Tendangnhap equals tk.Username
            where tk.Username == userName
            select kh.TenKh
            ).FirstOrDefault();
            ViewBag.ten = tenNguoiDung;

            ViewBag.KHDonHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLDH").Select(q => q.MaCn).ToList();
            ViewBag.KHKhachHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLKH").Select(q => q.MaCn).ToList();
            ViewBag.KHSanBongCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLS").Select(q => q.MaCn).ToList();

            return View();  
        }
    }
}

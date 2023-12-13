using Microsoft.AspNetCore.Mvc;
using PagedList;
using QLSanBong.Data;
using QLSanBong.Models;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QLSanBong.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class SanBongController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index()
        {

            if (HttpContext.Session.GetString("user") == null)
            {
                return Redirect("~/Login/Index");
            }
            string userName = HttpContext.Session.GetString("user");
            var tenNguoiDung = (
            from nv in db.NhanViens
            join tk in db.TaiKhoans on nv.Tendangnhap equals tk.Username
            where tk.Username == userName
            select nv.TenNv
            ).FirstOrDefault();
            ViewBag.ten = tenNguoiDung;


            ViewBag.NVDonHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 2 && q.MaQuyen == "QLDH").Select(q => q.MaCn).ToList();
            ViewBag.NVKhachHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 2 && q.MaQuyen == "QLKH").Select(q => q.MaCn).ToList();
            ViewBag.NVSanBongCn = db.NhomQuyenCns.Where(q => q.MaNhom == 2 && q.MaQuyen == "QLS").Select(q => q.MaCn).ToList();

            return View();
        }
        public IActionResult AllSB(int page = 1, int pageSize = 3)
        {
            var query = db.SanBongs.AsQueryable();
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
            return PartialView("_danhsachsb", pagedList);
        }
        [HttpGet]
        public PartialViewResult AllST(DateTime? dayOrder, TimeSpan timeStart, TimeSpan timeEnd)
        {

            ViewBag.NVDonHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 2 && q.MaQuyen == "QLDH").Select(q => q.MaCn).ToList();
            ViewBag.NVKhachHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 2 && q.MaQuyen == "QLKH").Select(q => q.MaCn).ToList();
            ViewBag.NVSanBongCn = db.NhomQuyenCns.Where(q => q.MaNhom == 2 && q.MaQuyen == "QLS").Select(q => q.MaCn).ToList();

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
                     ggt.Gioketthuc
                 }
             ).Distinct();

            var sanDaDatQuery1 = (
                from yc in db.ChiTietYcds
                where yc.Ngaysudung == dayOrder &&
                      yc.Giobatdau >= timeStart &&
                      yc.Gioketthuc <= timeEnd &&
                      yc.TrangThai != "Đã hủy"
                select new
                {
                    yc.MaSb,
                    dayOrder,
                    yc.Magio,
                    yc.Giobatdau,
                    yc.Gioketthuc
                }
            ).Distinct();

            var sanDaDatQuery2 = (
                from yc in db.ChiTietPds
                where yc.Ngaysudung == dayOrder &&
                      yc.Giobatdau >= timeStart &&
                      yc.Gioketthuc <= timeEnd
                select new
                {
                    yc.MaSb,
                    dayOrder,
                    yc.Magio,
                    yc.Giobatdau,
                    yc.Gioketthuc
                }
            ).Distinct();

            var sanDaDatQuery= sanDaDatQuery1.Union( sanDaDatQuery2 ).Distinct();

            var sansTrong = sanallQuery.Except(sanDaDatQuery)
                .Select(result => new Santrong
                {
                    MaSb = result.MaSb,
                    Ngaydatsan = dayOrder,
                    Magio = result.Magio,
                    Giobatdau = result.Giobatdau,
                    Gioketthuc = result.Gioketthuc
                })
                .ToList();


            return PartialView("_santrong", sansTrong);

        }
        [HttpGet]
        public IActionResult Chitiet( string magio)
        {
            string userName = HttpContext.Session.GetString("user");
            var manv = (
                from nv in db.NhanViens
                where nv.Tendangnhap == userName
                select nv.MaNv
            ).FirstOrDefault();

            var giathue = db.GiaGioThues.FirstOrDefault(gt => gt.Magio == magio);

            if (giathue != null)
            {
                // Trả về thông tin nhân viên, tài khoản và giá thuê
                return new JsonResult(new { code = 200, manv, giathue });
            }
            else
            {
                return Json(new { code = 500, msg = "Không tìm thấy yêu cầu đặt sân" });
            }
        }

        [HttpPost]
        public IActionResult DatSan( string ten, string sdt, string masb, string manv, DateTime ntao, DateTime nsd, TimeSpan giobd, TimeSpan giokt, string magio, Decimal tien, string pttt, string ghichu)
        {
            try
            {
                    // Nếu chưa tồn tại, tạo mới mã
                    var lastpds = db.PhieuDatSans.OrderByDescending(pds => pds.MaPds).FirstOrDefault();
                    string maphieu;
                    if (lastpds != null)
                    {
                        // Tạo mã mới dựa trên mã cuối cùng và tăng lên 1
                        int lastId = int.Parse(lastpds.MaPds.Substring(3)); // Lấy phần số cuối cùng từ mã cũ
                        maphieu = "PDS" + (lastId + 1).ToString("D5");
                    }
                    else
                    {
                        // Nếu không có khách hàng nào, bắt đầu từ 00001
                        maphieu = "PDS00001";
                    }

                    // Tiếp tục với các bước khác chỉ khi cần thêm PhieuDatSan và ChiTietPds mới
                    var pds = new PhieuDatSan();
                    pds.MaPds = maphieu;
                    pds.TenKh = ten;
                    pds.Sdt = sdt;
                    pds.MaNv = manv;
                    pds.Ngaylap = ntao;
                    pds.Phuongthuctt = pttt;
                    pds.GhiChu = ghichu;

                    db.PhieuDatSans.Add(pds);

                    var ctpds = new ChiTietPd();
                    ctpds.MaPds = maphieu;
                    ctpds.MaSb = masb;
                    ctpds.Ngaysudung = nsd;
                    ctpds.Magio = magio;
                    ctpds.Giatien = tien;
                    ctpds.Giobatdau = giobd;
                    ctpds.Gioketthuc = giokt;

                    db.ChiTietPds.Add(ctpds);
                    db.SaveChanges();

                return Json(new { code = 200 });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Thất bại: " + ex.Message });
            }
        }
    }
}

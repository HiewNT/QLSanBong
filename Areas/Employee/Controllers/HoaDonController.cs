using Microsoft.AspNetCore.Mvc;
using PagedList;
using QLSanBong.Data;
using QLSanBong.Models;
using System.Drawing.Printing;

namespace QLSanBong.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class HoaDonController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index(string timname,DateTime? timdatemin,DateTime? timdatemax, int page = 1, int pageSize = 5)
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

            var manv = (
            from nv in db.NhanViens
            join tk in db.TaiKhoans on nv.Tendangnhap equals tk.Username
            where tk.Username == userName
            select nv.MaNv
            ).FirstOrDefault();


            var pds = db.PhieuDatSans.Where(pd=>pd.MaNv==manv && (string.IsNullOrEmpty(timname)||pd.TenKh.Contains(timname))&& (!timdatemin.HasValue || timdatemin.Value.Date<=pd.Ngaylap.Value.Date)&& (!timdatemax.HasValue || timdatemax.Value.Date>=pd.Ngaylap.Value.Date)).ToList();
            var totalItemCount = pds.Count();
            var model = pds.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (model.Count == 0)
            {
                // Nếu không tìm thấy kết quả, điều hướng đến trang thông báo
                return View();
            }

            var pagedList = new StaticPagedList<PhieuDatSan>(model, page, pageSize, totalItemCount);

            // Truyền dữ liệu phân trang, kết quả tìm kiếm và các thông tin tùy chọn vào view
            ViewBag.PageStartItem = (page - 1) * pageSize + 1;
            ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
            ViewBag.Page = page;
            ViewBag.TotalItemCount = totalItemCount;

            int currentCount1 = (page - 1) * pageSize + 1;
            ViewBag.CurrentCount = currentCount1;

            return View(pagedList);
        }
        [HttpGet]
        public PartialViewResult ChiTiet(string soPhieu)
        {
            var ctpds = db.ChiTietPds.Where(n => n.MaPds == soPhieu);
            return PartialView("ctpds", ctpds);
        }
    }
}

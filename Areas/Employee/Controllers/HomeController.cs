using Microsoft.AspNetCore.Mvc;
using QLSanBong.Data;

namespace QLSanBong.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class HomeController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index()
        {
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

            if (HttpContext.Session.GetString("user") == null)
            {
                return Redirect("~/Login/Index");
            }
            return View();
        }
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Login/Index");
        }
        public ActionResult ChiTiet()
        {
            string userName = HttpContext.Session.GetString("user");

            var tenNguoiDung = (
            from nv1 in db.NhanViens
            join tk in db.TaiKhoans on nv1.Tendangnhap equals tk.Username
            where tk.Username == userName
            select nv1.TenNv
            ).FirstOrDefault();
            ViewBag.ten = tenNguoiDung;

            
            ViewBag.NVDonHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 2 && q.MaQuyen == "QLDH").Select(q => q.MaCn).ToList();
            ViewBag.NVKhachHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 2 && q.MaQuyen == "QLKH").Select(q => q.MaCn).ToList();
            ViewBag.NVSanBongCn = db.NhomQuyenCns.Where(q => q.MaNhom == 2 && q.MaQuyen == "QLS").Select(q => q.MaCn).ToList();

            var id = (
            from nv2 in db.NhanViens
            where nv2.Tendangnhap == userName
            select nv2.MaNv
            ).FirstOrDefault();
            var nv = db.NhanViens.Find(id);
            return View(nv);
        }
    }
}

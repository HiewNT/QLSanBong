using Azure;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using QLSanBong.Data;
using QLSanBong.Models;
using System.Drawing.Printing;

namespace QLSanBong.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DatSanController : Controller
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
            from kh in db.KhachHangs
            join tk in db.TaiKhoans on kh.Tendangnhap equals tk.Username
            where tk.Username == userName
            select kh.TenKh
            ).FirstOrDefault();
            ViewBag.ten = tenNguoiDung;


            ViewBag.KHDonHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLDH").Select(q => q.MaCn).ToList();
            ViewBag.KHKhachHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLKH").Select(q => q.MaCn).ToList();
            ViewBag.KHSanBongCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLS").Select(q => q.MaCn).ToList();

            var sdt = (
            from kh in db.KhachHangs
            join tk in db.TaiKhoans on kh.Tendangnhap equals tk.Username
            where tk.Username == userName
            select kh.Sdt
            ).FirstOrDefault();

            ViewBag.KHDonHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLDH").Select(q => q.MaCn).ToList();

            var dsctyc = (from yeuCau in db.YeuCauDatSans
                        join chiTietYcd in db.ChiTietYcds on yeuCau.Stt equals chiTietYcd.Stt
                        where yeuCau.Sdt == sdt
                        select chiTietYcd).ToList();

            return View(dsctyc);
        }

       
        public JsonResult Xoa(int id)
        {
            try { 
                var yc = db.ChiTietYcds.SingleOrDefault(x => x.Stt == id);
                db.ChiTietYcds.Remove(yc);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Hủy yêu cầu thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Hủy yêu cầu thất bại: " + ex.Message });
            }
        }

        public IActionResult HoaDon()
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

            if (HttpContext.Session.GetString("user") == null)
            {
                return Redirect("~/Login/Index");
            }

            var pds = db.PhieuDatSans.Where(p => p.TenKh == tenNguoiDung).ToList(); 
            return View(pds);
        }

        [HttpGet]
        public PartialViewResult ChiTiet(string soPhieu)
        {
            var ctpds = db.ChiTietPds.Where(n => n.MaPds == soPhieu);
            return PartialView("ctpds", ctpds);

            ViewBag.KHDonHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLDH").Select(q => q.MaCn).ToList();
            ViewBag.KHKhachHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLKH").Select(q => q.MaCn).ToList();
            ViewBag.KHSanBongCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLS").Select(q => q.MaCn).ToList();
        }
        
    }
}

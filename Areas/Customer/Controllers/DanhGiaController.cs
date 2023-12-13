using Microsoft.AspNetCore.Mvc;
using QLSanBong.Data;
using QLSanBong.Models;

namespace QLSanBong.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DanhGiaController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DanhGia(string maSb, DateTime ngaySudung, TimeSpan gioBatDau, TimeSpan gioKetThuc, decimal giaTien)
        {
            ViewBag.sb = db.SanBongs.Find(maSb);

            string userName = HttpContext.Session.GetString("user");
            var tenNguoiDung = (
            from kh in db.KhachHangs
            join tk in db.TaiKhoans on kh.Tendangnhap equals tk.Username
            where tk.Username == userName
            select kh.TenKh
            ).FirstOrDefault();
            ViewBag.ten = tenNguoiDung;

            ViewBag.ngay = ngaySudung;
            ViewBag.gioBD = gioBatDau;
            ViewBag.gioKT = gioKetThuc;
            ViewBag.gia = giaTien;

            return View();
        }
        [HttpPost]
        public IActionResult DanhGia(string maSb, string ten, string danhgia)
        {
            var dg = new DanhGia()
            {
                MaSb = maSb,
                TenKh = ten,
                NoiDung = danhgia,
                ThoiGianGanhGia = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"),
            };
            db.DanhGia.Add(dg);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}

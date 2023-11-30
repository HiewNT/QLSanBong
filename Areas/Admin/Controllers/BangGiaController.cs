using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong.Data;
using QLSanBong.Models;

namespace QLSanBong.Controllers
{
    [Area("Admin")]
    public class BangGiaController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AllBG()
        {
            List<GiaGioThue> ggt = db.GiaGioThues.ToList();
            return PartialView("_danhsachbg", ggt);
        }
        [HttpPost]
        public JsonResult ThemMoi(string mag, TimeSpan tstart, TimeSpan tend, decimal price)
        {
            try
            {
                var bg = new GiaGioThue();
                //gan cac thuoc tinh cho object dc tao o tren

                bg.Magio = mag;
                bg.Giobatdau = tstart;
                bg.Gioketthuc = tend;
                bg.Dongia = price;

                db.GiaGioThues.Add(bg);
                db.SaveChanges();
                return Json(new { code = 200, mgs = "Thêm khung giờ mới thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, mgs = "Thêm khung mới thất bại. Lỗi: " + ex.Message });
            }
        }

        public JsonResult Xoa(string id)
        {
            try
            {
                var bg = db.GiaGioThues.SingleOrDefault(x => x.Magio == id);
                db.GiaGioThues.Remove(bg);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Xóa khung giờ thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Xóa khung giờ thất bại: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ChiTiet(string id)
        {
            try
            {
                var bg = db.GiaGioThues.Find(id);
                return new JsonResult(new { code = 200, bg });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy khung giờ thất bại: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Edit(string mag, TimeSpan tstart, TimeSpan tend, decimal price)
        {
            try
            {
                var bg = new GiaGioThue();
                //gan cac thuoc tinh cho object dc tao o tren

                bg.Magio = mag;
                bg.Giobatdau = tstart;
                bg.Gioketthuc = tend;
                bg.Dongia = price;

                db.GiaGioThues.Update(bg);
                db.SaveChanges();
                return Json(new { code = 200, mgs = "Cập nhật khung giờ mới thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, mgs = "Cập nhật khung mới thất bại. Lỗi: " + ex.Message });
            }
        }
    }
}

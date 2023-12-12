using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QLSanBong.Models;
using System.Text.RegularExpressions;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QLSanBong.Data;

namespace QLSanBong.Controllers
{
    [Area("Admin")]
    public class NhanVienController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index()
        {

            if (HttpContext.Session.GetString("user") == null)
            {
                return Redirect("~/Login/Index");
            }
            return View();
        }
        public IActionResult AllDS()
        {
            var dsda = db.NhanViens.Include(nv => nv.TendangnhapNavigation).ToList();

            return PartialView("_danhsach", dsda);
        }
        [HttpPost]
        public JsonResult ThemMoi(string ten, string gt, DateTime ns, string cv, string sdt, string usernv, string passnv)
        {
            try
            {
                var tk = new TaiKhoan();
                
                tk.Username = usernv;
                tk.Password = passnv;
                tk.Quyen = "NHANVIEN";
                db.TaiKhoans.Add(tk);
                db.SaveChanges();

                //gan cac thuoc tinh cho object dc tao o tren
                var lastNhanVien = db.NhanViens.OrderByDescending(nv => nv.MaNv).FirstOrDefault();

                var nv = new NhanVien();
                string newId;
                if (lastNhanVien != null)
                {
                    // Tạo mã mới dựa trên mã cuối cùng và tăng lên 1
                    int lastId = int.Parse(lastNhanVien.MaNv.Substring(2)); // Lấy phần số cuối cùng từ mã cũ
                    newId = "NV" + (lastId + 1).ToString("D3");
                }
                else
                {
                    // Nếu không có khách hàng nào, bắt đầu từ 001
                    newId = "NV001";
                }
                nv.MaNv = newId;

                nv.TenNv = ten;
                nv.Gioitinh = gt;
                nv.Ngaysinh = ns;
                nv.Chucvu = cv;
                nv.Sdt = sdt;
                nv.Tendangnhap = usernv;
                db.NhanViens.Add(nv);
                db.SaveChanges();
                return Json(new { code = 200, mgs = "Thêm thông tin nhân viên mới thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, mgs = "Thêm nhân viên mới thất bại. Lỗi: " + ex.Message });
            }
        }

        public JsonResult Xoa(string id)
        {
            try
            {
                var nv = db.NhanViens.SingleOrDefault(x => x.MaNv == id);
                var tk = db.TaiKhoans.SingleOrDefault(y => y.Username == nv.Tendangnhap);
                db.NhanViens.Remove(nv);
                db.TaiKhoans.Remove(tk);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Xóa nhân viên thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Xóa nhân viên thất bại: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ChiTiet(string id)
        {
            try
            {
                // Tìm nhân viên theo ID
                var nv = db.NhanViens.Find(id);


                if (nv != null)
                {
                    // Trả về thông tin nhân viên và tài khoản
                    return new JsonResult(new { code = 200, nv});
                }
                else
                {
                    return Json(new { code = 404, msg = "Không tìm thấy tài khoản tương ứng với nhân viên" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy thông tin nhân viên thất bại: " + ex.Message });
            }
        }




        [HttpPost]
        public JsonResult Edit(string id, string ten, string gt, DateTime ns, string cv, string sdt, string usernv, string passnv)
        {
            try
            {
                var nv = db.NhanViens.Find(id);
                var tk = db.TaiKhoans.Find(nv.Tendangnhap);
                tk.Username = usernv;
                tk.Password = passnv;
                db.TaiKhoans.Update(tk);

                db.SaveChanges();
                //gan cac thuoc tinh cho object dc tao o tren
                nv.TenNv = ten;
                nv.Gioitinh = gt;
                nv.Ngaysinh = ns;
                nv.Chucvu = cv;
                nv.Sdt = sdt;
                nv.Tendangnhap = usernv;
                db.NhanViens.Update(nv);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Cập nhật thông tin nhân viên thành công" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Cập nhật thông tin nhân viên thất bại. Lỗi: " + ex.Message });
            }
        }

    }
}

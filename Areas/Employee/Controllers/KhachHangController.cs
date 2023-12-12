using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList;
using QLSanBong.Data;
using QLSanBong.Models;

namespace QLSanBong.Areas.Employee.Controllers
{
    [Area("Employee")]

    public class KhachHangController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index(string searchname, string searchphone,string searchdiachi, int page = 1, int pageSize = 3 )
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

            var query = (from kh in db.KhachHangs
                             where (string.IsNullOrEmpty(searchname) || kh.TenKh.Contains(searchname))
                              && (string.IsNullOrEmpty(searchphone) || kh.Sdt.Contains(searchphone))
                              && (string.IsNullOrEmpty(searchdiachi) || kh.Diachi.Contains(searchdiachi))
                             select kh).ToList();

                // Thực hiện phân trang
                var totalItemCount = query.Count();
                var model = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                if (model.Count == 0)
                {
                    // Nếu không tìm thấy kết quả, điều hướng đến trang thông báo
                    return View();
                }

                var pagedList = new StaticPagedList<KhachHang>(model, page, pageSize, totalItemCount);

                // Truyền dữ liệu phân trang, kết quả tìm kiếm và các thông tin tùy chọn vào view
                ViewBag.PageStartItem = (page - 1) * pageSize + 1;
                ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
                ViewBag.Page = page;
                ViewBag.TotalItemCount = totalItemCount;

                int currentCount = (page - 1) * pageSize + 1;
                ViewBag.CurrentCount = currentCount;

                return View(pagedList);
        }

    }
}

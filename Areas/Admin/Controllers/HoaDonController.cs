using Microsoft.AspNetCore.Mvc;
using PagedList;
using QLSanBong.Data;
using QLSanBong.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;

namespace QLSanBong.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HoaDonController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index(string timname, DateTime? timdate, int page = 1, int pageSize = 5)
        {
            var pds = db.PhieuDatSans.Where(pd => (string.IsNullOrEmpty(timname) || pd.TenKh.Contains(timname)) && (!timdate.HasValue || timdate.Value.Date <= pd.Ngaylap.Value.Date)).ToList();
            // Thực hiện phân trang
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

            int currentCount = (page - 1) * pageSize + 1;
            ViewBag.CurrentCount = currentCount;

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

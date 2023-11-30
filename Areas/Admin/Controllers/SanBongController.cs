using Microsoft.AspNetCore.Mvc;
using PagedList;
using QLSanBong.Data;
using QLSanBong.Models;
using System.Linq;

namespace QLSanBong.Controllers
{
    [Area("Admin")]
    public class SanBongController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index()
        {
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

        public ActionResult CapNhat(string id)
        {
            var updateData = db.SanBongs.Find(id);
            return View(updateData);
        }

        [HttpPost]
        public ActionResult CapNhat(SanBong model, IFormFile fileAnh)
        {
            var updateData = db.SanBongs.Find(model.MaSb);
            try
            {
                updateData.TenSb = model.TenSb;
                updateData.Trangthai = model.Trangthai;
                updateData.Dientich = model.Dientich;
                updateData.Ghichu = model.Ghichu;

                if (fileAnh != null && fileAnh.Length > 0)
                {
                    // Đường dẫn lưu trên server
                    var rootFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data", "Images");

                    // Tạo thư mục nếu nó chưa tồn tại
                    Directory.CreateDirectory(rootFolder);

                    // Tạo một tên tệp tin duy nhất để tránh trùng lặp
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileAnh.FileName;

                    // Lưu tệp tin vào thư mục trên server
                    var filePath = Path.Combine(rootFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        fileAnh.CopyTo(stream);
                    }

                    // Cập nhật đường dẫn của ảnh trong model
                    updateData.Hinhanh = Path.Combine("/Data/Images/", uniqueFileName);
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(updateData);
            }
        }


    }
}

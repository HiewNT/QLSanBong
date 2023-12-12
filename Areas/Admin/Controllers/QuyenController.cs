using Microsoft.AspNetCore.Mvc;
using QLSanBong.Data;
using QLSanBong.Models;

namespace QLSanBong.Admin.Controllers
{
    [Area("Admin")]
    public class QuyenController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index()
        {

            if (HttpContext.Session.GetString("user") == null)
            {
                return Redirect("~/Login/Index");
            }
            return View(db.Quyens.OrderBy(n=>n.TenQuyen));
        }

        [HttpPost]
        public IActionResult ThemQuyen()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ThemQuyen(Quyen quyen)
        {
            if (ModelState.IsValid)
            {
                db.Quyens.Add(quyen);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { 
                if(db!=null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

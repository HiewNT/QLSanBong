using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using QLSanBong.Data;
using QLSanBong.Models;
using System;

namespace QLSanBong.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhanQuyenController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index()
        {

            if (HttpContext.Session.GetString("user") == null)
            {
                return Redirect("~/Login/Index");
            }
            return View(db.NhomNguoiDungs.OrderBy(n => n.TenNhom));
        }
        [HttpGet]
        public IActionResult PhanQuyen(int? id)
        {

            NhomNguoiDung nhom = db.NhomNguoiDungs.Find(id);
            if (nhom == null)
            {
                return NotFound();
            }
            // lấy ra ds quyền
            ViewBag.MaQuyen = db.Quyens;
            // lấy ds chức năng
            ViewBag.MaChucNang = db.ChucNangs;
            // lấy ra ds quyền_chức năng của nhóm đó
            ViewBag.NhomNDQuyenCN = db.NhomQuyenCns.Where(n => n.MaNhom == id);
            return View(nhom);
        }
        [HttpPost]
        public IActionResult PhanQuyen1(int? maNhom, IEnumerable<string> maCnValues, IEnumerable<NhomQuyenCn> dsPhanQuyen)
        {
            var dsDaPhanQuyen = db.NhomQuyenCns.Where(n => n.MaNhom == maNhom);

            if (dsDaPhanQuyen.Any())
            {
                db.NhomQuyenCns.RemoveRange(dsDaPhanQuyen);
                db.SaveChanges();
            }

            if (dsPhanQuyen != null && maCnValues != null)
            {
                var dsPhanQuyenList = dsPhanQuyen.ToList();

                while (dsPhanQuyenList.Count < maCnValues.Count())
                {
                    dsPhanQuyenList.Add(new NhomQuyenCn { MaNhom = (int)maNhom });
                }
                var zippedData = maCnValues.Zip(dsPhanQuyenList, (maCnValue, phanQuyen) => new { MaCnValue = maCnValue, PhanQuyen = phanQuyen });

                foreach (var data in zippedData)
                {
                    var parts = data.MaCnValue.Split('-');

                    if (parts.Length == 2)
                    {
                        var maCn = int.Parse(parts[0]);
                        var maQuyen = parts[1];

                        data.PhanQuyen.MaQuyen = maQuyen;
                        data.PhanQuyen.MaCn = maCn;
                        data.PhanQuyen.MaNhom = (int)maNhom;
                        db.NhomQuyenCns.Add(data.PhanQuyen);
                    }
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}

    using Microsoft.AspNetCore.Mvc;
using PagedList;
using QLSanBong.Data;
using QLSanBong.Models;
using System.Drawing.Printing;

namespace QLSanBong.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class YeuCauDatSanController : Controller
    {
        QlsanBongContext db = new QlsanBongContext(); 
        [HttpGet]        
        public IActionResult Index(string searchname, string searchphone, DateTime? searchdatedatmin, DateTime? searchdatedatmax, int page = 1, int pageSize = 5)
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

            var query = (from yc in db.YeuCauDatSans
                        join ct in db.ChiTietYcds on yc.Stt equals ct.Stt
                        where ct.TrangThai == "Đang chờ"
                              && (string.IsNullOrEmpty(searchname) || yc.Tennguoidat.Contains(searchname))
                              && (string.IsNullOrEmpty(searchphone) || yc.Sdt.Contains(searchphone))
                              && (!searchdatedatmin.HasValue || yc.Thoigiandat.HasValue && yc.Thoigiandat.Value.Date >= searchdatedatmin.Value.Date)
                              && (!searchdatedatmax.HasValue || yc.Thoigiandat.HasValue && yc.Thoigiandat.Value.Date <= searchdatedatmax.Value.Date)
                        select new YCDSViewModel
                        {
                            Stt = yc.Stt,
                            Tennguoidat = yc.Tennguoidat,
                            Sdt = yc.Sdt,
                            Thoigiandat = yc.Thoigiandat,
                            Ngaysudung = ct.Ngaysudung,
                            Magio = ct.Magio,
                            Giobatdau = ct.Giobatdau,
                            Gioketthuc = ct.Gioketthuc,
                            MaSb = ct.MaSb,
                            Phuongthuctt = yc.Phuongthuctt,
                            TrangThai = ct.TrangThai
                        }).OrderByDescending(q => q.Thoigiandat).ToList();

            var uniqueQuery = query.GroupBy(x => x.Stt).Select(g => g.First()).OrderByDescending(x=>x.Thoigiandat);

            var totalItemCount = uniqueQuery.Count();
            var model = uniqueQuery.ToList();
            var pagedList = model.ToPagedList(page, pageSize);

            ViewBag.paged = pagedList;
            ViewBag.PageStartItem = (page - 1) * pageSize + 1;
            ViewBag.PageEndItem = Math.Min(page * pageSize, totalItemCount);
            ViewBag.Page = page;
            ViewBag.TotalItemCount = totalItemCount;
            int currentCount = (page - 1) * pageSize + 1;
            ViewBag.CurrentCount = currentCount;

            return View(pagedList);
        }


        [HttpGet]
        public IActionResult GetChiTietYcdByStt(int stt)
        {
            // Lấy thông tin ChiTietYcd dựa trên Stt và trạng thái "Đang chờ"
            var chiTietYcdList = (
                from yc in db.YeuCauDatSans
                join ct in db.ChiTietYcds on yc.Stt equals ct.Stt
                where ct.Stt == stt && ct.TrangThai == "Đang chờ"
                select new YCDSViewModel
                {
                    Stt = yc.Stt,
                    Tennguoidat = yc.Tennguoidat,
                    Sdt = yc.Sdt,
                    Thoigiandat = yc.Thoigiandat,
                    Ngaysudung = ct.Ngaysudung,

                    Magio = ct.Magio,
                    Giobatdau = ct.Giobatdau,
                    Gioketthuc = ct.Gioketthuc,
                    MaSb = ct.MaSb,
                    Phuongthuctt = yc.Phuongthuctt,
                    TrangThai = ct.TrangThai
                })
                .ToList();

            // Trả về dữ liệu dưới dạng JSON hoặc PartialView tùy vào cách bạn muốn xử lý
            return Json(chiTietYcdList);
        }
        [HttpPost]
        public IActionResult RejectRequest(int id, string magio, string masb,string lydo)
        {
            var request = db.ChiTietYcds.FirstOrDefault(yc => yc.Stt == id && yc.Magio == magio && yc.MaSb == masb);
            if (request != null)
            {
                request.GhiChu = lydo;
                request.TrangThai = "Đã hủy";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public IActionResult AcceptRequestYC(int id, string magio, string masb)
        {
            var request = db.ChiTietYcds.FirstOrDefault(yc => yc.Stt == id && yc.Magio == magio && yc.MaSb == masb);

            if (request != null)
            {
                request.TrangThai = "Đã xác nhận";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public IActionResult ChitietPds(int id, string magio, string masb)
        {
            string userName = HttpContext.Session.GetString("user");
            var manv = (
                from nv in db.NhanViens
                where nv.Tendangnhap == userName
                select nv.MaNv
            ).FirstOrDefault();
            
            var ycds = (from yc in db.YeuCauDatSans
                                    join ct in db.ChiTietYcds on yc.Stt equals ct.Stt
                                    where ct.Magio==magio && ct.MaSb==masb && ct.Stt==id
                                    select new YCDSViewModel
                                    {
                                        Stt = yc.Stt,
                                        Tennguoidat = yc.Tennguoidat,
                                        Sdt = yc.Sdt,

                                        Thoigiandat = yc.Thoigiandat,
                                        Ngaysudung = ct.Ngaysudung,
                                        Magio = magio,
                                        Giobatdau = ct.Giobatdau,
                                        Gioketthuc = ct.Gioketthuc,
                                        MaSb = ct.MaSb,
                                        Phuongthuctt = yc.Phuongthuctt,
                                        TrangThai = ct.TrangThai
                                    }).FirstOrDefault();

                var giathue = db.GiaGioThues.FirstOrDefault(gt => gt.Magio == magio);

            if (ycds != null  && giathue != null)
            {
                    // Trả về thông tin nhân viên, tài khoản và giá thuê
                    return new JsonResult(new { code = 200, ycds, manv, giathue });
            }
            else
            {
                return Json(new { code = 500, msg = "Không tìm thấy yêu cầu đặt sân" });
            }
        }
        

        [HttpPost]
        public IActionResult AcceptRequest(int sttds, string tends, string sdtds, string masbds, string manvds, DateTime nsdds, TimeSpan giobd, TimeSpan giokt, string giods, Decimal tiends, string ptttds, string ghichuds)
        {
            try
            {
                // Kiểm tra xem có PhieuDatSan có sttds đã tồn tại hay chưa
                var existingPds = db.PhieuDatSans.FirstOrDefault(p => p.Sttds == sttds);

                string maphieuds;

                if (existingPds != null)
                {
                    // Nếu đã tồn tại, sử dụng mã từ PhieuDatSan đó
                    maphieuds = existingPds.MaPds;

                    // Tiếp tục với các bước khác chỉ khi cần thêm ChiTietPds mới
                    var ctpds = new ChiTietPd();
                    ctpds.MaPds = maphieuds;
                    ctpds.MaSb = masbds;
                    ctpds.Ngaysudung = nsdds;
                    ctpds.Magio = giods;
                    ctpds.Giatien = tiends;
                    ctpds.Giobatdau = giobd;
                    ctpds.Gioketthuc = giokt;

                    db.ChiTietPds.Add(ctpds);
                    db.SaveChanges();
                }
                else
                {
                    // Nếu chưa tồn tại, tạo mới mã
                    var lastpds = db.PhieuDatSans.OrderByDescending(pds => pds.MaPds).FirstOrDefault();

                    if (lastpds != null)
                    {
                        // Tạo mã mới dựa trên mã cuối cùng và tăng lên 1
                        int lastId = int.Parse(lastpds.MaPds.Substring(3)); // Lấy phần số cuối cùng từ mã cũ
                        maphieuds = "PDS" + (lastId + 1).ToString("D5");
                    }
                    else
                    {
                        // Nếu không có khách hàng nào, bắt đầu từ 00001
                        maphieuds = "PDS00001";
                    }
                    DateTime ntao =  DateTime.Now;
                    // Tiếp tục với các bước khác chỉ khi cần thêm PhieuDatSan và ChiTietPds mới
                    var pds = new PhieuDatSan();
                    pds.MaPds = maphieuds;
                    pds.TenKh = tends;
                    pds.Sdt = sdtds;
                    pds.MaNv = manvds;
                    pds.Ngaylap = ntao;
                    pds.Phuongthuctt = ptttds;
                    pds.GhiChu = ghichuds;
                    pds.Sttds = sttds;

                    db.PhieuDatSans.Add(pds);

                    var ctpds = new ChiTietPd();
                    ctpds.MaPds = maphieuds;
                    ctpds.MaSb = masbds;
                    ctpds.Ngaysudung = nsdds;
                    ctpds.Magio = giods;
                    ctpds.Giatien = tiends;
                    ctpds.Giobatdau = giobd;
                    ctpds.Gioketthuc = giokt;

                    db.ChiTietPds.Add(ctpds);
                    db.SaveChanges();
                }

                return Json(new { code = 200 });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Thất bại: " + ex.Message });
            }
        }

    }

}


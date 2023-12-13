using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLSanBong.Data;
using QLSanBong.Models;

namespace QLSanBong.Controllers
{
    public class LoginController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index()
        {
            var sb = db.SanBongs.ToList();

            return View(sb);
        }

        public PartialViewResult AllST(string? idSan, string loai, DateTime? dayOrder, TimeSpan timeStart, TimeSpan timeEnd)
        {
            var sanallQuery = (
                 from sb in db.SanBongs
                 from ggt in db.GiaGioThues
                 where ggt.Giobatdau >= timeStart
                       && ggt.Gioketthuc <= timeEnd
                       && (idSan == null || sb.MaSb == idSan)
                       && (string.IsNullOrEmpty(loai) || sb.LoaiSan == loai)
                 select new
                 {
                     sb.MaSb,
                     dayOrder,
                     ggt.Magio,
                     ggt.Giobatdau,
                     ggt.Gioketthuc,
                     sb.LoaiSan
                 }
             ).Distinct();

            var sanDaDatQuery1 = (
                from yc in db.ChiTietYcds
                from sb in db.SanBongs
                where yc.MaSb == sb.MaSb &&
                      yc.Ngaysudung == dayOrder &&
                      yc.Giobatdau >= timeStart &&
                      yc.Gioketthuc <= timeEnd &&
                      yc.TrangThai != "Đã hủy"
                select new
                {
                    yc.MaSb,
                    dayOrder,
                    yc.Magio,
                    yc.Giobatdau,
                    yc.Gioketthuc,
                    sb.LoaiSan
                }
            ).Distinct();

            var sanDaDatQuery2 = (
                from yc in db.ChiTietPds
                from sb in db.SanBongs
                where yc.MaSb == sb.MaSb &&
                      yc.Ngaysudung == dayOrder &&
                      yc.Giobatdau >= timeStart &&
                      yc.Gioketthuc <= timeEnd
                select new
                {
                    yc.MaSb,
                    dayOrder,
                    yc.Magio,
                    yc.Giobatdau,
                    yc.Gioketthuc,
                    sb.LoaiSan
                }
            ).Distinct();

            var sanDaDatQuery = sanDaDatQuery1.Union(sanDaDatQuery2).Distinct();

            var sansTrong = sanallQuery.Except(sanDaDatQuery)
                .Select(result => new Santrong
                {
                    MaSb = result.MaSb,
                    Ngaydatsan = dayOrder,
                    Magio = result.Magio,
                    Giobatdau = result.Giobatdau,
                    Gioketthuc = result.Gioketthuc,
                    Loai = result.LoaiSan,
                })
                .ToList();


            return PartialView("_santrong", sansTrong);

        }
        public IActionResult BangGia()
        {
            return View();
        }
        public IActionResult Khunggia()
        {
            List<GiaGioThue> khunggia = db.GiaGioThues.ToList();
            return PartialView("_khunggia", khunggia);
        }
        public IActionResult AllSB()
        {
            List<SanBong> dssb = db.SanBongs.ToList();
            return PartialView("_danhsachsb", dssb);
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Login(string userName, string password)
        {
            var user = db.TaiKhoans.SingleOrDefault(m => m.Username == userName);

            if (user != null && user.Password == password)
            {
                HttpContext.Session.SetString("user", userName);

                if (user.Quyen == "ADMIN")

                {
                    return Json(new { success = true, role = "ADMIN" });
                }
                else if (user.Quyen == "KHACHHANG")
                {
                    return Json(new { success = true, role = "KHACHHANG" });
                }
                else if (user.Quyen == "NHANVIEN")
                {
                    return Json(new { success = true, role = "NHANVIEN" });
                }
            }

            // Đăng nhập thất bại, trả về thông báo lỗi
            TempData["error"] = "Sai thông tin đăng nhập";
            return Json(new { success = false, errorMessage = "Sai thông tin đăng nhập" });
        }

        [HttpPost]
        public JsonResult ThemMoi(string name, DateTime day, TimeSpan start, string magio, TimeSpan end, string idstadium, string phoneNumber, string trangthai, string pttt)
        {
            try
            {
                var gia = (from g in db.GiaGioThues where g.Magio == magio select g.Dongia).FirstOrDefault();
                YeuCauDatSan yc = new YeuCauDatSan();
                yc.Tennguoidat = name;
                yc.Sdt = phoneNumber;
                yc.Thoigiandat = DateTime.Now;
                yc.Phuongthuctt = pttt;
                if (pttt == "Chuyển khoản")
                {
                    yc.GhiChu = "Đã thanh toán";
                }
                else
                {
                    yc.GhiChu = "Chưa thanh toán";
                }

                db.YeuCauDatSans.Add(yc);
                db.SaveChanges();
                var ctyc = new ChiTietYcd
                {
                    Stt = yc.Stt,
                    Magio = magio,
                    MaSb = idstadium,
                    Ngaysudung = day,
                    Giobatdau = start,
                    Gioketthuc = end,
                    TrangThai = trangthai,
                    GiaTien = gia
                };
                db.ChiTietYcds.Add(ctyc);
                db.SaveChanges();
                return Json(new { code = 200, mgs = "Yêu cầu đặt sân thành công" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, mgs = "Yêu cầu đặt sân thất bại. Lỗi: " + ex.Message });
            }
        }

        private bool confirm(string message)
        {
            throw new NotImplementedException();
        }

        public IActionResult DangKy(string tenkh, string gtkh, string dckh, string sdtkh, string userkh, string passkh)
        {
            try
            {
                var existingKhachHang = db.KhachHangs.FirstOrDefault(kh => kh.Sdt == sdtkh);
                var tentk = db.KhachHangs.FirstOrDefault(kh => kh.Tendangnhap == userkh);
                if (existingKhachHang != null)
                {
                    // A KhachHang with the same MaKh already exists, return a JSON response with an error message.
                    return Json(new { code = 300, mgs = "Số điện thoại đã được đăng ký!" });
                }
                else if (tentk != null)
                {
                    return Json(new { code = 400, mgs = "Tên đăng nhập đã tồn tại!" });
                }
                else
                {
                    // No duplicate MaKh found, proceed with registration.
                    var tk = new TaiKhoan();
                    tk.Username = userkh;
                    tk.Password = passkh;
                    tk.Quyen = "KHACHHANG";
                    db.TaiKhoans.Add(tk);

                    db.SaveChanges();
                    var kh = new KhachHang();
                    // Assign values to KhachHang properties here

                    kh.MaKh = sdtkh;
                    kh.TenKh = tenkh;
                    kh.Gioitinh = gtkh;
                    kh.Diachi = dckh;
                    kh.Sdt = sdtkh;
                    kh.Tendangnhap = userkh;
                    db.KhachHangs.Add(kh);

                    db.SaveChanges();

                    return Json(new { code = 200, mgs = "Đăng ký khách hàng mới thành công" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, mgs = "Đăng ký khách hàng mới thất bại. Lỗi: " + ex.Message });
            }
        }

        public IActionResult Cart()
        {
            List<Cart> itemCarts = HttpContext.Session.GetString("Cart") != null
        ? JsonConvert.DeserializeObject<List<Cart>>(HttpContext.Session.GetString("Cart"))
        : new List<Cart>();
            ViewBag.ItemCarts = itemCarts;
            return View();
        }

        public PartialViewResult Thongtin()
        {
            return PartialView("_tt");
        }

        public JsonResult Xoa(int id)
        {
            // Lấy danh sách các mục từ session
            List<Cart> cartItems = HttpContext.Session.GetString("Cart") != null
        ? JsonConvert.DeserializeObject<List<Cart>>(HttpContext.Session.GetString("Cart"))
        : new List<Cart>();

            if (cartItems != null)
            {
                var itemToRemove = cartItems.FirstOrDefault(item => item.Id == id);
                if (itemToRemove != null)
                {
                    cartItems.Remove(itemToRemove);

                    // Cập nhật session với danh sách mới
                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartItems));
                }
            }
            return Json(new { code = 200, msg = "Hủy yêu cầu thành công" });
        }
        [HttpPost]
        public JsonResult AddItem(string MaSb, string Magio, DateTime NgayDat)
        {
            var sl = 0;
            if (HttpContext.Session.GetString("Cart") == null)
            {
                // Tạo một danh sách mới của List<Cart> và lưu trữ nó trong phiên
                var newCart = new List<Cart>();
                var serializedCart = JsonConvert.SerializeObject(newCart);
                HttpContext.Session.SetString("Cart", serializedCart);
            }

            // Lấy dữ liệu phiên "Cart" và giải mã nó thành List<Cart>
            var cartData = HttpContext.Session.GetString("Cart");
            var itemCarts = JsonConvert.DeserializeObject<List<Cart>>(cartData);

            // Kiểm tra sân, giờ, ngày đã tồn tại trong giỏ hàng chưa
            var existingCartItem = itemCarts.FirstOrDefault(c => c.MaSb == MaSb && c.Magio == Magio && c.NgayDat == NgayDat);

            if (existingCartItem != null)
            {
                // Nếu mục đã tồn tại 
                return Json(new { success = false, message = "Lịch đặt sân này đã có trong danh sách chờ." });
            }
            else
            {
                sl = itemCarts.Count() + 1;
                var start = (from s in db.GiaGioThues where s.Magio == Magio select s.Giobatdau).FirstOrDefault();
                var end = (from e in db.GiaGioThues where e.Magio == Magio select e.Gioketthuc).FirstOrDefault();
                var gia = (from g in db.GiaGioThues where g.Magio == Magio select g.Dongia).FirstOrDefault();
                // Nếu mục chưa tồn tại, thêm một mục mới
                var newCartItem = new Cart
                {
                    Id = sl,
                    MaSb = MaSb,
                    Magio = Magio,
                    NgayDat = NgayDat,
                    Giobatdau = start,
                    Gioketthuc = end,
                    Giatien = gia,
                };

                itemCarts.Add(newCartItem);
            }

            // Sau khi thay đổi giỏ hàng, lưu giỏ hàng mới vào phiên
            var updatedCartData = JsonConvert.SerializeObject(itemCarts);
            HttpContext.Session.SetString("Cart", updatedCartData);

            // Trả về một JSON để xác nhận hoặc hiển thị thông báo
            return Json(new { success = true, message = "Lịch đặt sân đã được thêm vào danh sách chờ." });
        }

        [HttpPost]
        public IActionResult AddOrder(string name1, string phoneNumber1, string pttt1, decimal totalPrice1)
        {
            List<Cart> listCarts = HttpContext.Session.GetString("Cart") != null
        ? JsonConvert.DeserializeObject<List<Cart>>(HttpContext.Session.GetString("Cart"))
        : new List<Cart>();

            YeuCauDatSan yc = new YeuCauDatSan();
            yc.Tennguoidat = name1;
            yc.Sdt = phoneNumber1;
            yc.Thoigiandat = DateTime.Now;
            yc.Phuongthuctt = pttt1;
            yc.TongTien = totalPrice1;
            if (pttt1 == "Chuyển khoản")
            {
                yc.GhiChu = "Đã thanh toán";
            }
            else
            {
                yc.GhiChu = "Chưa thanh toán";
            }

            db.YeuCauDatSans.Add(yc);
            db.SaveChanges();
            foreach (Cart item in listCarts)
            {

                ChiTietYcd ctyc = new ChiTietYcd();
                ctyc.Stt = yc.Stt;
                ctyc.Ngaysudung = item.NgayDat;
                ctyc.Giobatdau = item.Giobatdau;
                ctyc.Gioketthuc = item.Gioketthuc;
                ctyc.MaSb = item.MaSb;
                ctyc.TrangThai = "Đang chờ";
                ctyc.GiaTien = item.Giatien;
                ctyc.Magio = item.Magio;
                db.ChiTietYcds.Add(ctyc);
            }
            db.SaveChanges();
            listCarts.Clear();
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(listCarts));
            return RedirectToAction("Cart");
        }
        [HttpGet]
        public JsonResult GetItemCount()
        {
            List<Cart> itemCarts = HttpContext.Session.GetString("Cart") != null
                ? JsonConvert.DeserializeObject<List<Cart>>(HttpContext.Session.GetString("Cart"))
                : new List<Cart>();

            int itemCount = itemCarts.Count();

            return Json(new { success = true, itemCount });
        }
    }
}

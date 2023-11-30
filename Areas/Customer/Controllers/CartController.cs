using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLSanBong.Data;
using QLSanBong.Models;

namespace QLSanBong.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        QlsanBongContext db = new QlsanBongContext();
        public IActionResult Index()
        {
            string userName = HttpContext.Session.GetString("user");
            var tenNguoiDung = (
            from kh in db.KhachHangs
            join tk in db.TaiKhoans on kh.Tendangnhap equals tk.Username
            where tk.Username == userName
            select kh.TenKh
            ).FirstOrDefault();
            ViewBag.ten = tenNguoiDung;

            List<Cart> itemCarts = HttpContext.Session.GetString("Cart") != null
        ? JsonConvert.DeserializeObject<List<Cart>>(HttpContext.Session.GetString("Cart"))
        : new List<Cart>();
            ViewBag.KHDonHangCn = db.NhomQuyenCns.Where(q => q.MaNhom == 3 && q.MaQuyen == "QLDH").Select(q => q.MaCn).ToList();

            ViewBag.ItemCarts = itemCarts;
            return View();
        }
        // thêm sân vào giỏ hàng
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

        [HttpGet]
        public JsonResult GetItemCount()
        {
            List<Cart> itemCarts = HttpContext.Session.GetString("Cart") != null
                ? JsonConvert.DeserializeObject<List<Cart>>(HttpContext.Session.GetString("Cart"))
                : new List<Cart>();

            int itemCount = itemCarts.Count;

            return Json(new { success = true, itemCount = itemCount });
        }

        [HttpGet]
        public IActionResult GetInfo()
        {
            string userName = HttpContext.Session.GetString("user");
            var tenNguoiDung = (
                from kh in db.KhachHangs
                join tk in db.TaiKhoans on kh.Tendangnhap equals tk.Username
                where tk.Username == userName
                select kh.TenKh
            ).FirstOrDefault();

            var sdtNguoiDung = (
                from kh in db.KhachHangs
                join tk in db.TaiKhoans on kh.Tendangnhap equals tk.Username
                where tk.Username == userName
                select kh.Sdt
            ).FirstOrDefault();

            return Json(new { tenNguoiDung, sdtNguoiDung });
        }


        [HttpPost]
        public IActionResult AddOrder(string pttt, decimal totalPrice1)
        {
            List<Cart> listCarts = HttpContext.Session.GetString("Cart") != null
        ? JsonConvert.DeserializeObject<List<Cart>>(HttpContext.Session.GetString("Cart"))
        : new List<Cart>();

            string userName = HttpContext.Session.GetString("user");
            var tenNguoiDung = (
            from kh in db.KhachHangs
            join tk in db.TaiKhoans on kh.Tendangnhap equals tk.Username
            where tk.Username == userName
            select kh.TenKh
            ).FirstOrDefault();
            var sdtNguoiDung = (
            from kh in db.KhachHangs
            join tk in db.TaiKhoans on kh.Tendangnhap equals tk.Username
            where tk.Username == userName
            select kh.Sdt
            ).FirstOrDefault();

            YeuCauDatSan yc = new YeuCauDatSan();
            yc.Tennguoidat = tenNguoiDung;
            yc.Sdt = sdtNguoiDung;
            yc.Thoigiandat = DateTime.Now;
            yc.Phuongthuctt = pttt;
            yc.TongTien = totalPrice1;
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
            return Redirect("~/Customer/Cart/Index");
        }

    }
}

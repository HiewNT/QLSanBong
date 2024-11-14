using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using QLSanBong_API.Data;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace QLSanBong_API.Services
{
    public class GioHangService : IGioHangService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly QlsanBongContext _context;
        private const string CartSessionKey = "GioHangSession";

        public GioHangService(QlsanBongContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Lấy danh sách chờ từ Session
        private List<GioHang> GetSessionCart()
        {
            var cartData = _httpContextAccessor.HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartData))
            {
                return new List<GioHang>(); // Trả về danh sách rỗng nếu danh sách chờ không có
            }

            try
            {
                return JsonConvert.DeserializeObject<List<GioHang>>(cartData);
            }
            catch (JsonException)
            {
                return new List<GioHang>(); // Trả về danh sách chờ rỗng nếu lỗi xảy ra trong quá trình deserialize
            }
        }


        // Lưu danh sách chờ vào Session
        private void SaveSessionCart(List<GioHang> cartItems)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString(CartSessionKey, JsonConvert.SerializeObject(cartItems)); // Sử dụng CartSessionKey
        }

        public ActionResult<string> AddToCart(GioHang gioHang)
        {
            var cartItems = GetSessionCart();

            if (cartItems == null)
            {
                cartItems = new List<GioHang>();
            }

            // Kiểm tra nếu Giờ thuê đã có trong danh sách chờ
            var existingItem = cartItems.FirstOrDefault(item =>
                item.MaSb == gioHang.MaSb &&
                item.Magio == gioHang.Magio &&
                item.Ngaysudung == gioHang.Ngaysudung);

            if (existingItem == null)
            {
                cartItems.Add(new GioHang
                {
                    MaSb = gioHang.MaSb,
                    Ngaysudung = gioHang.Ngaysudung,
                    Magio = gioHang.Magio,
                    Giobatdau = gioHang.Giobatdau.ToString(),
                    Gioketthuc = gioHang.Gioketthuc.ToString(),
                    Dongia = gioHang.Dongia,
                    TenSb = gioHang.TenSb,
                    DiaChi = gioHang.DiaChi,
                });

                SaveSessionCart(cartItems);

                // Log nội dung danh sách chờ để kiểm tra
                Console.WriteLine($"danh sách chờ đã được lưu: {JsonConvert.SerializeObject(cartItems)}");

                return new OkObjectResult("Giờ thuê đã được thêm vào danh sách chờ.");
            }
            else
            {
                return new ConflictObjectResult("Giờ thuê đã có trong danh sách chờ.");
            }
        }

        // Xóa tất cả Giờ thuê trong danh sách chờ
        public string DeleteAll()
        {
            // Lấy danh sách chờ và số lượng Giờ thuê
            var (cartItems, itemCount) = GetCartAndCount();

            // Kiểm tra nếu danh sách chờ trống
            if (itemCount == 0)
            {
                return "danh sách chờ hiện tại đang trống.";
            }

            // Xóa danh sách chờ
            cartItems.Clear();
            SaveSessionCart(cartItems); // Lưu lại danh sách chờ trống

            return "danh sách chờ đã được xóa thành công.";
        }

        // Xóa một Giờ thuê khỏi danh sách chờ
        public string DeleteCart(GioHang gioHang)
        {
            var cartItems = GetSessionCart();

            var itemToRemove = cartItems.FirstOrDefault(item => item.MaSb == gioHang.MaSb &&
                                                                item.Magio == gioHang.Magio &&
                                                                item.Ngaysudung == gioHang.Ngaysudung);

            if (itemToRemove == null)
            {
                return "Giờ thuê không tồn tại trong danh sách chờ.";  // Trả về thông báo nếu không tìm thấy Giờ thuê trong giỏ
            }

            cartItems.Remove(itemToRemove);
            SaveSessionCart(cartItems);

            return "Giờ thuê đã được xóa khỏi danh sách chờ.";
        }

        // Lấy danh sách chờ và số lượng Giờ thuê trong danh sách chờ
        public (List<GioHang> cartItems, int itemCount) GetCartAndCount()
        {
            var cartItems = GetSessionCart(); // Lấy danh sách chờ từ session

            // Đảm bảo danh sách chờ không null
            cartItems = cartItems ?? new List<GioHang>();

            // Trả về danh sách chờ và số lượng phần tử
            int itemCount = cartItems.Count;
            return (cartItems, itemCount);
        }
    }
}

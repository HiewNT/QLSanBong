using QLSanBong_API.Models;
using System.Text.Json;

namespace QLSanBong_API.Helpers
{
    public static class SessionExtensions
    {
        private const string CartSessionKey = "CartSession";

        // Phương thức lưu danh sách GioHang vào Session
        public static void SetCart(this ISession session, List<GioHang> cart)
        {
            session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        // Phương thức lấy danh sách GioHang từ Session
        public static List<GioHang> GetCart(this ISession session)
        {
            var data = session.GetString(CartSessionKey);
            return data == null ? new List<GioHang>() : JsonSerializer.Deserialize<List<GioHang>>(data);
        }
    }

}

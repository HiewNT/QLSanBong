using Microsoft.AspNetCore.Mvc;
using QLSanBong_API.Models;
using System.Collections.Generic;

namespace QLSanBong_API.Services.IService
{
    public interface IGioHangService
    {
        // Thêm sản phẩm vào giỏ hàng
        ActionResult<string> AddToCart(GioHang gioHang);

        // Xóa toàn bộ giỏ hàng
        string DeleteAll();

        // Xóa sản phẩm khỏi giỏ hàng
        string DeleteCart(GioHang gioHang);

        // Lấy giỏ hàng hiện tại
        (List<GioHang> cartItems, int itemCount) GetCartAndCount();
    }
}

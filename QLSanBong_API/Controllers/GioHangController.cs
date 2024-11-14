using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;
using System.Collections.Generic;

namespace QLSanBong_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangService _gioHangService;

        public GioHangController(IGioHangService gioHangService)
        {
            _gioHangService = gioHangService;
        }
        // Thêm sản phẩm vào giỏ hàng
        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] GioHang gioHang)
        {
            var message = _gioHangService.AddToCart(gioHang);
            return Ok(new { result = true, message="Thêm giờ thuê vào danh sách chờ thành công!", data = gioHang });
        }

        // Lấy giỏ hàng và số lượng sản phẩm trong giỏ hàng
        [HttpGet("cart")]
        public ActionResult GetCartAndCount()
        {
            var (cartItems, itemCount) = _gioHangService.GetCartAndCount();
            string message = itemCount == 0 ? "Giỏ hàng trống." : "Số lượng sản phẩm trong giỏ hàng.";

            // Trả về giỏ hàng và số lượng
            return Ok(new { cartItems, itemCount, message });
        }

        // Xóa toàn bộ giỏ hàng
        [HttpDelete("deleteall")]
        public ActionResult DeleteAll()
        {
            // Lấy giỏ hàng và số lượng sản phẩm trong giỏ hàng
            var (_, itemCount) = _gioHangService.GetCartAndCount();

            // Kiểm tra nếu giỏ hàng đang trống
            if (itemCount == 0)
            {
                return Ok(new { message = "Giỏ hàng hiện tại đang trống." });
            }

            // Nếu có sản phẩm, xóa giỏ hàng
            HttpContext.Session.Remove("gioHang");
            return Ok(new { message = "Giỏ hàng đã được xóa thành công." });
        }

        [HttpPost("delete")]
        public IActionResult DeleteCart([FromBody] GioHang gioHang)
        {
            var message = _gioHangService.DeleteCart(gioHang);

            // Kiểm tra nếu không tìm thấy sản phẩm trong giỏ
            if (string.IsNullOrEmpty(message))
            {
                return NotFound(new { message = "Sản phẩm không tồn tại trong giỏ hàng." });
            }

            return Ok(new { data = gioHang, message });
        }

    }

}

using Microsoft.AspNetCore.Mvc;
using QLSanBong_API.Helpers;
using QLSanBong_API.Models;

namespace QLSanBong_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KhachHangController : ControllerBase
    {
        private readonly IKhachHangService _khachHangService;

        public KhachHangController(IKhachHangService khachHangService)
        {
            _khachHangService = khachHangService;
        }

        // GET: api/khachhang
        [HttpGet("getall")]
        //public ActionResult<List<KhachHang>> GetAll([FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        //{
        //    var khachHangList = _khachHangService.GetAll(search, sortBy, page, pageSize);
        //    return Ok(khachHangList);
        //}

        public ActionResult<IEnumerable<KhachHang>> GetAll()
        {
            // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
            if (!PermissionHelper.HasPermission(User, "khachhang_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập chức năng này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }

            var khachHangList = _khachHangService.GetAll(); // Gọi phương thức GetAll không tham số
            return Ok(khachHangList);
        }
        // GET: api/khachhang/{id}
        [HttpGet("getbyid")]
        public ActionResult<KhachHang> GetById([FromQuery] string id)
        {
            // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
            if (!PermissionHelper.HasPermission(User, "khachhang_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập chức năng này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }

            var khachHang = _khachHangService.GetById(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return Ok(khachHang);
        }
        // POST: api/khachhang
        [HttpPost("add")]
        public ActionResult<KhachHang> Add(KhachHangVM khachHangVM)
        {
            try
            {
                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User, "khachhang_add"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }

                _khachHangService.Add(khachHangVM);
                return CreatedAtAction(nameof(GetById), new { id = khachHangVM.UserID }, khachHangVM);
            }
            catch (InvalidOperationException ex)
            {
                // Trả về mã trạng thái 400 (Bad Request) và thông báo lỗi
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ khác
                return StatusCode(500, "Đã xảy ra lỗi: " + ex.Message);
            }
        }


        // PUT: api/khachhang/{id}
        [HttpPut("update")]
        public IActionResult Update([FromQuery] string id, KhachHangVM khachHangVM)
        {
            try
            {
                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User, "khachhang_edit"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }

                _khachHangService.Update(id, khachHangVM);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                // Trả về mã trạng thái 400 (Bad Request) và thông báo lỗi
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ khác
                return StatusCode(500, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        // DELETE: api/khachhang/{id}
        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] string id)
        {
            try
            {
                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User, "khachhangdelete"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }

                _khachHangService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}

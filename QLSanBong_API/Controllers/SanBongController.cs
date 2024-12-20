﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong_API.Helpers;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;

namespace QLSanBong_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanBongController : ControllerBase
    {
        private readonly ISanBongService _sanBongService;

        public SanBongController(ISanBongService sanBongService)
        {
            _sanBongService = sanBongService;
        }

        // Lấy tất cả sân bóng
        [HttpGet("usergetall")]
        public IActionResult UserGetAll()
        {
            var sanBongs = _sanBongService.UserGetAll();
            return Ok(sanBongs);
        }
        // Lấy tất cả sân bóng
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            // Lấy role từ header
            var role = Request.Headers["Role"].ToString();

            // Kiểm tra quyền "sanbong_read" và role trước khi thực hiện logic
            if (!PermissionHelper.HasPermission(User, role, "sanbong_read"))
            {
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền xem danh sách sân bóng."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }

            var sanBongs = _sanBongService.GetAll();
            return Ok(sanBongs);
        }


        // Lấy sân bóng theo ID
        [HttpGet("getbyid")]
        public IActionResult GetById([FromQuery] string id)
        {
            var sanBong = _sanBongService.GetById(id);
            if (sanBong == null)
            {
                return NotFound();
            }
            return Ok(sanBong);
        }
        [HttpPost("santrong")]
        public IActionResult GetSanTrong([FromBody] SanDaDatResult result)
        {
            // Kiểm tra xem dữ liệu đầu vào có hợp lệ không
            if (result == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                // Gọi dịch vụ để lấy danh sách sân trống
                var sansTrong = _sanBongService.GetSanTrong(result);

                // Kiểm tra xem có sân trống không
                if (sansTrong == null || sansTrong.Count == 0)
                {
                    return NotFound("Không có sân trống trong khoảng thời gian đã chọn.");
                }

                // Trả về danh sách sân trống
                return Ok(sansTrong);
            }
            catch (ArgumentException argEx)
            {
                // Xử lý lỗi ArgumentException, có thể liên quan đến việc truyền tham số không hợp lệ
                return BadRequest("Lỗi tham số: " + argEx.Message);
            }
            catch (FormatException formatEx)
            {
                // Xử lý lỗi khi có lỗi định dạng (FormatException)
                return BadRequest("Lỗi định dạng: " + formatEx.Message);
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi chung và ghi log nếu cần
                // Bạn có thể ghi log vào file hoặc hệ thống log như Serilog, NLog, v.v.
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình xử lý yêu cầu.");
            }
        }


        // Thêm sân bóng
        [HttpPost("add")]
        public IActionResult Add([FromForm] SanBongVM sanBongVM, IFormFile imageFile)
        {
            // Lấy role từ header
            var role = Request.Headers["Role"].ToString();

            // Kiểm tra quyền "sanbong_read" và role trước khi thực hiện logic
            if (!PermissionHelper.HasPermission(User, role, "sanbong_add"))
            {
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập quyền này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _sanBongService.Add(sanBongVM, imageFile);
            return CreatedAtAction(nameof(GetById), new { id = sanBongVM.TenSb }, sanBongVM);
        }

        // Cập nhật sân bóng
        [HttpPut("update")]
        public IActionResult Update([FromQuery] string id, [FromForm] SanBongVM sanBongVM, IFormFile? imageFile) // Đảm bảo `imageFile` có thể là null
        {
            // Lấy role từ header
            var role = Request.Headers["Role"].ToString();

            // Kiểm tra quyền "sanbong_read" và role trước khi thực hiện logic
            if (!PermissionHelper.HasPermission(User, role, "sanbong_edit"))
            {
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập quyền này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                _sanBongService.Update(id, sanBongVM, imageFile);
                return Ok(new { message = "Cập nhật sân bóng thành công" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Sân bóng không tồn tại" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Cập nhật thất bại", error = ex.Message });
            }
        }


        // Xóa sân bóng
        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] string id)
        {
            try
            {
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();

                // Kiểm tra quyền "sanbong_read" và role trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User, role, "sanbong_delete"))
                {
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập quyền này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                _sanBongService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}

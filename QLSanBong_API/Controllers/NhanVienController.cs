using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong_API.Models;

namespace QLSanBong_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NhanVienController : ControllerBase
    {
        private readonly INhanVienService _nhanVienService;

        public NhanVienController(INhanVienService nhanVienService)
        {
            _nhanVienService = nhanVienService;
        }

        // GET: api/nhanvien
        [HttpGet("getall")]
        [Authorize(Roles = "Admin,NhanVien")]
        public ActionResult<IEnumerable<NhanVien>> GetAll()
        {
            var nhanVienList = _nhanVienService.GetAll(); // Gọi phương thức GetAll không tham số
            return Ok(nhanVienList);
        }
        // GET: api/nhanvien/{id}
        [HttpGet("getbyid")]
        [Authorize(Roles = "Admin,NhanVien")]
        public ActionResult<NhanVien> GetById([FromQuery] string id)
        {
            var nhanVien = _nhanVienService.GetById(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return Ok(nhanVien);
        }

        // POST: api/nhanvien
        [HttpPost("add")]
        [Authorize(Policy = "RequireAdminRole")] // Chỉ cho phép admin thực hiện
        public ActionResult<NhanVien> Add(NhanVienVM nhanVienVM)
        {
            try
            {
                _nhanVienService.Add(nhanVienVM);
                return CreatedAtAction(nameof(GetById), new { id = nhanVienVM.Tendangnhap }, nhanVienVM);
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

        // PUT: api/nhanvien/{id}
        [HttpPut("update")]
        [Authorize(Policy = "RequireAdminRole")] // Chỉ cho phép admin thực hiện
        public IActionResult Update([FromQuery] string id, [FromBody] NhanVienVM nhanVienVM)
        {
            try
            {
                _nhanVienService.Update(id, nhanVienVM);
                return Ok(nhanVienVM);
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

        // DELETE: api/nhanvien/{id}
        [HttpDelete("delete")]
        [Authorize(Policy = "RequireAdminRole")] // Chỉ cho phép admin thực hiện
        public IActionResult Delete([FromQuery] string id)
        {
            try
            {
                _nhanVienService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException)
            {
                return BadRequest("Không thể xóa nhân viên quản trị.");
            }
        }
    }


}


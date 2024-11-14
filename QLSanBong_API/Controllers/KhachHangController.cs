using Microsoft.AspNetCore.Mvc;
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
            var khachHangList = _khachHangService.GetAll(); // Gọi phương thức GetAll không tham số
            return Ok(khachHangList);
        }
        // GET: api/khachhang/{id}
        [HttpGet("getbyid")]
        public ActionResult<KhachHang> GetById([FromQuery] string id)
        {
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
                _khachHangService.Add(khachHangVM);
                return CreatedAtAction(nameof(GetById), new { id = khachHangVM.Tendangnhap }, khachHangVM);
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

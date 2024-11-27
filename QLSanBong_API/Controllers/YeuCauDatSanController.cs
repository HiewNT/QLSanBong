using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong_API.Helpers;
using QLSanBong_API.Models; // Namespace chứa các lớp Models
using QLSanBong_API.Services.IService; // Namespace chứa các interface dịch vụ

namespace QLSanBong_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class YeuCauDatSanController : ControllerBase
    {
        private readonly IYeuCauDatSanService _yeuCauDatSanService;

        public YeuCauDatSanController(IYeuCauDatSanService yeuCauDatSanService)
        {
            _yeuCauDatSanService = yeuCauDatSanService;
        }

        [HttpGet("getall")]
        public ActionResult<IEnumerable<YeuCauDatSan>> GetAll()
        {
            // Lấy role từ header
            var role = Request.Headers["Role"].ToString();
            if (!PermissionHelper.HasPermission(User,role, "yeucau_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền xem danh sách yêu cầu đặt sân."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            var result = _yeuCauDatSanService.GetAll();
            return Ok(result);
        }

        [HttpGet("getbyid")]
        public ActionResult<YeuCauDatSan> GetById([FromQuery] int id)
        {
            // Lấy role từ header
            var role = Request.Headers["Role"].ToString();
            if (!PermissionHelper.HasPermission(User,role, "yeucau_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập chức năng này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            var result = _yeuCauDatSanService.GetByID(id);
            if (result == null)
            {
                return NotFound($"Yêu cầu đặt sân với ID {id} không tồn tại.");
            }
            return Ok(result);
        }

        [HttpGet("getbykh")]
        public ActionResult<YeuCauDatSan> GetByKH([FromQuery] string makh)
        {
            // Lấy role từ header
            var role = Request.Headers["Role"].ToString();
            if (!PermissionHelper.HasPermission(User,role, "yeucau_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập chức năng này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            var result = _yeuCauDatSanService.GetByKH(makh);
            if (result == null)
            {
                return NotFound($"Yêu cầu đặt sân với ID {makh} không tồn tại.");
            }
            return Ok(result);
        }
        

        [HttpPost("getby")]
        public ActionResult<YeuCauDatSan> GetBy([FromBody] GetYCDSRequest request)
        {
            var result = _yeuCauDatSanService.GetBy(request);
            if (result == null)
            {
                return NotFound($"Yêu cầu đặt sân với ID {request.Id} không tồn tại.");
            }
            return Ok(result);
        }


        [HttpPost("add")]
        public ActionResult Add([FromBody] YeuCauDatSanAdd yeuCauDatSanAdd)
        {
            // Lấy role từ header
            var role = Request.Headers["Role"].ToString();
            if (!PermissionHelper.HasPermission(User,role, "yeucau_add"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập chức năng này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            // Gọi hàm Add từ service
            _yeuCauDatSanService.Add(yeuCauDatSanAdd);
            return CreatedAtAction(nameof(GetById), yeuCauDatSanAdd);
        }

        [HttpPut("update")]
        public ActionResult Update([FromBody] UpdateYCDSRequest request)
        {
            try
            {
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();
                if (!PermissionHelper.HasPermission(User,role, "yeucau_edit"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                // Gọi hàm Update từ service
                _yeuCauDatSanService.Update(request);
                return Ok("Success");
            }
            catch (Exception ex) // Thay KeyNotFoundException bằng Exception để bắt tất cả các loại lỗi
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("delete")]
        public ActionResult Delete([FromQuery] int id)
        {
            try
            {
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();
                if (!PermissionHelper.HasPermission(User,role, "yeucau_delete"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                _yeuCauDatSanService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}

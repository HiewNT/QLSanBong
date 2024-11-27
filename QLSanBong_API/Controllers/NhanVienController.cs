using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong_API.Helpers;
using QLSanBong_API.Models;

namespace QLSanBong_API.Controllers
{
    //[Authorize]
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
            try
            {
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();

                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User,role, "nhanvien_read"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền xem danh sách nhân viên."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }


                // Lấy danh sách nhân viên từ service
                var nhanVienList = _nhanVienService.GetAll();

                // Lọc bỏ nhân viên có MaNv là "NV00000"
                var filteredList = nhanVienList.Where(nv => nv.User.Username != "admin").ToList();

                // Trả về danh sách đã lọc
                return Ok(filteredList);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu xảy ra
                return StatusCode(StatusCodes.Status500InternalServerError, $"Đã xảy ra lỗi: {ex.Message}");
            }
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
        //[Authorize(Policy = "RequireAdminRole")] // Chỉ cho phép admin thực hiện
        public ActionResult<NhanVien> Add(NhanVienVM nhanVienVM)
        {
            try
            {
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();
                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User, role,"nhanvien_add"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                _nhanVienService.Add(nhanVienVM);
                return CreatedAtAction(nameof(GetById), new { id = nhanVienVM.User.Username }, nhanVienVM);
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
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();
                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User,role, "nhanvien_edit"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
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
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();
                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User,role, "nhanvien_delete"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
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


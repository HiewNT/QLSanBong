using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong_API.Helpers;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;

namespace QLSanBong_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GiaGioThueController : ControllerBase
    {
        private readonly IGiaGioThueService _giaGioThueService;

        public GiaGioThueController(IGiaGioThueService giaGioThueService)
        {
            _giaGioThueService = giaGioThueService;
        }

        // GET: api/giagiothue
        [HttpGet("getall")]
        public ActionResult<GiaGioThueVM> GetAll()
        {
            if (!PermissionHelper.HasPermission(User, "giathue_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập chức năng này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            var giaThueList = _giaGioThueService.GetAll();
            return Ok(giaThueList);
        }

        /*
         public ActionResult<IEnumerable<GiaGioThueVM>> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? from,
            [FromQuery] string? to,
            [FromQuery] string? sortBy,
            [FromQuery] int page = 1)
        {
            var giaGioThueList = _giaGioThueService.GetAll(search, from, to, sortBy, page);
            return Ok(giaGioThueList);
        }
        */

        [Authorize(Roles = "Admin,NhanVien,KhachHang")]
        // GET: api/giagiothue/{id}
        [HttpGet("getbyid")]
        public ActionResult<GiaGioThueVM> GetById([FromQuery] string id)
        {
            if (!PermissionHelper.HasPermission(User, "giathue_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập chức năng này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            var giaGioThue = _giaGioThueService.GetById(id);
            if (giaGioThue == null)
            {
                return NotFound();
            }
            return Ok(giaGioThue);
        }

        [Authorize(Roles = "Admin")]
        // POST: api/giagiothue
        [HttpPost("add")]
        public ActionResult<GiaGioThueVM> Add(GiaGioThueVM giaGioThueVM)
        {
            try
            {
                if (!PermissionHelper.HasPermission(User, "giathue_add"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                _giaGioThueService.Add(giaGioThueVM);
                return CreatedAtAction(nameof(GetById), new { id = giaGioThueVM.MaGio }, giaGioThueVM);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        // PUT: api/giagiothue/{id}
        [HttpPut("update")]
        public IActionResult Update([FromQuery] string id, GiaGioThueVM1 giaGioThueVM1)
        {
            try
            {
                if (!PermissionHelper.HasPermission(User, "giathue_edit"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                _giaGioThueService.Update(id, giaGioThueVM1);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        // DELETE: api/giagiothue/{id}
        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] string id)
        {
            try
            {
                if (!PermissionHelper.HasPermission(User, "giathue_delete"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                _giaGioThueService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}

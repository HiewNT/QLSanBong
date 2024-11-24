using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLSanBong_API.Helpers;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;

namespace QLSanBong_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PhieuDatSanController : ControllerBase
    {
        private readonly IPhieuDatSanService _phieuDatSanService;

        public PhieuDatSanController(IPhieuDatSanService phieuDatSanService)
        {
            _phieuDatSanService = phieuDatSanService;
        }

        // GET: api/phieudatsan
        [HttpGet("getall")]
        public ActionResult<IEnumerable<Models.PhieuDatSan>> GetAll()
        {
            if (!PermissionHelper.HasPermission(User, "hoadon_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập chức năng này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            var phieuDatSans = _phieuDatSanService.GetAll();
            return Ok(phieuDatSans);
        }

        // GET: api/phieudatsan/{id}
        [HttpGet("getbykh")]
        public ActionResult<IEnumerable<PhieuDatSan>> GetByKH([FromQuery] string maKh)
        {
            if (!PermissionHelper.HasPermission(User, "hoadon_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập chức năng này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            var phieuDatSan = _phieuDatSanService.GetByKH(maKh);
            if (phieuDatSan == null)
            {
                return NotFound();
            }
            return Ok(phieuDatSan);
        }
        // GET: api/phieudatsan/{id}
        [HttpGet("getbynv")]
        public ActionResult<IEnumerable<PhieuDatSan>> GetByNV([FromQuery] string maNv)
        {
            if (!PermissionHelper.HasPermission(User, "hoadon_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập chức năng này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            var phieuDatSan = _phieuDatSanService.GetByNV(maNv);
            if (phieuDatSan == null)
            {
                return NotFound();
            }
            return Ok(phieuDatSan);
        }

        // GET: api/phieudatsan/{id}
        [HttpGet("getbyid")]
        public ActionResult<Models.PhieuDatSan> GetById([FromQuery] string id)
        {
            if (!PermissionHelper.HasPermission(User, "hoadon_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập chức năng này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            var phieuDatSan = _phieuDatSanService.GetById(id);
            if (phieuDatSan == null)
            {
                return NotFound();
            }
            return Ok(phieuDatSan);
        }

        // POST: api/phieudatsan
        [HttpPost("add")]
        public ActionResult Add([FromBody] PDSAdd pDSAdd)
        {
            if (!PermissionHelper.HasPermission(User, "hoadon_add"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền truy cập chức năng này."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            try
            {
                _phieuDatSanService.Add(pDSAdd);
                return CreatedAtAction(nameof(GetById), new { pDSAdd });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/phieudatsan/{id}
        [HttpPut("update")]
        public ActionResult Update([FromQuery] string id, [FromBody] PDSAdd pDSAdd)
        {
            try
            {
                if (!PermissionHelper.HasPermission(User, "hoadon_edit"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                _phieuDatSanService.Update(id, pDSAdd);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/phieudatsan/{id}
        [HttpDelete("delete")]
        public ActionResult Delete([FromQuery] string id)
        {
            try
            {
                if (!PermissionHelper.HasPermission(User, "hoadon_delete"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền truy cập chức năng này."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                _phieuDatSanService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;

namespace QLSanBong_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        #region Action


        // Action
        [HttpGet("getallaction")]
        public IActionResult GetAllActions()
        {
            var actions = _authService.GetAllAction();
            return Ok(actions);
        }

        [HttpPost("addaction")]
        public IActionResult AddAction([FromBody] Models.Action action)
        {
            try
            {
                var actionId = _authService.AddAction(action);
                return Ok(new { Message = "Thêm hành động thành công!", ActionId = actionId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpDelete("deleteaction")]
        public IActionResult DeleteAction([FromQuery] string id)
        {
            try
            {
                var result = _authService.DeleteAction(id);
                if (result)
                {
                    return Ok(new { Message = "Xóa hành động thành công!" });
                }
                return NotFound(new { Message = "Không tìm thấy hành động cần xóa!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPut("updateaction")]
        public IActionResult UpdateAction([FromQuery] string id, [FromBody] string newActionName)
        {
            try
            {
                var result = _authService.UpdateAction(id, newActionName);
                return Ok(new { Message = "Cập nhật hành động thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        #endregion


        #region Service


        // Service
        [HttpPost("addservice")]
        public IActionResult AddService([FromBody] Models.Service service)
        {
            try
            {
                var serviceId = _authService.AddService(service);
                return Ok(new { Message = "Thêm dịch vụ thành công!", ServiceId = serviceId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("getallservice")]
        public IActionResult GetAllServices()
        {
            var services = _authService.GetAllService();
            return Ok(services);
        }

        [HttpDelete("deleteservice")]
        public IActionResult DeleteService([FromQuery] string id)
        {
            try
            {
                var result = _authService.DeleteService(id);
                if (result)
                {
                    return Ok(new { Message = "Xóa dịch vụ thành công!" });
                }
                return NotFound(new { Message = "Không tìm thấy dịch vụ cần xóa!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPut("updateservice")]
        public IActionResult UpdateService([FromQuery] string id, [FromBody] string newServiceName)
        {
            try
            {
                var result = _authService.UpdateService(id, newServiceName);
                return Ok(new { Message = "Cập nhật dịch vụ thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        #endregion

        // API endpoint để lấy authId cho một dịch vụ và hành động
        [HttpGet("getauth")]
        public IActionResult GetAuth([FromQuery] string serviceId, [FromQuery] string actionId)
        {
            // Lấy authId từ service
            var authId = _authService.GetAuth(serviceId, actionId);

            if (authId == null)
            {
                return NotFound("Không tìm thấy authId cho dịch vụ và hành động này.");
            }

            // Trả về kết quả dưới dạng JSON
            return Ok(new { authId });
        }



        [HttpGet("getallactionservice")]
        public IActionResult GetAllActionService()
        {
            var services = _authService.GetAllActionService();
            return Ok(services);
        }

        [HttpPost("syncactionservice")]
        public IActionResult SyncActionService()
        {
            try
            {
                _authService.SyncActionService();
                return Ok(new { Message = "Đồng bộ bảng ActionService thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi đồng bộ dữ liệu.", Error = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using QLSanBong_API.Helpers;
using QLSanBong_API.Models;
using QLSanBong_API.Services.IService;

namespace QLSanBong_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IKhachHangService _khachHangService;

        public RoleController(IRoleService roleService, IKhachHangService khachHangService)
        {
            _roleService = roleService;
            _khachHangService = khachHangService;
        }


        #region Role

        [HttpGet("getallrole")]
        public IActionResult GetAll()
        {
            // Lấy role từ header
            var role = Request.Headers["Role"].ToString();

            // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
            if (!PermissionHelper.HasPermission(User, role, "vaitro_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền xem danh sách nhân viên."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            var roles = _roleService.GetAll();
            return Ok(roles);
        }

        [HttpPost("addrole")]
        public IActionResult AddRole([FromBody] RoleVM roleName)
        {
            try
            {
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();

                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User, role, "vaitro_add"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền xem danh sách nhân viên."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                var roleId = _roleService.AddRole(roleName); // Gọi service để thêm vai trò
                return Ok(new { RoleId = roleId, Message = "Vai trò được thêm thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi, vui lòng thử lại sau.", Detail = ex.Message });
            }
        }

        [HttpDelete("deleterole")]
        public IActionResult DeleteRole([FromQuery] string roleId)
        {
            try
            {
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();

                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User, role, "vaitro_delete"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền xem danh sách nhân viên."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                var success = _roleService.DeleteRole(roleId);
                return Ok(new { Success = success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("updaterole")]
        public IActionResult UpdateRole([FromQuery] string roleId, [FromBody] RoleVM newRole)
        {
            try
            {
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();

                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User, role, "vaitro_update"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền xem danh sách nhân viên."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                var success = _roleService.UpdateRole(roleId, newRole);
                return Ok(new { Success = success });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        #endregion



        #region User


        [HttpGet("getalluser")]
        public IActionResult GetAllUser()
        {
            // Lấy role từ header
                var role = Request.Headers["Role"].ToString();

                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User,role, "nguoidung_read"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền xem danh sách người dùng."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
            var roles = _roleService.GetAllUser();

            // Lọc bỏ nhân viên có MaNv là "NV00000"
            var filteredList = roles.Where(nv => nv.Username != "admin").ToList();
            return Ok(filteredList);
        }

        [HttpPost("adduser")]
        public IActionResult AddUser([FromBody] UserAddVM user)
        {
            try
            {
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();

                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User,role, "nguoidung_add"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền xem danh sách nhân viên."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                // Gọi service để thêm người dùng
                _roleService.AddUser(user);

                // Nếu không có lỗi, trả về thông báo thành công
                return Ok(new { message = "Thêm người dùng thành công." });
            }
            catch (InvalidOperationException ex)
            {
                // Trả về lỗi với thông báo cụ thể
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi xử lý yêu cầu." });
            }
        }



        [HttpDelete("deleteuser")]
        public IActionResult DeleteUser([FromQuery] string userID)
        {
            try
            {
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();

                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User,role, "nguoidung_delete"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền xem danh sách nhân viên."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                // Gọi service để xóa người dùng
                bool result = _roleService.DeleteUser(userID);

                if (result)
                {
                    return Ok(new { message = "User deleted successfully." });
                }
                else
                {
                    return NotFound(new { message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return StatusCode(500, new { message = "An error occurred while deleting the user.", error = ex.Message });
            }
        }

        [HttpGet("getalluserrole")]
        public IActionResult GetAllUserRole()
        {
            var roles = _roleService.GetAllUserRole();
            return Ok(roles);
        }

        [HttpGet("getuserbyrole")]
        public IActionResult GetUserByRole([FromQuery] string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("RoleID không được để trống.");
            }

            try
            {
                // Gọi phương thức từ service hoặc trực tiếp từ DbContext
                var users = _roleService.GetUserByRole(roleId);

                if (users == null)
                {
                    return NotFound($"Không tìm thấy người dùng nào với vai trò ID: {roleId}.");
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi xảy ra trong quá trình xử lý: {ex.Message}");
            }
        }

        [HttpDelete("deleteuserrole")]
        public IActionResult DeleteUserRole(UserRoleAdd request)
        {
            if (string.IsNullOrWhiteSpace(request.UserID)||string.IsNullOrWhiteSpace(request.RoleID))
            {
                return BadRequest("Thông tin không hợp lệ.");
            }

            var result = _roleService.DeleteUserRole(request);
            if (result)
            {
                return Ok("Xóa UserRole thành công.");
            }
            else
            {
                return NotFound("Không tìm thấy UserRole cần xóa.");
            }
        }

        [HttpPost("adduserrole")]
        public IActionResult AddUserRole([FromBody] UserRoleAdd request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserID) || string.IsNullOrWhiteSpace(request.RoleID))
            {
                return BadRequest("Dữ liệu UserRole không hợp lệ.");
            }

            var result = _roleService.AddUserRole(request);

            if (result == "Thêm UserRole thành công.")
            {
                return Ok(new { message = result });  // Trả về thông báo thành công
            }

            return BadRequest(new { message = result });  // Trả về thông báo lỗi
        }


        #endregion



        #region Authencation

        [HttpGet("getallroleauth")]
        public IActionResult GetAllRoleAuth()
        {
            // Lấy role từ header
            var role = Request.Headers["Role"].ToString();

            // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
            if (!PermissionHelper.HasPermission(User, role, "phanquyen_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền xem danh sách nhân viên."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            var roles = _roleService.GetAllRoleAuth();
            return Ok(roles);
        }

        [HttpGet("getauthbyrole")]
        public ActionResult<List<Models.RoleAuth>> GetAuthByRole([FromQuery] string roleId)
        {
            // Lấy role từ header
            var role = Request.Headers["Role"].ToString();

            // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
            if (!PermissionHelper.HasPermission(User, role, "phanquyen_read"))
            {
                // Tạo đối tượng thông báo dưới dạng JSON
                var errorResponse = new
                {
                    success = false,
                    message = "Bạn không có quyền xem danh sách nhân viên."
                };

                return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
            }
            // Kiểm tra roleId có null hoặc rỗng
            if (string.IsNullOrEmpty(roleId))
            {
                return BadRequest("RoleId không được để trống.");
            }

            // Lấy danh sách quyền từ service
            var result = _roleService.GetAuthByRole(roleId);

            // Trả về danh sách rỗng nếu không tìm thấy quyền
            if (result == null || !result.Any())
            {
                return Ok(new List<Models.RoleAuth>()); // Trả về danh sách rỗng
            }

            // Trả về danh sách quyền
            return Ok(result);
        }

        [HttpPost("addroleauth")]
        public async Task<IActionResult> AddRoleAuth(string roleId, string authId)
        {
            try
            {
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();

                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User, role, "phanquyen_add"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền xem danh sách nhân viên."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                await _roleService.AddRoleAuthAsync(roleId, authId);
                return Ok("Thêm quyền thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

        // Phương thức DELETE để xóa RoleAuth
        [HttpDelete("deleteroleauth")]
        public async Task<IActionResult> DeleteRoleAuth(string roleId, string authId)
        {
            try
            {
                // Lấy role từ header
                var role = Request.Headers["Role"].ToString();

                // Kiểm tra quyền "nhanvien_read" trước khi thực hiện logic
                if (!PermissionHelper.HasPermission(User, role, "phanquyen_delete"))
                {
                    // Tạo đối tượng thông báo dưới dạng JSON
                    var errorResponse = new
                    {
                        success = false,
                        message = "Bạn không có quyền xem danh sách nhân viên."
                    };

                    return StatusCode(StatusCodes.Status403Forbidden, errorResponse);
                }
                // Gọi service để xóa RoleAuth
                await _roleService.DeleteRoleAuthAsync(roleId, authId);
                return Ok("Xóa RoleAuth thành công.");
            }
            catch (InvalidOperationException ex)
            {
                // Trả về thông báo lỗi nếu RoleAuth không tồn tại
                return BadRequest($"Lỗi: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Trả về lỗi chung
                return StatusCode(StatusCodes.Status500InternalServerError, $"Lỗi: {ex.Message}");
            }
        }
    }
}

#endregion
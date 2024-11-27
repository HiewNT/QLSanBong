using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;

namespace QLSanBong_API.Helpers
{
    public static class PermissionHelper
    {
        /// <summary>
        /// Kiểm tra quyền dựa trên danh sách permissions trong token JWT và role của người dùng.
        /// </summary>
        /// <param name="userClaims">ClaimsPrincipal từ context của người dùng.</param>
        /// <param name="role">Role cần kiểm tra quyền.</param>
        /// <param name="requiredPermission">Quyền cần kiểm tra.</param>
        /// <returns>True nếu người dùng có quyền, ngược lại là False.</returns>
        public static bool HasPermission(ClaimsPrincipal userClaims, string role, string requiredPermission)
        {
            if (userClaims == null || string.IsNullOrEmpty(role) || string.IsNullOrEmpty(requiredPermission))
            {
                return false;
            }

            // Kiểm tra xem người dùng có role hay không
            var userRoles = userClaims.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            // Nếu người dùng không có role này, trả về false
            if (!userRoles.Contains(role))
            {
                return false;
            }

            // Lấy quyền của role từ claims, dựa trên tên của role
            var permissionClaim = userClaims.FindFirst($"Permission_{role}");
            if (permissionClaim == null)
            {
                return false; // Không có quyền cho role này
            }

            // Chuyển đổi giá trị quyền của role thành danh sách quyền
            var permissions = permissionClaim.Value.Split(',').ToList();

            // Kiểm tra xem quyền yêu cầu có tồn tại trong danh sách quyền của role không
            return permissions.Contains(requiredPermission);
        }
    }


}

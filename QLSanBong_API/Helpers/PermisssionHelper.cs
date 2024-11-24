using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace QLSanBong_API.Helpers
{
    public static class PermissionHelper
    {
        /// <summary>
        /// Kiểm tra quyền dựa trên danh sách permissions trong token JWT.
        /// </summary>
        /// <param name="userClaims">ClaimsPrincipal từ context của người dùng.</param>
        /// <param name="requiredPermission">Quyền cần kiểm tra.</param>
        /// <returns>True nếu người dùng có quyền, ngược lại là False.</returns>
        public static bool HasPermission(ClaimsPrincipal userClaims, string requiredPermission)
        {
            if (userClaims == null || string.IsNullOrEmpty(requiredPermission))
            {
                return false;
            }

            // Lấy danh sách quyền (permissions) từ claims
            var permissions = userClaims.Claims
                .Where(claim => claim.Type == "Permission")
                .Select(claim => claim.Value)
                .ToList();

            // Kiểm tra xem quyền yêu cầu có tồn tại trong danh sách không
            return permissions.Contains(requiredPermission);
        }
    }
}

using QLSanBong_API.Models;

namespace QLSanBong_API.Services.IService
{
    public interface IRoleService
    {

        #region Role

        string AddRole(RoleVM roleVM);
        List<Role> GetAll();
        bool DeleteRole(string roleId);
        bool UpdateRole(string roleId, RoleVM newRole);


        #endregion


        #region User
        List<User> GetAllUser();

        List<UserRole> GetUserByRole(string RoleId);
        List<UserRole> GetAllUserRole();

        string AddUserRole(UserRoleAdd request);
        bool DeleteUserRole(UserRoleAdd request);

        #endregion

        List<Models.RoleAuth> GetAllRoleAuth();
        List<Models.AuthByRole> GetAuthByRole(string roleId);

        Task AddRoleAuthAsync(string roleId, string authId);

        Task DeleteRoleAuthAsync(string roleId, string authId);
    }
}

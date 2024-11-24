namespace QLSanBong_API.Models
{
    public class RoleAuthVM
    {
        public string RoleName { get; set; } = null!;
        public virtual AuthInfo? AuthInfo { get; set; }
    }
    public partial class RoleAuth: RoleAuthVM
    {
        public string RoleID { get; set; } = null!;
        public string AuthID { get; set; } = null!;
    }
    public class RoleAuthRequest
    {
        public string RoleID { get; set; } = null!;
    }

    public class AuthByRole
    {
        public string RoleID { get; set; } = null!;
        public string AuthID { get; set; } = null!;
        public virtual AuthInfo? AuthInfo { get; set; }

    }
}

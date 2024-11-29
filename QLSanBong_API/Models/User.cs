namespace QLSanBong_API.Models
{
    public class UserAddVM
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Ten { get; set; }
        public string? Sdt { get; set; }
        public string? Diachi { get; set; }

    }
    public class UserAdd : UserAddVM
    {
        public string UserID { get; set; } = null!;

    }
    public class UserVM
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Name { get; set; }
        public string? SDT { get; set; }
        public string? Diachi { get; set; }

        public List<Role>? Role { get; set; }

    }
    public partial class User : UserVM
    {
        public string UserID { get; set; } = null!;
    }

    public class UserRoleVM
    {
        public string Username { get; set; } = null!;

        public string? Name { get; set; }
        public string? SDT { get; set; }

        public List<Role>? Role { get; set; }

    }
    public partial class UserRole : UserRoleVM
    {
        public string UserID { get; set; } = null!;
    }
    public class UserRoleAdd
    {
        public string UserID { get; set; } = null!;
        public string RoleID { get; set; } = null!;
    }
    public class RoleVM
    {
        public string RoleName { get; set; } = null!;
        public string ThongTin { get; set; } = null!;
    }
    public partial class Role : RoleVM
    {
        public string RoleID { get; set; } = null!;
    }

}

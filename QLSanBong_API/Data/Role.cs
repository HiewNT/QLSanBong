using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class Role
{
    public string RoleId { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public string? ThongTin { get; set; }

    public virtual ICollection<RoleAuth> RoleAuths { get; set; } = new List<RoleAuth>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Ten { get; set; }

    public string? Sdt { get; set; }

    public string? Diachi { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class RoleAuth
{
    public string RoleId { get; set; } = null!;

    public string AuthId { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual ActionService Auth { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}

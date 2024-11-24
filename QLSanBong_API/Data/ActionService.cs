using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class ActionService
{
    public string Id { get; set; } = null!;

    public string? ActionId { get; set; }

    public string? ServiceId { get; set; }

    public virtual Action? Action { get; set; }

    public virtual ICollection<RoleAuth> RoleAuths { get; set; } = new List<RoleAuth>();

    public virtual Service? Service { get; set; }
}

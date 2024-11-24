using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class Service
{
    public string ServiceId { get; set; } = null!;

    public string? ServiceName { get; set; }

    public virtual ICollection<ActionService> ActionServices { get; set; } = new List<ActionService>();
}

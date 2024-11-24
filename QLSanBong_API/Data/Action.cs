using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class Action
{
    public string ActionId { get; set; } = null!;

    public string? ActionName { get; set; }

    public virtual ICollection<ActionService> ActionServices { get; set; } = new List<ActionService>();
}

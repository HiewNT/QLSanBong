using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class Quyen
{
    public string MaQuyen { get; set; } = null!;

    public string? TenQuyen { get; set; }

    public virtual ICollection<NhomQuyenCn> NhomQuyenCns { get; set; } = new List<NhomQuyenCn>();
}

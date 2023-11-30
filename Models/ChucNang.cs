using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class ChucNang
{
    public int MaCn { get; set; }

    public string? TenCn { get; set; }

    public virtual ICollection<NhomQuyenCn> NhomQuyenCns { get; set; } = new List<NhomQuyenCn>();
}

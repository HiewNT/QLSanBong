using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class NhomNguoiDung
{
    public int MaNhom { get; set; }

    public string? TenNhom { get; set; }

    public virtual ICollection<NhomQuyenCn> NhomQuyenCns { get; set; } = new List<NhomQuyenCn>();
}

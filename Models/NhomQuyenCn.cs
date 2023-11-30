using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class NhomQuyenCn
{
    public string MaQuyen { get; set; } = null!;

    public int MaNhom { get; set; }

    public int MaCn { get; set; }

    public string? GhiChu { get; set; }

    public virtual ChucNang MaCnNavigation { get; set; } = null!;

    public virtual NhomNguoiDung MaNhomNavigation { get; set; } = null!;

    public virtual Quyen MaQuyenNavigation { get; set; } = null!;
}

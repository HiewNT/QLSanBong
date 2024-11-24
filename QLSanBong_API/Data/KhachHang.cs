using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class KhachHang
{
    public string MaKh { get; set; } = null!;

    public string TenKh { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public string? Gioitinh { get; set; }

    public string? Diachi { get; set; }

    public string UserId { get; set; } = null!;

    public virtual ICollection<PhieuDatSan> PhieuDatSans { get; set; } = new List<PhieuDatSan>();

    public virtual User User { get; set; } = null!;
}

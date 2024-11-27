using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class ChiTietPd
{
    public string MaPds { get; set; } = null!;

    public string MaGio { get; set; } = null!;

    public string MaSb { get; set; } = null!;

    public DateOnly Ngaysudung { get; set; }

    public string? Ghichu { get; set; }

    public virtual PhieuDatSan MaPdsNavigation { get; set; } = null!;
}

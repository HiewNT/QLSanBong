using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class ChiTietPd
{
    public string MaPds { get; set; } = null!;

    public DateTime? Ngaysudung { get; set; }

    public string Magio { get; set; } = null!;

    public decimal? Giatien { get; set; }

    public string? Ghichu { get; set; }

    public string MaSb { get; set; } = null!;

    public TimeSpan? Giobatdau { get; set; }

    public TimeSpan? Gioketthuc { get; set; }

    public virtual PhieuDatSan MaPdsNavigation { get; set; } = null!;

    public virtual SanBong MaSbNavigation { get; set; } = null!;

    public virtual GiaGioThue MagioNavigation { get; set; } = null!;
}

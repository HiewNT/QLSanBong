using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class PhieuHuy
{
    public string Maphieuhuy { get; set; } = null!;

    public string? Tennguoidat { get; set; }

    public string? Sdt { get; set; }

    public DateTime? Thoigiandat { get; set; }

    public DateTime? Ngaysudung { get; set; }

    public string? Magio { get; set; }

    public string? MaSb { get; set; }

    public string? Lydo { get; set; }
}

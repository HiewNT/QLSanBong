using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class ChiTietYcd
{
    public int Stt { get; set; }

    public string MaSb { get; set; } = null!;

    public string Magio { get; set; } = null!;

    public DateOnly Ngaysudung { get; set; }

    public string? TrangThai { get; set; }

    public string? GhiChu { get; set; }

    public virtual SanBong MaSbNavigation { get; set; } = null!;

    public virtual GiaGioThue MagioNavigation { get; set; } = null!;

    public virtual YeuCauDatSan SttNavigation { get; set; } = null!;
}

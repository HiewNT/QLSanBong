using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class ChiTietYcd
{
    public int Stt { get; set; }

    public string MaSb { get; set; } = null!;

    public string Magio { get; set; } = null!;

    public DateTime? Ngaysudung { get; set; }

    public TimeSpan? Giobatdau { get; set; }

    public TimeSpan? Gioketthuc { get; set; }

    public decimal? GiaTien { get; set; }

    public string? TrangThai { get; set; }

    public virtual YeuCauDatSan SttNavigation { get; set; } = null!;
}

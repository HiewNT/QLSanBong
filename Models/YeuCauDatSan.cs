using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class YeuCauDatSan
{
    public int Stt { get; set; }

    public string? Tennguoidat { get; set; }

    public string? Sdt { get; set; }

    public DateTime? Thoigiandat { get; set; }

    public string? GhiChu { get; set; }

    public string? Phuongthuctt { get; set; }

    public decimal? TongTien { get; set; }

    public virtual ICollection<ChiTietYcd> ChiTietYcds { get; set; } = new List<ChiTietYcd>();
}

using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class PhieuDatSan
{
    public string MaPds { get; set; } = null!;

    public DateTime? Ngaylap { get; set; }

    public decimal? TongTien { get; set; }

    public string? GhiChu { get; set; }

    public int? Sttds { get; set; }

    public string MaNv { get; set; } = null!;

    public string MaKh { get; set; } = null!;

    public virtual ICollection<ChiTietPd> ChiTietPds { get; set; } = new List<ChiTietPd>();
}

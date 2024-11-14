using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class YeuCauDatSan
{
    public int Stt { get; set; }

    public string MaKh { get; set; } = null!;

    public DateTime? Thoigiandat { get; set; }

    public string? GhiChu { get; set; }

    public decimal? TongTien { get; set; }

    public virtual ICollection<ChiTietYcd> ChiTietYcds { get; set; } = new List<ChiTietYcd>();

    public virtual KhachHang MaKhNavigation { get; set; } = null!;
}

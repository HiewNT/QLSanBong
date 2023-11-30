using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class PhieuDatSan
{
    public string MaPds { get; set; } = null!;

    public string? MaNv { get; set; }

    public string? TenKh { get; set; }

    public string Sdt { get; set; } = null!;

    public DateTime? Ngaylap { get; set; }

    public decimal? TongTien { get; set; }

    public string? Phuongthuctt { get; set; }

    public string? GhiChu { get; set; }

    public int? Sttds { get; set; }

    public virtual ICollection<ChiTietPd> ChiTietPds { get; set; } = new List<ChiTietPd>();

    public virtual NhanVien? MaNvNavigation { get; set; }
}

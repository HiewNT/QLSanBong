using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class VHoaDon
{
    public string MaPds { get; set; } = null!;

    public DateTime? Ngaydatsan { get; set; }

    public string? Magio { get; set; }

    public decimal? Giatien { get; set; }

    public string? PhuongthucTt { get; set; }

    public string? Ghichu { get; set; }

    public string? MaSb { get; set; }

    public string? MaNv { get; set; }

    public string? TenKh { get; set; }

    public string Sdt { get; set; } = null!;

    public DateTime? Ngaylap { get; set; }
}

using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class SanBong
{
    public string MaSb { get; set; } = null!;

    public string TenSb { get; set; } = null!;

    public string Trangthai { get; set; } = null!;

    public string? Dientich { get; set; }

    public string? Ghichu { get; set; }

    public string? Hinhanh { get; set; }

    public string? LoaiSan { get; set; }

    public string? GiaSan { get; set; }

    public string? DiaChi { get; set; }

    public virtual ICollection<ChiTietPd> ChiTietPds { get; set; } = new List<ChiTietPd>();
}

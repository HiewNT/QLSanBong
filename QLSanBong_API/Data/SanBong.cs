using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class SanBong
{
    public string MaSb { get; set; } = null!;

    public string TenSb { get; set; } = null!;

    public string? Dientich { get; set; }

    public string? Ghichu { get; set; }

    public string? Hinhanh { get; set; }

    public string? DiaChi { get; set; }

    public virtual ICollection<ChiTietPd> ChiTietPds { get; set; } = new List<ChiTietPd>();

    public virtual ICollection<ChiTietYcd> ChiTietYcds { get; set; } = new List<ChiTietYcd>();
}

using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class GiaGioThue
{
    public string MaGio { get; set; } = null!;

    public TimeOnly? Giobatdau { get; set; }

    public TimeOnly? Gioketthuc { get; set; }

    public decimal? Dongia { get; set; }

    public string? Ghichu { get; set; }

    public virtual ICollection<ChiTietPd> ChiTietPds { get; set; } = new List<ChiTietPd>();

    public virtual ICollection<ChiTietYcd> ChiTietYcds { get; set; } = new List<ChiTietYcd>();
}

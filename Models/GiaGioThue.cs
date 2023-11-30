using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class GiaGioThue
{
    public string Magio { get; set; } = null!;

    public TimeSpan? Giobatdau { get; set; }

    public TimeSpan? Gioketthuc { get; set; }

    public decimal? Dongia { get; set; }

    public string? Ghichu { get; set; }

    public virtual ICollection<ChiTietPd> ChiTietPds { get; set; } = new List<ChiTietPd>();
}

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
}

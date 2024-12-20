﻿using System;
using System.Collections.Generic;

namespace QLSanBong_API.Data;

public partial class NhanVien
{
    public string MaNv { get; set; } = null!;

    public string? TenNv { get; set; }

    public string? Chucvu { get; set; }

    public string? Sdt { get; set; }

    public string UserId { get; set; } = null!;

    public virtual ICollection<PhieuDatSan> PhieuDatSans { get; set; } = new List<PhieuDatSan>();

    public virtual User User { get; set; } = null!;
}

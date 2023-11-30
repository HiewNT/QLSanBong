using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class KhachHang
{
    public string MaKh { get; set; } = null!;

    public string TenKh { get; set; } = null!;

    public string Sdt { get; set; } = null!;

    public string? Gioitinh { get; set; }

    public string? Diachi { get; set; }

    public string Tendangnhap { get; set; } = null!;

    public virtual TaiKhoan TendangnhapNavigation { get; set; } = null!;
}

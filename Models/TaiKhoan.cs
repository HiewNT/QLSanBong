using System;
using System.Collections.Generic;

namespace QLSanBong.Models;

public partial class TaiKhoan
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Quyen { get; set; }

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}

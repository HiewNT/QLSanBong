namespace QLSanBong_API.Models
{

    public class NhanVienVM
    {

        public string? TenNv { get; set; }

        public string? Chucvu { get; set; }

        public string? Sdt { get; set; }
        public string Tendangnhap { get; set; } = null!;
        public virtual TaiKhoanVM? TaiKhoan { get; set; }
    }
    public partial class NhanVien : NhanVienVM
    {
        public string MaNv { get; set; } = null!;

    }
    public class NhanVienDS
    {
        public string? TenNv { get; set; }

    }
}
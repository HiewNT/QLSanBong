namespace QLSanBong_API.Models
{

    public class NhanVienVM
    {

        public string? TenNv { get; set; }

        public string? Chucvu { get; set; }
        public string? Diachi { get; set; }

        public string? Sdt { get; set; }
        public virtual UserVM? User { get; set; }
    }
    public partial class NhanVien : NhanVienVM
    {
        public string UserID { get; set; } = null!;

    }
    public class NhanVienDS
    {
        public string? TenNv { get; set; }

        public string Sdt { get; set; } = null!;

        public string? Diachi { get; set; }

    }
}
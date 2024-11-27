namespace QLSanBong_API.Models
{

    public class KhachHangVM
    {

        public string TenKh { get; set; } = null!;

        public string Sdt { get; set; } = null!;

        public string? Diachi { get; set; }
        public virtual UserVM? User { get; set; }
    }
    public partial class KhachHang : KhachHangVM
    {
        public string UserID { get; set; } = null!;
    }
    public class KhachHangDS
    {
        public string TenKh { get; set; } = null!;

        public string Sdt { get; set; } = null!;

        public string? Diachi { get; set; }
    }
}

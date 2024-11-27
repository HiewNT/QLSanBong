namespace QLSanBong_API.Models
{
    public class PhieuDatSanVM
    {
        public string? Ngaylap { get; set; }

        public string? TongTien { get; set; }

        public string? GhiChu { get; set; }

        public int? Sttds { get; set; }
        public List<ChiTietPds>? ChiTietPds { get; set; }
        public KhachHangDS KhachHangDS { get; set; }
        public NhanVienDS NhanVienDS { get; set; }
    }
    public partial class PhieuDatSan : PhieuDatSanVM
    {
        public string MaPds { get; set; } = null!;
        public string MaNv { get; set; } = null!;

        public string MaKh { get; set; } = null!;

    }
    public class PDSAdd
    {
        public string MaNv { get; set; } = null!;

        public string MaKh { get; set; } = null!;
        public string? Ngaylap { get; set; }

        public string? GhiChu { get; set; }

        public int? Sttds { get; set; }
        public List<ChiTietPdsVM>? ChiTietPdsVM { get; set; }
    }
}

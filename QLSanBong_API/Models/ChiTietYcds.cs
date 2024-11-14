namespace QLSanBong_API.Models
{
    public class ChiTietYcdsVM
    {
        public int stt { get; set; }
        public string MaSb { get; set; } = null!;

        public string Magio { get; set; } = null!;

        public string? Ngaysudung { get; set; }


        public string? GhiChu { get; set; }

    }
    public partial class ChiTietYcds : ChiTietYcdsVM
    {
        public GiaGioThueVM1 GiaGioThueVM1 { get; set; } = new GiaGioThueVM1();
        public string? TrangThai { get; set; }
    }
}

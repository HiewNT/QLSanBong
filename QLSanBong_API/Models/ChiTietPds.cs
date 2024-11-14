namespace QLSanBong_API.Models
{

    public class ChiTietPdsVM
    {

        public string MaGio { get; set; } = null!;
        public string MaSb { get; set; } = null!;

        public string? Ngaysudung { get; set; }

        public string? Ghichu { get; set; }
    }

    public partial class ChiTietPds : ChiTietPdsVM
    {
        public string MaPds { get; set; } = null!;

        public GiaGioThueVM1 GiaGioThueVM1 { get; set; } = new GiaGioThueVM1();
    }
}

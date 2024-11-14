namespace QLSanBong_API.Models
{

    public class SanBongVM1
    {

        public string TenSb { get; set; } = null!;
        public string? DiaChi { get; set; }

    }
    public class SanBongVM
    {

        public string TenSb { get; set; } = null!;

        public string? Dientich { get; set; }

        public string? Ghichu { get; set; }

        public string? Hinhanh { get; set; }
        public string? DiaChi { get; set; }

    }
    public partial class SanBong : SanBongVM
    {
        public string MaSb { get; set; } = null!;
    }

    public class SanDaDatResult
    {
        public string? MaSb { get; set; }
        public string? Ngaysudung { get; set; }
        public string? Giobatdau { get; set; }
        public string? Gioketthuc { get; set; }
    }
    public class DSSanTrongVM
    {
        public string? MaSb { get; set; }
        public string? Ngaysudung { get; set; }
        public string? Magio { get; set; }

    }

    public partial class DSSanTrong : DSSanTrongVM
    {
        public GiaGioThueVM1 GiaGioThueVM1 { get; set; }
        public SanBongVM1 SanBongVM1 { get; set; }

    }

}
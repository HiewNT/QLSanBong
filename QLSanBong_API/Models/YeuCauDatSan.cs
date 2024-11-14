namespace QLSanBong_API.Models
{
    public class YeuCauDatSanVM
    {

        public string MaKh { get; set; } = null!;

        public string? Thoigiandat { get; set; }

        public string? GhiChu { get; set; }

        public decimal? TongTien { get; set; }
        public List<ChiTietYcds>? ChiTietYcds { get; set; }
        public KhachHangDS KhachHangDS { get; set; }
    }
    public partial class YeuCauDatSan : YeuCauDatSanVM
    {
        public int Stt { get; set; }

    }
    public class YeuCauDatSanAdd
    {
        public string MaKh { get; set; } = null!;

        public string? GhiChu { get; set; }
        public List<ChiTietYcdsVM>? ChiTietYcdsVM { get; set; }

    }

    public class GetYCDSRequest
    {
        public int Id { get; set; }
        public string MaSb { get; set; }
        public string Magio { get; set; }
        public string Ngaysudung { get; set; }
    }

    public class UpdateYCDSRequest : GetYCDSRequest
    {
        public string trangThai { get; set; }
    }

}

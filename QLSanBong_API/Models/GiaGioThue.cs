namespace QLSanBong_API.Models
{
    public partial class GiaGioThueVM : GiaGioThueVM1
    {
        public string MaGio { get; set; } = null!;

    }
    public class GiaGioThueVM1
    {

        public string? Giobatdau { get; set; }

        public string? Gioketthuc { get; set; }

        public decimal? Dongia { get; set; }

    }

}

namespace QLSanBong.Models
{
    public class Cart
    {
        public int? Id { get; set; } = null!;
        public string MaSb { get; set; } = null!;
        public string TenKh { get; set; } = null!;
        public DateTime NgayDat { get; set; }
        public string? Magio { get; set; }
        public TimeSpan? Giobatdau { get; set; }
        public TimeSpan? Gioketthuc { get; set; }
        public decimal? Giatien { get; set; }
    }
}

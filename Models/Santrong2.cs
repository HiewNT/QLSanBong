namespace QLSanBong.Models
{
    public class Santrong2
    {
        public string MaSb { get; set; } = null!;

        public DateTime? Ngaydatsan { get; set; }
        public string? Magio { get; set; }
        public TimeSpan? Giobatdau { get; set; }

        public TimeSpan? Gioketthuc { get; set; }
        public string Trangthai { get; set; } = null!;
        public decimal? Giatien { get; set; }
        public string? Loai { get; set; }
    }
}

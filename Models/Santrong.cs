namespace QLSanBong.Models
{
    public class Santrong
    {
        public string? MaSb { get; set; }

        public DateTime? Ngaydatsan { get; set; }
        public string? Magio { get; set; }
        public TimeSpan? Giobatdau { get; set; }

        public TimeSpan? Gioketthuc { get; set; }
        public string Trangthai { get; set; } = null!;
        public string? Loai { get; set; }
    }
}

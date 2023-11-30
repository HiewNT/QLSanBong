namespace QLSanBong.Models
{
    public class YCDSViewModel
    {
        public int Stt { get; set; }
        public string? Tennguoidat { get; set; }
        public string? Sdt { get; set; }
        public DateTime? Thoigiandat { get; set; }
        public DateTime? Ngaysudung { get; set; }
        public string? Magio { get; set; }
        public TimeSpan? Giobatdau { get; set; }
        public TimeSpan? Gioketthuc { get; set; }
        public string? MaSb { get; set; }
        public string? Phuongthuctt { get; set; }
        public string? TrangThai { get; set; }

        internal dynamic ToPagedList(int pagecho, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}

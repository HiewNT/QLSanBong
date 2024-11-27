namespace QLSanBong_API.Models
{
    public class LoginResponse
    {
        public string UserID { get; set; }
        public string Tennguoidung { get; set; }
        public string Manguoidung { get; set; }
        public string Sdt { get; set; }
        public List<string> Roles { get; set; }
        public Dictionary<string, List<string>> Permissions { get; set; }
        public string Token { get; set; }
    }

}

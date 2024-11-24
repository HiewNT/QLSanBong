namespace QLSanBong_API.Models
{
    public class Action
    {
        public string ActionId { get; set; } = null!;
        public string? ActionName { get; set; }
    }
    public class Service
    {
        public string ServiceId { get; set; } = null!;
        public string? ServiceName { get; set; }
    }
    public class ActionService
    {
        public string ID { get; set; } = null!;
        public string ActionId { get; set; } = null!;
        public string ServiceId { get; set; } = null!;
    }
    public class AuthInfo
    {
        public string ActionId { get; set; } = null!;
        public string ServiceId { get; set; } = null!;
        public string? ServiceName { get; set; }
        public string? ActionName { get; set; }

    }
}

using QLSanBong_API.Models;

public interface IKhachHangService
{
    // Các phương thức đã có
    // public List<KhachHang> GetAll(string search, string sortBy, int page = 1, int pageSize = 5);
    public List<KhachHang> GetAll();
    KhachHang? GetById(string id);
    void Add(KhachHangVM khachHangVM);
    void Delete(string id);
    void Update(string id, KhachHangVM khachHangVM);
}

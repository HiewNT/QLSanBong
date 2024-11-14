using QLSanBong_API.Models;

public interface INhanVienService
{
    List<NhanVien> GetAll();
    NhanVien? GetById(string id);
    void Add(NhanVienVM nhanVienVM);
    void Delete(string id);
    void Update(string id, NhanVienVM nhanVienVM);
}

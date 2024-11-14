using QLSanBong_API.Models;

namespace QLSanBong_API.Services.IService
{
    public interface IGiaGioThueService
    {
        // Các phương thức đã có
        //List<GiaGioThueVM> GetAll(string search, string? from, string? to, string sortBy, int page = 1);

        public List<GiaGioThueVM> GetAll();
        GiaGioThueVM? GetById(string id);
        void Add(GiaGioThueVM giaGioThueVM);
        void Delete(string id);
        void Update(string id, GiaGioThueVM1 giaGioThueVM1);
    }
}

using QLSanBong_API.Models;

namespace QLSanBong_API.Services.IService
{
    public interface ISanBongService
    {
        // Phương thức lấy tất cả các sân bóng
        IEnumerable<SanBong> GetAll();

        // Phương thức lấy sân bóng theo ID
        SanBong GetById(string id);
        public List<DSSanTrong> GetSanTrong(SanDaDatResult result);

        // Phương thức thêm mới sân bóng
        void Add(SanBongVM sanBongVM, IFormFile imageFile);

        // Phương thức cập nhật sân bóng
        void Update(string id, SanBongVM sanBongVM, IFormFile imageFile);

        // Phương thức xóa sân bóng theo ID
        void Delete(string id);
    }
}

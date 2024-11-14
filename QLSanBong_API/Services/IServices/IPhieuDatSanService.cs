using QLSanBong_API.Models;

namespace QLSanBong_API.Services.IService
{
    public interface IPhieuDatSanService
    {
        IEnumerable<PhieuDatSan> GetAll();
        IEnumerable<PhieuDatSan> GetByKH(string maKh);
        IEnumerable<PhieuDatSan> GetByNV(string maNv);
        PhieuDatSan GetById(string id);

        void Add(PDSAdd pDSAdd);

        void Update(string id, PDSAdd pDSAdd);

        void Delete(string id);
    }
}

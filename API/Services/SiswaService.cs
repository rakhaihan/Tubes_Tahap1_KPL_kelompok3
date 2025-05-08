using System.Collections.Generic;
using System.Linq;
using API.Models;

namespace API.Services
{
    public class SiswaService
    {
        private static List<Siswa> siswaList = new List<Siswa>();

        public List<Siswa> GetAll() => siswaList;

        public Siswa GetById(int id) => siswaList.FirstOrDefault(s => s.Id == id);

        public void Add(Siswa siswa) => siswaList.Add(siswa);

        public void Update(int id, Siswa updated)
        {
            var siswa = GetById(id);
            if (siswa != null)
            {
                siswa.Nama = updated.Nama;
                siswa.Kelas = updated.Kelas;
                siswa.TotalPoin = updated.TotalPoin;
                siswa.RiwayatPelanggaran = updated.RiwayatPelanggaran;
            }
        }

        public void Delete(int id)
        {
            var siswa = GetById(id);
            if (siswa != null)
                siswaList.Remove(siswa);
        }

        public void TambahPelanggaran(int siswaId, PoinPelanggaran pelanggaran)
        {
            var siswa = GetById(siswaId);
            if (siswa != null)
            {
                siswa.RiwayatPelanggaran.Add(pelanggaran);
                siswa.TotalPoin += pelanggaran.Poin;
            }
        }
    }
}

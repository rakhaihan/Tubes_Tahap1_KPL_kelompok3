using System.Collections.Generic;

namespace API.Models
{
    public class Siswa
    {
        public int Id { get; set; }
        public string Nama { get; set; }
        public string Kelas { get; set; }
        public int TotalPoin { get; set; } = 0;
        public List<PoinPelanggaran> RiwayatPelanggaran { get; set; } = new List<PoinPelanggaran>();
    }
}

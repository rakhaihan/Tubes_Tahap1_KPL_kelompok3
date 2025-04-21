using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tubes_Tahap1_KPL_kelompok3.table_driven
{
    public static class TabelPelanggaran
    {
        public static JenisPelanggaran[] Daftar = new JenisPelanggaran[]
        {
        new JenisPelanggaran("Tidak Membawa Buku", LevelPelanggaran.RINGAN, 5),
        new JenisPelanggaran("Terlambat Masuk", LevelPelanggaran.SEDANG, 10),
        new JenisPelanggaran("Merokok", LevelPelanggaran.BERAT, 20)
        };

        public static int GetPoin(string nama)
        {
            foreach (var jenis in Daftar)
            {
                if (jenis.Nama == nama)
                    return jenis.Poin;
            }
            return 0;
        }
    }
}

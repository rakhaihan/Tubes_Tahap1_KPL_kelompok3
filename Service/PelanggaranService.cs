using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tubes_Tahap1_KPL_kelompok3.Automata;
using Tubes_Tahap1_KPL_kelompok3.Model;
using Tubes_Tahap1_KPL_kelompok3.table_driven;

namespace Tubes_Tahap1_KPL_kelompok3.Service
{
    public class PelanggaranService
    {
        
        public void TambahPelanggaran(Siswa siswa, Pelanggaran pelanggaran)
        {
            
            siswa.RiwayatPelanggaran.Add(pelanggaran);
            siswa.TotalPoin += pelanggaran.Poin;

            
            int totalPoin = siswa.TotalPoin;
            string? sanksi = null;

            if (totalPoin >= 30)
            {
                sanksi = "Skorsing";
            }
            else if (totalPoin >= 20)
            {
                sanksi = "Panggilan Orang Tua";
            }
            else if (totalPoin >= 10)
            {
                sanksi = "Peringatan Lisan";
            }

            
            if (!string.IsNullOrEmpty(sanksi))
            {
                Console.WriteLine($"[NOTIF] {siswa.Nama} menerima sanksi: {sanksi}");
            }
        }

        
        public bool UbahStatusPelanggaran(Pelanggaran pelanggaran, Trigger trigger)
        {
            
            var sm = new PelanggaranStateMachine();
            sm.currentState = pelanggaran.Status;

            
            sm.activate(trigger);

            
            if (sm.currentState != pelanggaran.Status)
            {
                pelanggaran.Status = sm.currentState; 
                return true;
            }

            return false; 
        }
    }
}
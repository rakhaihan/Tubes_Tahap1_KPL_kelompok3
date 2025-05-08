using System;
using System.Collections.Generic;
using Tubes_Tahap1_KPL_kelompok3.Automata;
using Tubes_Tahap1_KPL_kelompok3.Model;
using Tubes_Tahap1_KPL_kelompok3.table_driven;

namespace Tubes_Tahap1_KPL_kelompok3.Service
{
    public class PelanggaranService
    {
        public void TambahPelanggaran(Siswa siswa, Pelanggaran pelanggaran)
        {
            // Defensive Programming: Validasi objek agar tidak null
            if (siswa == null)
                throw new ArgumentNullException(nameof(siswa), "Objek siswa tidak boleh null.");

            if (pelanggaran == null)
                throw new ArgumentNullException(nameof(pelanggaran), "Objek pelanggaran tidak boleh null.");

            // Menambahkan pelanggaran ke riwayat siswa
            siswa.RiwayatPelanggaran.Add(pelanggaran);
            siswa.TotalPoin += pelanggaran.Poin;

            // Menentukan sanksi berdasarkan total poin
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

            // Memberikan notifikasi jika ada sanksi
            if (!string.IsNullOrEmpty(sanksi))
            {
                Console.WriteLine($"[NOTIF] {siswa.Nama} menerima sanksi: {sanksi}");
            }
        }

        public bool UbahStatusPelanggaran(Pelanggaran pelanggaran, Trigger trigger)
        {
            // Validasi objek pelanggaran agar tidak null
            if (pelanggaran == null)
                throw new ArgumentNullException(nameof(pelanggaran), "Objek pelanggaran tidak boleh null.");

            var sm = new PelanggaranStateMachine();

            // Pastikan mesin status memulai dari status pelanggaran saat ini
            sm.Activate(trigger);

            // Jika status berubah, update pelanggaran
            if (sm.CurrentState != pelanggaran.Status)
            {
                pelanggaran.Status = sm.CurrentState;
                return true;
            }
            else
            {
                Console.WriteLine($"[INFO] Status pelanggaran '{pelanggaran.Jenis}' tidak berubah.");
            }

            return false;
        }

        public void TampilkanSemuaPelanggaran(SiswaService siswaService)
        {
            var semuaSiswa = siswaService.GetType()
                .GetField("_daftarSiswa", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(siswaService) as List<Siswa>;

            if (semuaSiswa == null || semuaSiswa.Count == 0)
            {
                Console.WriteLine("Belum ada siswa dalam sistem.");
                return;
            }

            bool adaPelanggaran = false;

            foreach (var siswa in semuaSiswa)
            {
                if (siswa.RiwayatPelanggaran.Count == 0)
                    continue;

                adaPelanggaran = true;

                Console.WriteLine($"\nNama Siswa: {siswa.Nama} | Kelas: {siswa.Kelas} | Total Poin: {siswa.TotalPoin}");
                Console.WriteLine("--------------------------------------------------");

                foreach (var pel in siswa.RiwayatPelanggaran)
                {
                    Console.WriteLine($"- Tanggal   : {pel.Tanggal.ToShortDateString()}");
                    Console.WriteLine($"  Jenis     : {pel.Jenis}");
                    Console.WriteLine($"  Poin      : {pel.Poin}");
                    Console.WriteLine($"  Status    : {pel.Status}");
                    Console.WriteLine();
                }
            }

            if (!adaPelanggaran)
            {
                Console.WriteLine("Belum ada pelanggaran yang tercatat.");
            }
        }

    }
}

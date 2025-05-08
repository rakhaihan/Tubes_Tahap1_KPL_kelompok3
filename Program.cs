using System;
using System.Collections.Generic;
using Tubes_Tahap1_KPL_kelompok3.Model;
using Tubes_Tahap1_KPL_kelompok3.Service;
using Tubes_Tahap1_KPL_kelompok3.table_driven;
using Tubes_Tahap1_KPL_kelompok3.Automata;
using Tubes_Tahap1_KPL_kelompok3.Utils;
using Tubes_Tahap1_KPL_kelompok3.Components;
using Tubes_Tahap1_KPL_kelompok3.Configuration;

namespace Tubes_Tahap1_KPL_kelompok3
{
    class Program
    {
        static void Main(string[] args)
        {
            var configManager = new ConfigManager();
            var config = configManager.Config;

            var siswaService = new SiswaService();
            var pelanggaranService = new PelanggaranService();
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n=== MENU UTAMA ===");
                Console.WriteLine("1. Tambah Siswa");
                Console.WriteLine("2. Lihat Semua Siswa");
                Console.WriteLine("3. Tambah Pelanggaran");
                Console.WriteLine("4. Lihat Pelanggaran Siswa");
                Console.WriteLine("5. Ubah Status Pelanggaran");
                Console.WriteLine("6. Keluar");
                Console.Write("Pilih menu (1-6): ");

                string pilihan = Console.ReadLine();

                switch (pilihan)
                {
                    case "1":
                        TambahSiswa(siswaService);
                        break;
                    case "2":
                        TampilkanSemuaSiswa(siswaService);
                        break;
                    case "3":
                        TambahPelanggaran(siswaService, pelanggaranService, config);
                        break;
                    case "4":
                        LihatPelanggaranSiswa(siswaService);
                        break;
                    case "5":
                        UbahStatusPelanggaran(siswaService, pelanggaranService);
                        break;
                    case "6":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid.");
                        break;
                }
            }

            Console.WriteLine("Program selesai.");
        }

        static void TambahSiswa(SiswaService siswaService)
        {
            Console.Write("Nama siswa: ");
            string nama = Console.ReadLine();
            Console.Write("Kelas: ");
            string kelas = Console.ReadLine();

            var siswa = new Siswa
            {
                Id = siswaService.GetSemua().Count + 1,
                Nama = nama,
                Kelas = kelas,
                TotalPoin = 0
            };

            siswaService.TambahSiswa(siswa);
            Console.WriteLine("Siswa berhasil ditambahkan.");
        }

        static void TampilkanSemuaSiswa(SiswaService siswaService)
        {
            Console.WriteLine("\n=== DAFTAR SISWA ===");
            var siswaList = siswaService.GetSemua();

            var columns = new Dictionary<string, Func<Siswa, string>>
            {
                { "ID", s => s.Id.ToString() },
                { "Nama", s => s.Nama },
                { "Kelas", s => s.Kelas },
                { "Total Poin", s => s.TotalPoin.ToString() }
            };

            TableRenderer.Render(siswaList, columns);
        }

        static void TambahPelanggaran(SiswaService siswaService, PelanggaranService pelanggaranService, AppConfig config)
        {
            Console.Write("Masukkan ID siswa: ");
            int id = int.Parse(Console.ReadLine());
            var siswa = siswaService.CariSiswa(id);

            if (siswa == null)
            {
                Console.WriteLine("Siswa tidak ditemukan.");
                return;
            }

            Console.WriteLine("\n=== DAFTAR PELANGGARAN ===");
            foreach (var item in TabelPelanggaran.Daftar)
            {
                Console.WriteLine($"- {item.Key} ({item.Value.Poin} poin)");
            }

            Console.Write("Pilih jenis pelanggaran: ");
            string jenis = Console.ReadLine();

            try
            {
                int poin = TabelPelanggaran.GetPoin(jenis);
                var pelanggaran = new Pelanggaran
                {
                    Jenis = jenis,
                    Poin = poin,
                    Tanggal = DateTime.Now,
                    Status = StatusPelanggaran.DILAPORKAN
                };

                pelanggaranService.TambahPelanggaran(siswa, pelanggaran);

                // Menggunakan konfigurasi batas poin untuk menentukan sanksi
                if (siswa.TotalPoin >= config.BatasPoinSkorsing)
                    Console.WriteLine("[NOTIF] Siswa diberikan sanksi: Skorsing");
                else if (siswa.TotalPoin >= config.BatasPoinPanggilanOrangTua)
                    Console.WriteLine("[NOTIF] Siswa harus dipanggil orang tua.");
                else if (siswa.TotalPoin >= config.BatasPoinPeringatan)
                    Console.WriteLine("[NOTIF] Siswa mendapat peringatan.");

                Console.WriteLine("Pelanggaran berhasil ditambahkan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void LihatPelanggaranSiswa(SiswaService siswaService)
        {
            Console.Write("Masukkan ID siswa: ");
            int id = int.Parse(Console.ReadLine());
            var siswa = siswaService.CariSiswa(id);

            if (siswa == null)
            {
                Console.WriteLine("Siswa tidak ditemukan.");
                return;
            }

            Console.WriteLine($"\n=== RIWAYAT PELANGGARAN {siswa.Nama} ===");
            var fieldGenerators = new Dictionary<string, Func<Pelanggaran, string>>
            {
                { "Jenis", p => p.Jenis },
                { "Poin", p => p.Poin.ToString() },
                { "Tanggal", p => Formatter.FormatTanggal(p.Tanggal) },
                { "Status", p => p.Status.ToString() }
            };

            foreach (var p in siswa.RiwayatPelanggaran)
            {
                FormBuilder.TampilkanForm(fieldGenerators, p);
                Console.WriteLine();
            }
        }

        static void UbahStatusPelanggaran(SiswaService siswaService, PelanggaranService pelanggaranService)
        {
            Console.Write("Masukkan ID siswa: ");
            int id = int.Parse(Console.ReadLine());
            var siswa = siswaService.CariSiswa(id);

            if (siswa == null)
            {
                Console.WriteLine("Siswa tidak ditemukan.");
                return;
            }

            Console.WriteLine("\n=== PILIH PELANGGARAN ===");
            for (int i = 0; i < siswa.RiwayatPelanggaran.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {siswa.RiwayatPelanggaran[i].Jenis} - {siswa.RiwayatPelanggaran[i].Status}");
            }

            Console.Write("Pilih pelanggaran (nomor): ");
            int index = int.Parse(Console.ReadLine()) - 1;

            if (index < 0 || index >= siswa.RiwayatPelanggaran.Count)
            {
                Console.WriteLine("Pilihan tidak valid.");
                return;
            }

            Console.WriteLine("\n=== PILIH STATUS BARU ===");
            Console.WriteLine("1. DISETUJUI");
            Console.WriteLine("2. DIBERI SANKSI");
            Console.WriteLine("3. SELESAI");

            Console.Write("Pilih status (1-3): ");
            string pilihanStatus = Console.ReadLine();
            Trigger trigger = pilihanStatus switch
            {
                "1" => Trigger.SETUJUI,
                "2" => Trigger.BERI_SANKSI, 
                "3" => Trigger.SELESAIKAN,
                _ => throw new ArgumentException("Pilihan tidak valid.")
            };

            bool statusBerubah = pelanggaranService.UbahStatusPelanggaran(siswa.RiwayatPelanggaran[index], trigger);
            if (statusBerubah)
            {
                Console.WriteLine($"Status pelanggaran '{siswa.RiwayatPelanggaran[index].Jenis}' berubah menjadi {siswa.RiwayatPelanggaran[index].Status}.");
            }
        }

    }
}

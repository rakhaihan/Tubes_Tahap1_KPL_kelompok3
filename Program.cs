using System;
using System.Collections.Generic;
using PoinSiswa.Library.Model;
using PoinSiswa.Library.Service;
using PoinSiswa.Library.Configuration;
using PoinSiswa.Library.TableDriven;
using PoinSiswa.Library.Automata;
using PoinSiswa.Library.Components;

namespace PoinSiswa.App
{
    class Program
    {
        static SiswaService siswaService = new SiswaService();
        static PelanggaranService pelanggaranService = new PelanggaranService();
        static ConfigManager configManager = new ConfigManager();

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== SISTEM POIN SISWA ===");
                Console.WriteLine("1. Tambah Siswa");
                Console.WriteLine("2. Lihat Semua Siswa");
                Console.WriteLine("3. Tambah Pelanggaran");
                Console.WriteLine("4. Lihat Riwayat Pelanggaran");
                Console.WriteLine("5. Keluar");
                Console.Write("Pilih menu: ");
                string pilihan = Console.ReadLine();

                switch (pilihan)
                {
                    case "1":
                        TambahSiswa();
                        break;
                    case "2":
                        LihatSemuaSiswa();
                        break;
                    case "3":
                        TambahPelanggaran();
                        break;
                    case "4":
                        LihatPelanggaran();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Pilihan tidak valid.");
                        break;
                }

                Console.WriteLine("\nTekan Enter untuk kembali ke menu...");
                Console.ReadLine();
            }
        }

        static void TambahSiswa()
        {
            Console.Write("Nama: ");
            string nama = Console.ReadLine();
            Console.Write("Kelas: ");
            string kelas = Console.ReadLine();
            Siswa siswa = new Siswa { Id = Guid.NewGuid().GetHashCode(), Nama = nama, Kelas = kelas };
            siswaService.TambahSiswa(siswa);
            Console.WriteLine("Siswa berhasil ditambahkan.");
        }

        static void LihatSemuaSiswa()
        {
            var semuaSiswa = siswaService.GetType()
                .GetField("_daftarSiswa", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(siswaService) as List<Siswa>;

            if (semuaSiswa == null || semuaSiswa.Count == 0)
            {
                Console.WriteLine("Belum ada data siswa.");
                return;
            }

            TableRenderer.Render(semuaSiswa, new Dictionary<string, Func<Siswa, string>>
            {
                { "ID", s => s.Id.ToString() },
                { "Nama", s => s.Nama },
                { "Kelas", s => s.Kelas },
                { "Total Poin", s => s.TotalPoin.ToString() }
            });
        }

        static void TambahPelanggaran()
        {
            Console.Write("ID Siswa: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID tidak valid.");
                return;
            }

            Siswa siswa = siswaService.CariSiswa(id);
            if (siswa == null)
            {
                Console.WriteLine("Siswa tidak ditemukan.");
                return;
            }

            Console.Write("Jenis Pelanggaran (Misal: Merokok, Terlambat Masuk): ");
            string jenis = Console.ReadLine();

            try
            {
                int poin = TabelPelanggaran.GetPoin(jenis);
                var pelanggaran = new Pelanggaran
                {
                    Id = Guid.NewGuid().GetHashCode(),
                    SiswaId = siswa.Id,
                    Jenis = jenis,
                    Poin = poin,
                    Tanggal = DateTime.Now,
                    Status = StatusPelanggaran.DILAPORKAN
                };

                pelanggaranService.TambahPelanggaran(siswa, pelanggaran);
                Console.WriteLine("Pelanggaran berhasil ditambahkan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void LihatPelanggaran()
        {
            Console.Write("ID Siswa: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID tidak valid.");
                return;
            }

            Siswa siswa = siswaService.CariSiswa(id);
            if (siswa == null)
            {
                Console.WriteLine("Siswa tidak ditemukan.");
                return;
            }

            var riwayat = siswa.RiwayatPelanggaran;
            if (riwayat.Count == 0)
            {
                Console.WriteLine("Tidak ada riwayat pelanggaran.");
                return;
            }

            TableRenderer.Render(riwayat, new Dictionary<string, Func<Pelanggaran, string>>
            {
                { "Tanggal", p => p.Tanggal.ToShortDateString() },
                { "Jenis", p => p.Jenis },
                { "Poin", p => p.Poin.ToString() },
                { "Status", p => p.Status.ToString() }
            });
        }
    }
}

using System;
using Tubes_Tahap1_KPL_kelompok3;
using System.Collections.Generic;
namespace Tubes_Tahap1_KPL_kelompok3
{
    class Program
    {
        static void Main(string[] args)
        {
            SiswaService siswaService = new SiswaService();
            PelanggaranService pelanggaranService = new PelanggaranService();
            NotifikasiService notifikasiService = new NotifikasiService();
            bool jalan = true;
            while (jalan)
            {
                Console.WriteLine("\n=== Sistem Pelanggaran Siswa ===");
                Console.WriteLine("1. Tambah Siswa");
                Console.WriteLine("2. Tambah Pelanggaran");
                Console.WriteLine("3. Lihat Semua Pelanggaran");
                Console.WriteLine("4. Ubah Status Pelanggaran");
                Console.WriteLine("5. Keluar");
                Console.Write("Pilih menu: ");
                string pilihan = Console.ReadLine();
                switch (pilihan)
                {
                    case "1":
                        Console.Write("Masukkan Nama Siswa: ");
                        string nama = Console.ReadLine();
                        Console.Write("Masukkan Kelas: ");
                        string kelas = Console.ReadLine();
                        Console.Write("Masukkan Nomor Induk (NIS): ");
                        string nis = Console.ReadLine();

                        siswaService.TambahSiswa(nama, kelas, nis);
                        Console.WriteLine("Siswa berhasil ditambahkan!");
                        break;

                    case "2":
                        Console.Write("Masukkan Nama Siswa: ");
                        string namaSiswaPelanggaran = Console.ReadLine();
                        Console.Write("Masukkan Jenis Pelanggaran: ");
                        string jenisPelanggaran = Console.ReadLine();
                        Console.Write("Masukkan Poin Pelanggaran: ");
                        int poin = int.Parse(Console.ReadLine());

                        pelanggaranService.TambahPelanggaran(namaSiswaPelanggaran, jenisPelanggaran, poin);
                        Console.WriteLine("Pelanggaran berhasil ditambahkan!");

                        notifikasiService.KirimNotifikasi(namaSiswaPelanggaran, $"Anda melakukan pelanggaran: {jenisPelanggaran} (+{poin} poin)");
                        notifikasiService.KirimNotifikasiOrangTua(namaSiswaPelanggaran, $"Anak anda melakukan pelanggaran: {jenisPelanggaran} (+{poin} poin)");
                        break;

                    case "3":
                        List<Pelanggaran> semuaPelanggaran = pelanggaranService.GetSemuaPelanggaran();
                        Console.WriteLine("\n--- Daftar Pelanggaran ---");
                        if (semuaPelanggaran.Count == 0)
                        {
                            Console.WriteLine("Belum ada data pelanggaran.");
                        }
                        else
                        {
                            int index = 0;
                            foreach (var pel in semuaPelanggaran)
                            {
                                Console.WriteLine($"{index++}. {pel.NamaSiswa} | {pel.JenisPelanggaran} | {pel.Poin} poin | {pel.Tanggal.ToShortDateString()} | Status: {pel.Status}");
                            }
                        }
                        break;

                    case "4":
                        Console.Write("Masukkan Index Pelanggaran yang ingin diubah: ");
                        int idx = int.Parse(Console.ReadLine());
                        Console.Write("Masukkan Status Baru (Belum Ditindak / Sudah Ditindak): ");
                        string statusBaru = Console.ReadLine();

                        pelanggaranService.UbahStatusPelanggaran(idx, statusBaru);
                        Console.WriteLine("Status pelanggaran berhasil diubah!");
                        break;

                    case "5":
                        jalan = false;
                        Console.WriteLine("Terima kasih sudah menggunakan sistem ini.");
                        break;

                    default:
                        Console.WriteLine("❗ Pilihan tidak valid. Silakan coba lagi.");
                        break;
                }
            }
        }
    }
}

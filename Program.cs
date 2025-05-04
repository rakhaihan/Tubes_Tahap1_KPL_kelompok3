using System;
using System.Collections.Generic;
using Tubes_Tahap1_KPL_kelompok3;
using Tubes_Tahap1_KPL_kelompok3.Components;
using PoinSiswa.Library.Configuration;

class Program
{
    enum State
    {
        MenuUtama,
        TambahSiswa,
        TambahPelanggaran,
        LihatDataSiswa,
        LihatDataPelanggaran,
        UbahStatusPelanggaran,
        Keluar
    }

    static void Main()
    {
        // Inisialisasi service
        SiswaService siswaService = new SiswaService();
        PelanggaranService pelanggaranService = new PelanggaranService();
        NotifikasiService notifikasiService = new NotifikasiService();
        ConfigManager configManager = new ConfigManager();

        // Automata untuk kontrol alur aplikasi
        State state = State.MenuUtama;

        // Tabel kolom siswa untuk Table-driven rendering
        var siswaColumns = new Dictionary<string, Func<Siswa, string>>
        {
            { "Nama", s => s.Nama },
            { "Kelas", s => s.Kelas },
            { "NIS", s => s.NomorInduk }
        };

        // Tabel kolom pelanggaran
        var pelanggaranColumns = new Dictionary<string, Func<Pelanggaran, string>>
        {
            { "Nama Siswa", p => p.NamaSiswa },
            { "Jenis Pelanggaran", p => p.JenisPelanggaran },
            { "Poin", p => p.Poin.ToString() },
            { "Tanggal", p => p.Tanggal.ToShortDateString() },
            { "Status", p => p.Status }
        };

        while (state != State.Keluar)
        {
            switch (state)
            {
                case State.MenuUtama:
                    Console.WriteLine("\n===== Sistem Poin Pelanggaran Siswa =====");
                    Console.WriteLine("1. Tambah Siswa");
                    Console.WriteLine("2. Tambah Pelanggaran");
                    Console.WriteLine("3. Lihat Data Siswa");
                    Console.WriteLine("4. Lihat Data Pelanggaran");
                    Console.WriteLine("5. Ubah Status Pelanggaran");
                    Console.WriteLine("0. Keluar");
                    Console.Write("Pilih menu: ");
                    string inputMenu = Console.ReadLine();

                    switch (inputMenu)
                    {
                        case "1": state = State.TambahSiswa; break;
                        case "2": state = State.TambahPelanggaran; break;
                        case "3": state = State.LihatDataSiswa; break;
                        case "4": state = State.LihatDataPelanggaran; break;
                        case "5": state = State.UbahStatusPelanggaran; break;
                        case "0": state = State.Keluar; break;
                        default:
                            Console.WriteLine("Input tidak valid.");
                            break;
                    }
                    break;

                case State.TambahSiswa:
                    Console.Write("Nama: ");
                    string nama = Console.ReadLine();
                    Console.Write("Kelas: ");
                    string kelas = Console.ReadLine();
                    Console.Write("NIS: ");
                    string nis = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(kelas) || string.IsNullOrWhiteSpace(nis))
                    {
                        Console.WriteLine("Semua data wajib diisi!");
                    }
                    else
                    {
                        siswaService.TambahSiswa(nama, kelas, nis);
                        Console.WriteLine("Siswa berhasil ditambahkan.");
                    }

                    state = State.MenuUtama;
                    break;

                case State.TambahPelanggaran:
                    Console.Write("Nama Siswa: ");
                    string namaSiswa = Console.ReadLine();
                    Console.Write("Jenis Pelanggaran: ");
                    string jenis = Console.ReadLine();
                    Console.Write("Poin: ");
                    if (!int.TryParse(Console.ReadLine(), out int poin))
                    {
                        Console.WriteLine("Poin harus berupa angka.");
                        state = State.MenuUtama;
                        break;
                    }

                    pelanggaranService.TambahPelanggaran(namaSiswa, jenis, poin);
                    Console.WriteLine("Pelanggaran ditambahkan.");

                    // Kirim notifikasi berdasarkan batas poin
                    if (poin >= configManager.Config.BatasPoinPanggilanOrangTua)
                    {
                        notifikasiService.KirimNotifikasiOrangTua(namaSiswa, $"Pelanggaran berat dengan {poin} poin.");
                    }
                    else if (poin >= configManager.Config.BatasPoinPeringatan)
                    {
                        notifikasiService.KirimNotifikasi(namaSiswa, $"Peringatan: Anda mendapat pelanggaran {jenis} ({poin} poin).");
                    }

                    state = State.MenuUtama;
                    break;

                case State.LihatDataSiswa:
                    Console.WriteLine("\nDaftar Siswa:");
                    TableRenderer.Render(siswaService.GetSemuaSiswa(), siswaColumns);
                    state = State.MenuUtama;
                    break;

                case State.LihatDataPelanggaran:
                    Console.WriteLine("\nDaftar Pelanggaran:");
                    TableRenderer.Render(pelanggaranService.GetSemuaPelanggaran(), pelanggaranColumns);
                    state = State.MenuUtama;
                    break;

                case State.UbahStatusPelanggaran:
                    var semuaPelanggaran = pelanggaranService.GetSemuaPelanggaran();

                    if (semuaPelanggaran.Count == 0)
                    {
                        Console.WriteLine("Belum ada pelanggaran.");
                        state = State.MenuUtama;
                        break;
                    }

                    TableRenderer.Render(semuaPelanggaran, pelanggaranColumns);
                    Console.Write("Masukkan index pelanggaran yang ingin diubah: ");
                    if (!int.TryParse(Console.ReadLine(), out int index) || index < 0 || index >= semuaPelanggaran.Count)
                    {
                        Console.WriteLine("Index tidak valid.");
                        state = State.MenuUtama;
                        break;
                    }

                    Console.Write("Masukkan status baru (Belum Ditindak / Sudah Ditindak): ");
                    string statusBaru = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(statusBaru))
                    {
                        Console.WriteLine("Status tidak boleh kosong.");
                    }
                    else
                    {
                        pelanggaranService.UbahStatusPelanggaran(index, statusBaru);
                        Console.WriteLine("Status pelanggaran berhasil diubah.");
                    }

                    state = State.MenuUtama;
                    break;
            }
        }

        Console.WriteLine("\nTerima kasih telah menggunakan sistem ini.");
    }
}

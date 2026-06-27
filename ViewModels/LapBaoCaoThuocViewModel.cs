using System;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories;

namespace QuanLyPhongKham.ViewModels
{
    public partial class LapBaoCaoThuocViewModel : ObservableObject
    {
        private readonly BaoCaoRepository _baoCaoRepo = new();

        [ObservableProperty] private ObservableCollection<int> danhSachNam = new();
        [ObservableProperty] private int namDuocChon;
        [ObservableProperty] private ObservableCollection<int> danhSachThang = new();
        [ObservableProperty] private int thangDuocChon;
        [ObservableProperty] private ObservableCollection<BaoCaoThuocItem> danhSachKetQua = new();

        public LapBaoCaoThuocViewModel()
        {
            // Năm: vài năm quanh năm hiện hành
            int namHienTai = DateTime.Now.Year;
            for (int n = namHienTai - 2; n <= namHienTai + 1; n++)
                DanhSachNam.Add(n);
            NamDuocChon = namHienTai;

            // Tháng 1..12
            for (int t = 1; t <= 12; t++)
                DanhSachThang.Add(t);
            ThangDuocChon = DateTime.Now.Month;
        }

        [RelayCommand]
        private void LapBaoCao()
        {
            var kq = _baoCaoRepo.LapBaoCaoThuoc(NamDuocChon, ThangDuocChon);
            DanhSachKetQua = new ObservableCollection<BaoCaoThuocItem>(kq);

            if (kq.Count == 0)
                MessageBox.Show("Không có dữ liệu sử dụng thuốc cho tháng/năm đã chọn.",
                    "Lập báo cáo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void Thoat()
        {
            foreach (Window w in Application.Current.Windows)
                if (w.DataContext == this) { w.Close(); break; }
        }
    }
}

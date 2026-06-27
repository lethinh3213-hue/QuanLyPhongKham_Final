using System;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories;

namespace QuanLyPhongKham.ViewModels
{
    public partial class LapBaoCaoHoaDonViewModel : ObservableObject
    {
        private readonly LoaiPhongKhamRepository _phongKhamRepo = new();
        private readonly BaoCaoRepository _baoCaoRepo = new();

        [ObservableProperty] private ObservableCollection<int> danhSachNam = new();
        [ObservableProperty] private int namDuocChon;
        [ObservableProperty] private ObservableCollection<int> danhSachThang = new();
        [ObservableProperty] private int thangDuocChon;
        [ObservableProperty] private ObservableCollection<LoaiPhongKham> danhSachLoaiPhongKham = new();
        [ObservableProperty] private LoaiPhongKham? loaiPhongKhamDuocChon;
        [ObservableProperty] private ObservableCollection<BaoCaoHoaDonItem> danhSachKetQua = new();

        public LapBaoCaoHoaDonViewModel()
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

            // Loại phòng khám
            DanhSachLoaiPhongKham = new ObservableCollection<LoaiPhongKham>(_phongKhamRepo.GetAll());
            if (DanhSachLoaiPhongKham.Count > 0)
                LoaiPhongKhamDuocChon = DanhSachLoaiPhongKham[0];
        }

        [RelayCommand]
        private void LapBaoCao()
        {
            if (LoaiPhongKhamDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn loại phòng khám.",
                    "Lập báo cáo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var kq = _baoCaoRepo.LapBaoCaoHoaDon(NamDuocChon, ThangDuocChon,
                LoaiPhongKhamDuocChon.MaLoaiPhongKham);
            DanhSachKetQua = new ObservableCollection<BaoCaoHoaDonItem>(kq);

            if (kq.Count == 0)
                MessageBox.Show("Không có dữ liệu khám bệnh cho tháng/năm và loại phòng khám đã chọn.",
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

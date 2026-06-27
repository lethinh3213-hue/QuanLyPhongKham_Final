using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories;
using QuanLyPhongKham.Views;

namespace QuanLyPhongKham.ViewModels
{
    public partial class LapHoaDonThanhToanViewModel : ObservableObject
    {
        //  REPOSITORIES 
        private readonly PhieuKhamBenhRepository _phieuRepo = new();
        private readonly HoaDonThanhToanRepository _hoaDonRepo = new();

        // Văn hóa VN để format tiền (dấu chấm ngăn cách hàng nghìn)
        private static readonly CultureInfo VN = new CultureInfo("vi-VN");

        //  DỮ LIỆU COMBOBOX 
        [ObservableProperty] private ObservableCollection<PhieuKhamBenhItem> danhSachPhieu = new();
        [ObservableProperty] private PhieuKhamBenhItem? phieuDuocChon;

        //  CÁC Ô HIỂN THỊ (read-only trên giao diện) 
        [ObservableProperty] private string hoTenBenhNhan = string.Empty;
        [ObservableProperty] private string ngayKhamStr = string.Empty;
        [ObservableProperty] private string tenLoaiPhongKham = string.Empty;
        [ObservableProperty] private string tienKhamStr = string.Empty;
        [ObservableProperty] private string tienThuocStr = string.Empty;
        [ObservableProperty] private string tongTienStr = string.Empty;

        //  CONSTRUCTOR 
        public LapHoaDonThanhToanViewModel()
        {
            NapDanhSachPhieu();
        }

        private void NapDanhSachPhieu()
        {
            DanhSachPhieu = new ObservableCollection<PhieuKhamBenhItem>(_phieuRepo.GetAll());
        }

        // Khi chọn 1 Mã phiếu khám bệnh -> tự điền thông tin mô tả, xóa các ô tiền cũ.
        partial void OnPhieuDuocChonChanged(PhieuKhamBenhItem? value)
        {
            if (value == null)
            {
                XoaTrang();
                return;
            }

            HoTenBenhNhan = value.HoTenBenhNhan;
            NgayKhamStr = value.NgayKham.ToString("dd/MM/yyyy");
            TenLoaiPhongKham = value.TenLoaiPhongKham;

            // Tiền chỉ hiện ra sau khi bấm "Lập hóa đơn"
            TienKhamStr = string.Empty;
            TienThuocStr = string.Empty;
            TongTienStr = string.Empty;
        }

        private void XoaTrang()
        {
            HoTenBenhNhan = string.Empty;
            NgayKhamStr = string.Empty;
            TenLoaiPhongKham = string.Empty;
            TienKhamStr = string.Empty;
            TienThuocStr = string.Empty;
            TongTienStr = string.Empty;
        }

        //  COMMAND: LẬP HÓA ĐƠN (nút xử lý nghiệp vụ) 
        [RelayCommand]
        private void LapHoaDon()
        {
            if (PhieuDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn Mã phiếu khám bệnh.",
                    "Lập hóa đơn thanh toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string maPhieu = PhieuDuocChon.MaPhieuKhamBenh;

            // Bước 01..10: tính hóa đơn
            var kq = _hoaDonRepo.TinhHoaDon(maPhieu);

            // Bước 03: mã phiếu không tồn tại trong CSDL
            if (kq == null)
            {
                MessageBox.Show("Mã phiếu khám bệnh không tồn tại trong hệ thống.",
                    "Lập hóa đơn thanh toán", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Đổ kết quả lên màn hình
            HoTenBenhNhan = kq.HoTenBenhNhan;
            NgayKhamStr = kq.NgayKham.ToString("dd/MM/yyyy");
            TenLoaiPhongKham = kq.TenLoaiPhongKham;
            TienKhamStr = DinhDangTien(kq.TienKham);
            TienThuocStr = DinhDangTien(kq.TienThuoc);
            TongTienStr = DinhDangTien(kq.TongTien);

            // Bước 11: lưu xuống CSDL Hóa đơn thanh toán
            var hd = new HoaDonThanhToan
            {
                MaPhieuKhamBenh = maPhieu,
                TienKham = kq.TienKham,
                TienThuoc = kq.TienThuoc,
                TongTien = kq.TongTien
            };
            string maHoaDon = _hoaDonRepo.LuuHoaDon(hd);

            if (!string.IsNullOrEmpty(maHoaDon))
            {
                string ghiChuThuoc = kq.CoDungThuoc ? "" : "\n(Phiếu này không kê thuốc nên Tiền thuốc = 0)";
                MessageBox.Show(
                    $"Lập hóa đơn thành công!\n\n" +
                    $"Mã hóa đơn: {maHoaDon}\n" +
                    $"Tiền khám: {TienKhamStr}\n" +
                    $"Tiền thuốc: {TienThuocStr}\n" +
                    $"Tổng tiền: {TongTienStr}{ghiChuThuoc}",
                    "Lập hóa đơn thanh toán", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //  COMMAND: TÌM PHIẾU KHÁM BỆNH (nút xử lý chất lượng) 
        [RelayCommand]
        private void TimPhieu()
        {
            var dialog = new TimPhieuKhamBenhDialog(DanhSachPhieu)
            {
                Owner = Application.Current.MainWindow
            };
            bool? ketQua = dialog.ShowDialog();

            if (ketQua == true && !string.IsNullOrEmpty(dialog.MaPhieuDuocChon))
            {
                var item = DanhSachPhieu.FirstOrDefault(
                    p => p.MaPhieuKhamBenh == dialog.MaPhieuDuocChon);
                if (item != null)
                    PhieuDuocChon = item;   // chọn vào ComboBox -> tự auto-fill
            }
        }

        //  COMMAND: THOÁT (nút xử lý hệ thống) 
        [RelayCommand]
        private void Thoat()
        {
            var result = MessageBox.Show("Bạn có chắc muốn thoát?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                foreach (Window w in Application.Current.Windows)
                    if (w.DataContext == this) { w.Close(); break; }
            }
        }

        // Định dạng tiền: 100000 -> "100.000 VNĐ"
        private string DinhDangTien(decimal tien) => tien.ToString("#,##0", VN) + " VNĐ";
    }
}

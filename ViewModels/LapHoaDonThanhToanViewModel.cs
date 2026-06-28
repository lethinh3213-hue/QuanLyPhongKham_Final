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

        // Lưu kết quả tính toán để dùng khi lập hóa đơn
        private KetQuaHoaDon? ketQuaTinh = null;

        //  CONSTRUCTOR 
        public LapHoaDonThanhToanViewModel()
        {
            NapDanhSachPhieu();
        }

        private void NapDanhSachPhieu()
        {
            DanhSachPhieu = new ObservableCollection<PhieuKhamBenhItem>(_phieuRepo.GetAll());
        }

        // Khi chọn 1 Mã phiếu khám bệnh → Tính tiền và hiển thị ngay
        partial void OnPhieuDuocChonChanged(PhieuKhamBenhItem? value)
        {
            if (value == null)
            {
                XoaTrang();
                ketQuaTinh = null;
                return;
            }

            HoTenBenhNhan = value.HoTenBenhNhan;
            NgayKhamStr = value.NgayKham.ToString("dd/MM/yyyy");
            TenLoaiPhongKham = value.TenLoaiPhongKham;

            // Tính hóa đơn ngay khi chọn phiếu
            string maPhieu = value.MaPhieuKhamBenh;
            ketQuaTinh = _hoaDonRepo.TinhHoaDon(maPhieu);

            if (ketQuaTinh != null)
            {
                // Hiển thị tiền trên form
                TienKhamStr = DinhDangTien(ketQuaTinh.TienKham);
                TienThuocStr = DinhDangTien(ketQuaTinh.TienThuoc);
                TongTienStr = DinhDangTien(ketQuaTinh.TongTien);
            }
            else
            {
                // Nếu lỗi thì xóa rỗng các trường tiền
                TienKhamStr = string.Empty;
                TienThuocStr = string.Empty;
                TongTienStr = string.Empty;
            }
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

        //  COMMAND: LẬP HÓA ĐƠN (nút lưu vào CSDL) 
        [RelayCommand]
        private void LapHoaDon()
        {
            if (PhieuDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn Mã phiếu khám bệnh.",
                    "Lập hóa đơn thanh toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ketQuaTinh == null)
            {
                MessageBox.Show("Vui lòng chọn phiếu khám bệnh hợp lệ.",
                    "Lập hóa đơn thanh toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lưu hóa đơn xuống CSDL
            var hd = new HoaDonThanhToan
            {
                MaPhieuKhamBenh = PhieuDuocChon.MaPhieuKhamBenh,
                TienKham = ketQuaTinh.TienKham,
                TienThuoc = ketQuaTinh.TienThuoc,
                TongTien = ketQuaTinh.TongTien
            };
            string maHoaDon = _hoaDonRepo.LuuHoaDon(hd);

            if (!string.IsNullOrEmpty(maHoaDon))
            {
                string ghiChuThuoc = ketQuaTinh.CoDungThuoc ? "" : "\n(Phiếu này không kê thuốc nên Tiền thuốc = 0)";
                MessageBox.Show(
                    $"Lập hóa đơn thành công!\n\n" +
                    $"Mã hóa đơn: {maHoaDon}\n" +
                    $"Tiền khám: {TienKhamStr}\n" +
                    $"Tiền thuốc: {TienThuocStr}\n" +
                    $"Tổng tiền: {TongTienStr}{ghiChuThuoc}",
                    "Lập hóa đơn thanh toán", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Lỗi lưu hóa đơn. Vui lòng thử lại.",
                    "Lập hóa đơn thanh toán", MessageBoxButton.OK, MessageBoxImage.Error);
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

using System;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories;

namespace QuanLyPhongKham.ViewModels
{
    public partial class ThayDoiQuyDinhViewModel : ObservableObject
    {
        private readonly LoaiPhongKhamRepository _lpkRepo = new();
        private readonly LoaiBenhRepository _lbRepo = new();
        private readonly DonViTinhRepository _dvtRepo = new();
        private readonly CachDungRepository _cdRepo = new();
        private readonly LoaiThuocRepository _ltRepo = new();

        [ObservableProperty] private ObservableCollection<LoaiPhongKham> danhSachPhongKham = new();
        [ObservableProperty] private LoaiPhongKham? phongKhamDuocChon;
        [ObservableProperty] private string tenPhongKhamNhap = string.Empty;
        [ObservableProperty] private string soLuongToiDaNhap = string.Empty;
        [ObservableProperty] private string tienKhamNhap = string.Empty;

        [ObservableProperty] private ObservableCollection<LoaiBenh> danhSachLoaiBenh = new();
        [ObservableProperty] private LoaiBenh? loaiBenhDuocChon;
        [ObservableProperty] private string tenLoaiBenhNhap = string.Empty;

        [ObservableProperty] private ObservableCollection<DonViTinh> danhSachDonViTinh = new();
        [ObservableProperty] private DonViTinh? donViTinhDuocChon;
        [ObservableProperty] private string tenDonViTinhNhap = string.Empty;

        [ObservableProperty] private ObservableCollection<CachDung> danhSachCachDung = new();
        [ObservableProperty] private CachDung? cachDungDuocChon;
        [ObservableProperty] private string tenCachDungNhap = string.Empty;

        [ObservableProperty] private ObservableCollection<LoaiThuoc> danhSachThuoc = new();
        [ObservableProperty] private LoaiThuoc? thuocDuocChon;
        [ObservableProperty] private string tenThuocNhap = string.Empty;
        [ObservableProperty] private DonViTinh? donViTinhThuoc;
        [ObservableProperty] private CachDung? cachDungThuoc;
        [ObservableProperty] private string soLuongTonNhap = string.Empty;
        [ObservableProperty] private string donGiaNhap = string.Empty;

        public ThayDoiQuyDinhViewModel()
        {
            NapPhongKham();
            NapLoaiBenh();
            NapDonViTinh();
            NapCachDung();
            NapThuoc();
        }

        private void NapPhongKham() => DanhSachPhongKham = new ObservableCollection<LoaiPhongKham>(_lpkRepo.GetAll());
        private void NapLoaiBenh() => DanhSachLoaiBenh = new ObservableCollection<LoaiBenh>(_lbRepo.GetAll());
        private void NapDonViTinh() => DanhSachDonViTinh = new ObservableCollection<DonViTinh>(_dvtRepo.GetAll());
        private void NapCachDung() => DanhSachCachDung = new ObservableCollection<CachDung>(_cdRepo.GetAll());
        private void NapThuoc() => DanhSachThuoc = new ObservableCollection<LoaiThuoc>(_ltRepo.GetAll());

        partial void OnLoaiBenhDuocChonChanged(LoaiBenh? value)
        {
            if (value != null) TenLoaiBenhNhap = value.TenLoaiBenh;
        }
        partial void OnDonViTinhDuocChonChanged(DonViTinh? value)
        {
            if (value != null) TenDonViTinhNhap = value.TenDonViTinh;
        }
        partial void OnCachDungDuocChonChanged(CachDung? value)
        {
            if (value != null) TenCachDungNhap = value.TenCachDung;
        }
        partial void OnPhongKhamDuocChonChanged(LoaiPhongKham? value)
        {
            if (value == null)
            {
                TenPhongKhamNhap = string.Empty;
                SoLuongToiDaNhap = string.Empty;
                TienKhamNhap = string.Empty;
                return;
            }

            TenPhongKhamNhap = value.TenLoaiPhongKham;
            SoLuongToiDaNhap = value.SoLuongToiDa.ToString();
            TienKhamNhap = ((long)value.TienKham).ToString();
        }
        partial void OnThuocDuocChonChanged(LoaiThuoc? value)
        {
            if (value == null) return;
            TenThuocNhap = value.TenLoaiThuoc;
            SoLuongTonNhap = value.SoLuongTon.ToString();
            DonGiaNhap = ((long)value.DonGia).ToString();
            DonViTinhThuoc = TimDonViTinh(value.MaDonViTinh);
            CachDungThuoc = TimCachDung(value.MaCachDung);
        }

        private DonViTinh? TimDonViTinh(string ma)
        {
            foreach (var x in DanhSachDonViTinh)
                if (x.MaDonViTinh == ma) return x;
            return null;
        }
        private CachDung? TimCachDung(string ma)
        {
            foreach (var x in DanhSachCachDung)
                if (x.MaCachDung == ma) return x;
            return null;
        }

        [RelayCommand]
        private void ThemPhongKham()
        {
            if (!KiemTraPhongKham(out int sl, out decimal tien)) return;

            var pk = new LoaiPhongKham
            {
                TenLoaiPhongKham = TenPhongKhamNhap,
                SoLuongToiDa = sl,
                TienKham = tien
            };

            if (_lpkRepo.Them(pk))
            {
                NapPhongKham();
                XoaFormPhongKham();
                MessageBox.Show("Đã thêm loại phòng khám.", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private void SuaPhongKham()
        {
            if (PhongKhamDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!KiemTraPhongKham(out int sl, out decimal tien)) return;

            var pk = new LoaiPhongKham
            {
                MaLoaiPhongKham = PhongKhamDuocChon.MaLoaiPhongKham,
                TenLoaiPhongKham = TenPhongKhamNhap,
                SoLuongToiDa = sl,
                TienKham = tien
            };

            if (_lpkRepo.CapNhat(pk))
            {
                NapPhongKham();
                MessageBox.Show("Đã sửa loại phòng khám.", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private void XoaPhongKham()
        {
            if (PhongKhamDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_lpkRepo.Xoa(PhongKhamDuocChon.MaLoaiPhongKham))
            {
                NapPhongKham();
                XoaFormPhongKham();
                MessageBox.Show("Đã xóa loại phòng khám.", "Thành công",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool KiemTraPhongKham(out int sl, out decimal tien)
        {
            sl = 0;
            tien = 0;

            if (string.IsNullOrWhiteSpace(TenPhongKhamNhap))
            {
                MessageBox.Show("Vui lòng nhập tên phòng khám.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(SoLuongToiDaNhap, out sl) || sl < 1)
            {
                MessageBox.Show("Số lượng tối đa phải lớn hơn 0.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(TienKhamNhap, out tien) || tien < 0)
            {
                MessageBox.Show("Tiền khám không hợp lệ.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void XoaFormPhongKham()
        {
            PhongKhamDuocChon = null;
            TenPhongKhamNhap = string.Empty;
            SoLuongToiDaNhap = string.Empty;
            TienKhamNhap = string.Empty;
        }

        [RelayCommand]
        private void ThemLoaiBenh()
        {
            if (string.IsNullOrWhiteSpace(TenLoaiBenhNhap))
            {
                MessageBox.Show("Vui lòng nhập tên loại bệnh.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_lbRepo.Them(new LoaiBenh { TenLoaiBenh = TenLoaiBenhNhap }))
            {
                NapLoaiBenh();
                TenLoaiBenhNhap = string.Empty;
            }
        }

        [RelayCommand]
        private void SuaLoaiBenh()
        {
            if (LoaiBenhDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(TenLoaiBenhNhap))
            {
                MessageBox.Show("Tên loại bệnh không được trống.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_lbRepo.CapNhat(new LoaiBenh { MaLoaiBenh = LoaiBenhDuocChon.MaLoaiBenh, TenLoaiBenh = TenLoaiBenhNhap }))
                NapLoaiBenh();
        }

        [RelayCommand]
        private void XoaLoaiBenh()
        {
            if (LoaiBenhDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_lbRepo.Xoa(LoaiBenhDuocChon.MaLoaiBenh))
            {
                NapLoaiBenh();
                TenLoaiBenhNhap = string.Empty;
            }
        }

        [RelayCommand]
        private void ThemDonViTinh()
        {
            if (string.IsNullOrWhiteSpace(TenDonViTinhNhap))
            {
                MessageBox.Show("Vui lòng nhập tên đơn vị tính.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_dvtRepo.Them(new DonViTinh { TenDonViTinh = TenDonViTinhNhap }))
            {
                NapDonViTinh();
                TenDonViTinhNhap = string.Empty;
            }
        }

        [RelayCommand]
        private void SuaDonViTinh()
        {
            if (DonViTinhDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(TenDonViTinhNhap))
            {
                MessageBox.Show("Tên đơn vị tính không được trống.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_dvtRepo.CapNhat(new DonViTinh { MaDonViTinh = DonViTinhDuocChon.MaDonViTinh, TenDonViTinh = TenDonViTinhNhap }))
                NapDonViTinh();
        }

        [RelayCommand]
        private void XoaDonViTinh()
        {
            if (DonViTinhDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_dvtRepo.Xoa(DonViTinhDuocChon.MaDonViTinh))
            {
                NapDonViTinh();
                TenDonViTinhNhap = string.Empty;
            }
        }

        [RelayCommand]
        private void ThemCachDung()
        {
            if (string.IsNullOrWhiteSpace(TenCachDungNhap))
            {
                MessageBox.Show("Vui lòng nhập tên cách dùng.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_cdRepo.Them(new CachDung { TenCachDung = TenCachDungNhap }))
            {
                NapCachDung();
                TenCachDungNhap = string.Empty;
            }
        }

        [RelayCommand]
        private void SuaCachDung()
        {
            if (CachDungDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(TenCachDungNhap))
            {
                MessageBox.Show("Tên cách dùng không được trống.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_cdRepo.CapNhat(new CachDung { MaCachDung = CachDungDuocChon.MaCachDung, TenCachDung = TenCachDungNhap }))
                NapCachDung();
        }

        [RelayCommand]
        private void XoaCachDung()
        {
            if (CachDungDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_cdRepo.Xoa(CachDungDuocChon.MaCachDung))
            {
                NapCachDung();
                TenCachDungNhap = string.Empty;
            }
        }

        [RelayCommand]
        private void ThemThuoc()
        {
            if (!KiemTraThuoc(out int ton, out decimal gia)) return;
            var t = new LoaiThuoc
            {
                TenLoaiThuoc = TenThuocNhap,
                MaDonViTinh = DonViTinhThuoc!.MaDonViTinh,
                MaCachDung = CachDungThuoc!.MaCachDung,
                SoLuongTon = ton,
                DonGia = gia
            };
            if (_ltRepo.Them(t))
            {
                NapThuoc();
                XoaFormThuoc();
            }
        }

        [RelayCommand]
        private void SuaThuoc()
        {
            if (ThuocDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn thuốc cần sửa.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!KiemTraThuoc(out int ton, out decimal gia)) return;
            var t = new LoaiThuoc
            {
                MaLoaiThuoc = ThuocDuocChon.MaLoaiThuoc,
                TenLoaiThuoc = TenThuocNhap,
                MaDonViTinh = DonViTinhThuoc!.MaDonViTinh,
                MaCachDung = CachDungThuoc!.MaCachDung,
                SoLuongTon = ton,
                DonGia = gia
            };
            if (_ltRepo.CapNhat(t))
                NapThuoc();
        }

        [RelayCommand]
        private void XoaThuoc()
        {
            if (ThuocDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn thuốc cần xóa.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_ltRepo.Xoa(ThuocDuocChon.MaLoaiThuoc))
            {
                NapThuoc();
                XoaFormThuoc();
            }
        }

        private bool KiemTraThuoc(out int ton, out decimal gia)
        {
            ton = 0;
            gia = 0;
            if (string.IsNullOrWhiteSpace(TenThuocNhap))
            {
                MessageBox.Show("Vui lòng nhập tên thuốc.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (DonViTinhThuoc == null || CachDungThuoc == null)
            {
                MessageBox.Show("Vui lòng chọn đơn vị tính và cách dùng.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(SoLuongTonNhap, out ton) || ton < 0)
            {
                MessageBox.Show("Số lượng tồn không hợp lệ.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!decimal.TryParse(DonGiaNhap, out gia) || gia < 0)
            {
                MessageBox.Show("Đơn giá không hợp lệ.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void XoaFormThuoc()
        {
            TenThuocNhap = string.Empty;
            SoLuongTonNhap = string.Empty;
            DonGiaNhap = string.Empty;
            DonViTinhThuoc = null;
            CachDungThuoc = null;
        }

        [RelayCommand]
        private void Thoat()
        {
            foreach (Window w in Application.Current.Windows)
                if (w.DataContext == this) { w.Close(); break; }
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories;

namespace QuanLyPhongKham.ViewModels
{
    public partial class TraCuuBenhNhanViewModel : ObservableObject
    {
        //  REPOSITORIES 
        private readonly LoaiPhongKhamRepository _loaiPhongKhamRepo = new();
        private readonly LoaiBenhRepository _loaiBenhRepo = new();
        private readonly LoaiThuocRepository _loaiThuocRepo = new();
        private readonly DonViTinhRepository _donViTinhRepo = new();
        private readonly CachDungRepository _cachDungRepo = new();
        private readonly BenhNhanRepository _benhNhanRepo = new();

        //  DROPDOWN DATA 
        [ObservableProperty] private ObservableCollection<LoaiPhongKham> danhSachLoaiPhongKham = new();
        [ObservableProperty] private ObservableCollection<LoaiBenh> danhSachLoaiBenh = new();
        [ObservableProperty] private ObservableCollection<LoaiThuoc> danhSachLoaiThuoc = new();
        [ObservableProperty] private ObservableCollection<DonViTinh> danhSachDonViTinh = new();
        [ObservableProperty] private ObservableCollection<CachDung> danhSachCachDung = new();

        //  FILTER: text "có chứa" 
        [ObservableProperty] private string maBenhNhanFilter = string.Empty;
        [ObservableProperty] private string hoTenFilter = string.Empty;
        [ObservableProperty] private string trieuChungFilter = string.Empty;

        //  FILTER: combobox "tất cả" 
        [ObservableProperty] private LoaiPhongKham? loaiPhongKhamDuocChon;
        [ObservableProperty] private LoaiBenh? loaiBenhDuocChon;
        [ObservableProperty] private LoaiThuoc? loaiThuocDuocChon;
        [ObservableProperty] private DonViTinh? donViTinhDuocChon;
        [ObservableProperty] private CachDung? cachDungDuocChon;
        [ObservableProperty] private string gioiTinhDuocChon = "Tất cả";

        //  FILTER: khoảng từ-đến   
        [ObservableProperty] private DateTime? ngayKhamTu;
        [ObservableProperty] private DateTime? ngayKhamDen;
        [ObservableProperty] private string namSinhTuStr = string.Empty;
        [ObservableProperty] private string namSinhDenStr = string.Empty;
        [ObservableProperty] private string soLuongKeTuStr = string.Empty;
        [ObservableProperty] private string soLuongKeDenStr = string.Empty;

        //  KẾT QUẢ 
        [ObservableProperty] private ObservableCollection<KetQuaTraCuuItem> danhSachKetQua = new();

        //  CONSTRUCTOR 
        public TraCuuBenhNhanViewModel()
        {
            NapCacDanhSach();
        }

        // Nạp các danh sách khi form được tải
        private void NapCacDanhSach()
        {
            DanhSachLoaiPhongKham = new ObservableCollection<LoaiPhongKham>(_loaiPhongKhamRepo.GetAll());
            DanhSachLoaiBenh = new ObservableCollection<LoaiBenh>(_loaiBenhRepo.GetAll());
            DanhSachLoaiThuoc = new ObservableCollection<LoaiThuoc>(_loaiThuocRepo.GetAll());
            DanhSachDonViTinh = new ObservableCollection<DonViTinh>(_donViTinhRepo.GetAll());
            DanhSachCachDung = new ObservableCollection<CachDung>(_cachDungRepo.GetAll());
        }

        // Khi người dùng chọn Loại thuốc → tự động điền Đơn vị thuốc & Cách dùng
        partial void OnLoaiThuocDuocChonChanged(LoaiThuoc? value)
        {
            if (value == null) return;

            DonViTinhDuocChon = DanhSachDonViTinh
                .FirstOrDefault(dv => dv.MaDonViTinh == value.MaDonViTinh);

            CachDungDuocChon = DanhSachCachDung
                .FirstOrDefault(cd => cd.MaCachDung == value.MaCachDung);
        }

        //  COMMAND: TRA CỨU
        [RelayCommand]
        private void TraCuu()
        {
            var tieuChi = BuildTieuChi();
            var ketQua = _benhNhanRepo.TraCuu(tieuChi);

            // Đổ kết quả vào DataGrid (kèm STT)
            DanhSachKetQua.Clear();
            int stt = 1;
            foreach (var kq in ketQua)
            {
                DanhSachKetQua.Add(new KetQuaTraCuuItem
                {
                    STT = stt++,
                    HoTen = kq.HoTen,
                    NgayKham = kq.NgayKham,
                    NamSinh = kq.NamSinh,
                    TenLoaiPhongKham = kq.TenLoaiPhongKham,
                    TenLoaiBenh = kq.TenLoaiBenh,
                    TrieuChung = kq.TrieuChung,
                    Goc = kq
                });
            }

            MessageBox.Show(
                $"Tra cứu xong! Tìm thấy {DanhSachKetQua.Count} kết quả.",
                "Tra cứu bệnh nhân",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Helper: gom các filter từ form -> TieuChiTraCuu
        private TieuChiTraCuu BuildTieuChi()
        {
            var tc = new TieuChiTraCuu
            {
                MaBenhNhan = MaBenhNhanFilter?.Trim() ?? string.Empty,
                HoTen = HoTenFilter?.Trim() ?? string.Empty,
                TrieuChung = TrieuChungFilter?.Trim() ?? string.Empty,

                MaLoaiPhongKham = LoaiPhongKhamDuocChon?.MaLoaiPhongKham ?? string.Empty,
                MaLoaiBenh = LoaiBenhDuocChon?.MaLoaiBenh ?? string.Empty,
                MaLoaiThuoc = LoaiThuocDuocChon?.MaLoaiThuoc ?? string.Empty,
                MaDonViTinh = DonViTinhDuocChon?.MaDonViTinh ?? string.Empty,
                MaCachDung = CachDungDuocChon?.MaCachDung ?? string.Empty,

                // "Tất cả" => bỏ qua lọc theo giới tính
                GioiTinh = (GioiTinhDuocChon == "Nam" || GioiTinhDuocChon == "Nữ")
                           ? GioiTinhDuocChon
                           : string.Empty,

                NgayKhamTu = NgayKhamTu,
                NgayKhamDen = NgayKhamDen,

                NamSinhTu = ParseNullableInt(NamSinhTuStr),
                NamSinhDen = ParseNullableInt(NamSinhDenStr),
                SoLuongKeTu = ParseNullableInt(SoLuongKeTuStr),
                SoLuongKeDen = ParseNullableInt(SoLuongKeDenStr)
            };

            return tc;
        }

        // Parse string -> int? (null nếu rỗng / không phải số)
        private int? ParseNullableInt(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            return int.TryParse(s.Trim(), out int v) ? v : null;
        }

        //  COMMAND: THOÁT
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
    }

    //  DTO cho DataGrid (kèm STT) 
    public partial class KetQuaTraCuuItem : ObservableObject
    {
        [ObservableProperty] private int sTT;
        [ObservableProperty] private string hoTen = string.Empty;
        [ObservableProperty] private DateTime ngayKham;
        [ObservableProperty] private int namSinh;
        [ObservableProperty] private string tenLoaiPhongKham = string.Empty;
        [ObservableProperty] private string tenLoaiBenh = string.Empty;
        [ObservableProperty] private string trieuChung = string.Empty;

        public KetQuaTraCuuBenhNhan? Goc { get; set; }
    }
}
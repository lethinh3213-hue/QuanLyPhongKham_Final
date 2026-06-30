using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories;

namespace QuanLyPhongKham.ViewModels
{
    public partial class LapPhieuViewModel : ObservableObject
    {
        // Repositories
        private readonly LoaiPhongKhamRepository repoLoaiPK = new();
        private readonly BenhNhanRepository repoBN = new();
        private readonly LoaiBenhRepository repoLoaiBenh = new();
        private readonly LoaiThuocRepository repoLoaiThuoc = new();
        private readonly DonViTinhRepository repoDonVi = new();
        private readonly CachDungRepository repoCachDung = new();
        private readonly PhieuKhamBenhRepository repoPhieu = new();
        private readonly ChiTietPhieuKhamBenhRepository repoCT = new();

        // PROPERTIES
        [ObservableProperty] private string maPhieuKhamBenh = string.Empty;
        [ObservableProperty] private DateTime ngayKham = DateTime.Today;

        [ObservableProperty] private ObservableCollection<LoaiPhongKham> danhSachLoaiPhongKham = new();
        [ObservableProperty] private LoaiPhongKham? loaiPhongKhamDuocChon;

        [ObservableProperty] private ObservableCollection<BenhNhan> danhSachBenhNhan = new();
        [ObservableProperty] private BenhNhan? benhNhanDuocChon;

        [ObservableProperty] private string trieuChung = string.Empty;

        [ObservableProperty] private ObservableCollection<LoaiBenh> danhSachLoaiBenh = new();
        [ObservableProperty] private LoaiBenh? loaiBenhDuocChon;

        [ObservableProperty] private string ghiChu = string.Empty;

        [ObservableProperty] private string tongSoLuongThuoc = "0";

        [ObservableProperty] private ObservableCollection<LoaiThuoc> danhSachLoaiThuoc = new();

        [ObservableProperty] private ObservableCollection<ThuocKeDto> danhSachThuocKe = new();
        [ObservableProperty] private ThuocKeDto? thuocKeDuocChon;

        // CONSTRUCTOR
        public LapPhieuViewModel()
        {
            MaPhieuKhamBenh = "PKB" + DateTime.Now.ToString("yyyyMMddHHmmss");

            DanhSachLoaiPhongKham = new ObservableCollection<LoaiPhongKham>(repoLoaiPK.GetAll());
            DanhSachLoaiBenh = new ObservableCollection<LoaiBenh>(repoLoaiBenh.GetAll());
            DanhSachLoaiThuoc = new ObservableCollection<LoaiThuoc>(repoLoaiThuoc.GetAll());

            ThuocKeDto.CacheDonVi = repoDonVi.GetAll();
            ThuocKeDto.CacheCachDung = repoCachDung.GetAll();

            DanhSachThuocKe.CollectionChanged += OnDanhSachThuocKeChanged;

            // Khởi tạo 1 dòng trống ban đầu
            ThemDongTrong();
        }

        partial void OnNgayKhamChanged(DateTime value) => LoadBenhNhan();
        partial void OnLoaiPhongKhamDuocChonChanged(LoaiPhongKham? value) => LoadBenhNhan();

        private void LoadBenhNhan()
        {
            if (LoaiPhongKhamDuocChon == null)
            {
                DanhSachBenhNhan.Clear();
                return;
            }
            var list = repoBN.GetByNgayVaLoaiPhongKham(NgayKham, LoaiPhongKhamDuocChon.MaLoaiPhongKham);
            DanhSachBenhNhan = new ObservableCollection<BenhNhan>(list);
        }

        // Thêm 1 dòng trống vào DataGrid
        private void ThemDongTrong()
        {
            DanhSachThuocKe.Add(new ThuocKeDto { STT = DanhSachThuocKe.Count + 1 });
        }

        private void OnDanhSachThuocKeChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < DanhSachThuocKe.Count; i++)
                DanhSachThuocKe[i].STT = i + 1;

            if (e.NewItems != null)
            {
                foreach (ThuocKeDto item in e.NewItems)
                    item.PropertyChanged += OnThuocKePropertyChanged;
            }
            if (e.OldItems != null)
            {
                foreach (ThuocKeDto item in e.OldItems)
                    item.PropertyChanged -= OnThuocKePropertyChanged;
            }

            TinhTongSoLuong();
        }

        private void OnThuocKePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ThuocKeDto.SoLuongKe))
                TinhTongSoLuong();

            // Khi chọn thuốc trên dòng cuối -> tự thêm dòng trống mới
            if (e.PropertyName == nameof(ThuocKeDto.ThuocChon))
            {
                var dto = sender as ThuocKeDto;
                if (dto?.ThuocChon != null && DanhSachThuocKe.LastOrDefault() == dto)
                {
                    ThemDongTrong();
                }
            }
        }

        private void TinhTongSoLuong()
        {
            int tong = 0;
            foreach (var t in DanhSachThuocKe)
            {
                if (int.TryParse(t.SoLuongKe, out int sl)) tong += sl;
            }
            TongSoLuongThuoc = tong.ToString();
        }

        // COMMAND: LƯU PHIẾU KHÁM
        [RelayCommand]
        private void LuuPhieu()
        {
            if (!Validate()) return;

            if (repoPhieu.DaCoPhieuKhamBenh(BenhNhanDuocChon!.MaBenhNhan))
            {
                MessageBox.Show("Bệnh nhân này đã có phiếu khám bệnh, không thể lập thêm.", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var phieu = new PhieuKhamBenh
            {
                MaPhieuKhamBenh = MaPhieuKhamBenh,
                MaBenhNhan = BenhNhanDuocChon!.MaBenhNhan,
                TrieuChung = TrieuChung,
                MaLoaiBenh = LoaiBenhDuocChon!.MaLoaiBenh,
                GhiChu = GhiChu
            };

            if (!repoPhieu.ThemPhieuKhamBenh(phieu))
            {
                MessageBox.Show("Không lưu được phiếu khám!", "Lỗi");
                return;
            }

            int demThanhCong = 0;
            foreach (var t in DanhSachThuocKe)
            {
                if (t.ThuocChon == null || !int.TryParse(t.SoLuongKe, out int sl) || sl <= 0) continue;

                var ct = new ChiTietPhieuKhamBenh
                {
                    MaPhieuKhamBenh = MaPhieuKhamBenh,
                    MaThuoc = t.ThuocChon.MaLoaiThuoc,
                    SoLuong = sl
                };

                if (!repoCT.ThemChiTiet(ct)) continue;

                int tonMoi = t.ThuocChon.SoLuongTon - sl;

                if (repoLoaiThuoc.CapNhatSoLuongTon(t.ThuocChon.MaLoaiThuoc, tonMoi))
                {
                    t.ThuocChon.SoLuongTon = tonMoi;
                    demThanhCong++;
                }
            }

            MessageBox.Show($"Đã lưu phiếu khám với {demThanhCong} thuốc.", "Thành công");
            ResetForm();
        }

        // COMMAND: TRA CỨU BỆNH NHÂN
        [RelayCommand]
        private void TraCuu()
        {
            var dlg = new Views.TraCuuBenhNhanView { Owner = Application.Current.MainWindow };
            dlg.ShowDialog();
        }

        // COMMAND: THOÁT
        [RelayCommand]
        private void Thoat()
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (Window w in Application.Current.Windows)
                    if (w.DataContext == this) { w.Close(); break; }
            }
        }

        // VALIDATION
        private bool Validate()
        {
            if (LoaiPhongKhamDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn loại phòng khám!", "Lỗi");
                return false;
            }

            if (BenhNhanDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn bệnh nhân!", "Lỗi");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TrieuChung))
            {
                MessageBox.Show("Vui lòng nhập triệu chứng!", "Lỗi");
                return false;
            }

            if (LoaiBenhDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn loại bệnh dự đoán!", "Lỗi");
                return false;
            }

            // Lọc các dòng có thuốc (bỏ qua dòng trống cuối)
            var dongHopLe = DanhSachThuocKe.Where(x => x.ThuocChon != null).ToList();
            if (dongHopLe.Count == 0)
            {
                MessageBox.Show("Vui lòng kê ít nhất 1 thuốc!", "Lỗi");
                return false;
            }

            var maTrung = dongHopLe.GroupBy(x => x.ThuocChon!.MaLoaiThuoc)
                                   .FirstOrDefault(g => g.Count() > 1);
            if (maTrung != null)
            {
                MessageBox.Show($"Thuốc \"{maTrung.First().ThuocChon!.TenLoaiThuoc}\" bị trùng trong danh sách!", "Lỗi");
                return false;
            }

            foreach (var t in dongHopLe)
            {
                if (!int.TryParse(t.SoLuongKe, out int sl) || sl <= 0)
                {
                    MessageBox.Show($"Thuốc \"{t.ThuocChon!.TenLoaiThuoc}\": số lượng kê phải là số dương!", "Lỗi");
                    return false;
                }

                if (sl > t.ThuocChon!.SoLuongTon)
                {
                    MessageBox.Show($"Thuốc \"{t.ThuocChon!.TenLoaiThuoc}\": kê {sl} vượt quá tồn kho ({t.ThuocChon.SoLuongTon})!", "Lỗi");
                    return false;
                }
            }

            return true;
        }

        // RESET
        private void ResetForm()
        {
            MaPhieuKhamBenh = "PKB" + DateTime.Now.ToString("yyyyMMddHHmmss");
            BenhNhanDuocChon = null;
            TrieuChung = string.Empty;
            LoaiBenhDuocChon = null;
            GhiChu = string.Empty;
            DanhSachThuocKe.Clear();
            TongSoLuongThuoc = "0";

            // Thêm lại dòng trống sau khi reset
            ThemDongTrong();
        }
    }

    // Class dòng trong DataGrid
    public partial class ThuocKeDto : ObservableObject
    {
        public static List<DonViTinh> CacheDonVi = new();
        public static List<CachDung> CacheCachDung = new();

        [ObservableProperty] private int sTT;
        [ObservableProperty] private LoaiThuoc? thuocChon;
        [ObservableProperty] private string tenDonVi = string.Empty;
        [ObservableProperty] private int soLuongConLai;
        [ObservableProperty] private string soLuongKe = string.Empty;
        [ObservableProperty] private string tenCachDung = string.Empty;

        partial void OnThuocChonChanged(LoaiThuoc? value)
        {
            if (value == null)
            {
                TenDonVi = string.Empty;
                SoLuongConLai = 0;
                TenCachDung = string.Empty;
                return;
            }
            TenDonVi = CacheDonVi.FirstOrDefault(x => x.MaDonViTinh == value.MaDonViTinh)?.TenDonViTinh ?? string.Empty;
            SoLuongConLai = value.SoLuongTon;
            TenCachDung = CacheCachDung.FirstOrDefault(x => x.MaCachDung == value.MaCachDung)?.TenCachDung ?? string.Empty;
        }
    }
}
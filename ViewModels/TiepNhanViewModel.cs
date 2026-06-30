using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories;

namespace QuanLyPhongKham.ViewModels
{
    public partial class BenhNhanDto : ObservableObject
    {
        [ObservableProperty] private string maBenhNhan = string.Empty;
        [ObservableProperty] private int sTT;
        [ObservableProperty] private string hoTen = string.Empty;
        [ObservableProperty] private string gioiTinh = string.Empty;
        [ObservableProperty] private string namSinh = string.Empty;
        [ObservableProperty] private string diaChi = string.Empty;
    }
    public partial class TiepNhanViewModel : ObservableObject
    {
        // REPOSITORIES
        private readonly LoaiPhongKhamRepository _loaiPhongKhamRepo = new();
        private readonly KhamBenhRepository _khamBenhRepo = new();
        private readonly BenhNhanRepository _benhNhanRepo = new();

        // PROPERTIES

        [ObservableProperty]
        private DateTime ngayKham = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<LoaiPhongKham> danhSachLoaiPhongKham = new();

        [ObservableProperty]
        private LoaiPhongKham? loaiPhongKhamDuocChon;

        [ObservableProperty]
        private string maLoaiPhongKham = string.Empty;

        [ObservableProperty]
        private ObservableCollection<BenhNhanDto> danhSachBenhNhan = new();

        [ObservableProperty]
        private BenhNhanDto? benhNhanDuocChon;

        // CONSTRUCTOR
        public TiepNhanViewModel()
        {
            LoadLoaiPhongKham();
            DanhSachBenhNhan.Add(new BenhNhanDto { STT = 1 });
        }

        partial void OnLoaiPhongKhamDuocChonChanged(LoaiPhongKham? value)
        {
            MaLoaiPhongKham = value?.MaLoaiPhongKham ?? string.Empty;
        }

        private void LoadLoaiPhongKham()
        {
            var list = _loaiPhongKhamRepo.GetAll();
            DanhSachLoaiPhongKham = new ObservableCollection<LoaiPhongKham>(list);
        }

        private string SinhMaKhamBenh()
        {
            return "KB" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private string SinhMaBenhNhan(int index)
        {
            return "BN" + DateTime.Now.ToString("yyyyMMddHHmmss") + index.ToString("D3");
        }

        // KIỂM TRA DỮ LIỆU
        private bool KiemTraDuLieu()
        {
            if (LoaiPhongKhamDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn tên phòng khám!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DanhSachBenhNhan.Count == 0)
            {
                MessageBox.Show("Danh sách bệnh nhân trống!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            int daCoTrongNgay = _khamBenhRepo.DemSoBenhNhanTrongNgay(NgayKham, LoaiPhongKhamDuocChon.MaLoaiPhongKham);
            int dangNhapThem = DanhSachBenhNhan.Count;
            int tong = daCoTrongNgay + dangNhapThem;
            int soLuongToiDa = LoaiPhongKhamDuocChon.SoLuongToiDa;

            if (tong > soLuongToiDa)
            {
                MessageBox.Show(
                    $"Ngày {NgayKham:dd/MM/yyyy} của {LoaiPhongKhamDuocChon.TenLoaiPhongKham} đã có {daCoTrongNgay} bệnh nhân.\n" +
                    $"Bạn đang thêm {dangNhapThem} bệnh nhân nữa, vượt quá giới hạn {soLuongToiDa}.",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            int namHienTai = DateTime.Now.Year;

            for (int i = 0; i < DanhSachBenhNhan.Count; i++)
            {
                var bn = DanhSachBenhNhan[i];

                if (string.IsNullOrWhiteSpace(bn.HoTen))
                {
                    MessageBox.Show($"Dòng {i + 1}: Họ tên không được trống!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (bn.GioiTinh != "Nam" && bn.GioiTinh != "Nữ")
                {
                    MessageBox.Show($"Dòng {i + 1}: Vui lòng chọn giới tính (Nam/Nữ)!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(bn.NamSinh))
                {
                    MessageBox.Show($"Dòng {i + 1}: Năm sinh không được trống!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (!int.TryParse(bn.NamSinh, out int namSinh))
                {
                    MessageBox.Show($"Dòng {i + 1}: Năm sinh phải là số!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (namSinh < 1900 || namSinh > namHienTai)
                {
                    MessageBox.Show($"Dòng {i + 1}: Năm sinh phải từ 1900 đến {namHienTai}!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(bn.DiaChi))
                {
                    MessageBox.Show($"Dòng {i + 1}: Địa chỉ không được trống!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        // COMMAND: TIẾP NHẬN
        [RelayCommand]
        private void TiepNhan()
        {
            if (!KiemTraDuLieu()) return;

            foreach (var dto in DanhSachBenhNhan)
            {
                int.TryParse(dto.NamSinh, out int nsKiemTra);
                if (_benhNhanRepo.TonTai(dto.HoTen, nsKiemTra))
                {
                    MessageBox.Show($"Bệnh nhân \"{dto.HoTen}\" (năm sinh {dto.NamSinh}) đã có trong hệ thống.\nKhông thể tiếp nhận trùng.",
                        "Trùng bệnh nhân", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            string maKB = SinhMaKhamBenh();
            var khamBenh = new KhamBenh
            {
                MaKhamBenh = maKB,
                NgayKham = NgayKham,
                MaLoaiPhongKham = LoaiPhongKhamDuocChon!.MaLoaiPhongKham
            };

            bool okKB = _khamBenhRepo.ThemKhamBenh(khamBenh);
            if (!okKB)
            {
                MessageBox.Show("Lỗi lưu buổi khám!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int demLuuThanhCong = 0;
            for (int i = 0; i < DanhSachBenhNhan.Count; i++)
            {
                var bnDto = DanhSachBenhNhan[i];

                int.TryParse(bnDto.NamSinh, out int namSinhInt);

                var bn = new BenhNhan
                {
                    MaBenhNhan = SinhMaBenhNhan(i + 1),
                    HoTen = bnDto.HoTen,
                    GioiTinh = bnDto.GioiTinh,
                    NamSinh = namSinhInt,
                    DiaChi = bnDto.DiaChi,
                    MaKhamBenh = maKB
                };

                if (_benhNhanRepo.ThemBenhNhan(bn))
                    demLuuThanhCong++;
            }

            MessageBox.Show(
                $"Tiếp nhận thành công!\nĐã lưu {demLuuThanhCong}/{DanhSachBenhNhan.Count} bệnh nhân.",
                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            XoaForm();
        }

        // CÁC COMMAND KHÁC
        [RelayCommand]
        private void ThemBenhNhan()
        {
            int stt = DanhSachBenhNhan.Count + 1;
            DanhSachBenhNhan.Add(new BenhNhanDto { STT = stt });
        }

        [RelayCommand]
        private void Xoa()
        {
            if (BenhNhanDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DanhSachBenhNhan.Remove(BenhNhanDuocChon);

            for (int i = 0; i < DanhSachBenhNhan.Count; i++)
                DanhSachBenhNhan[i].STT = i + 1;
        }

        [RelayCommand]
        private void CapNhat()
        {
            if (BenhNhanDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn dòng cần cập nhật!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(BenhNhanDuocChon.MaBenhNhan))
            {
                MessageBox.Show("Dòng này chưa có trong CSDL. Hãy bấm 'Tiếp nhận danh sách' để lưu mới.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(BenhNhanDuocChon.NamSinh, out int namSinh))
            {
                MessageBox.Show("Năm sinh không hợp lệ!", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var bn = new BenhNhan
            {
                MaBenhNhan = BenhNhanDuocChon.MaBenhNhan,
                HoTen = BenhNhanDuocChon.HoTen,
                GioiTinh = BenhNhanDuocChon.GioiTinh,
                NamSinh = namSinh,
                DiaChi = BenhNhanDuocChon.DiaChi
            };

            bool ok = _benhNhanRepo.CapNhatBenhNhan(bn);

            MessageBox.Show(
                ok ? "Cập nhật bệnh nhân thành công!" : "Cập nhật thất bại!",
                ok ? "Thành công" : "Lỗi",
                MessageBoxButton.OK,
                ok ? MessageBoxImage.Information : MessageBoxImage.Error);
        }

        [RelayCommand]
        private void TimKiem()
        {
            if (LoaiPhongKhamDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn tên phòng khám!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ds = _benhNhanRepo.GetByNgayVaLoaiPhongKham(
                NgayKham.Date,
                LoaiPhongKhamDuocChon.MaLoaiPhongKham);

            DanhSachBenhNhan.Clear();
            BenhNhanDuocChon = null;

            if (ds.Count == 0)
            {
                MessageBox.Show(
                    $"Không có bệnh nhân cho ngày {NgayKham:dd/MM/yyyy} - {LoaiPhongKhamDuocChon.TenLoaiPhongKham}.",
                    "Kết quả", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            for (int i = 0; i < ds.Count; i++)
            {
                var bn = ds[i];
                DanhSachBenhNhan.Add(new BenhNhanDto
                {
                    MaBenhNhan = bn.MaBenhNhan,
                    STT = i + 1,
                    HoTen = bn.HoTen,
                    GioiTinh = bn.GioiTinh,
                    NamSinh = bn.NamSinh.ToString(),
                    DiaChi = bn.DiaChi
                });
            }

            MessageBox.Show(
                $"Đã tìm thấy {ds.Count} bệnh nhân cho ngày {NgayKham:dd/MM/yyyy} - {LoaiPhongKhamDuocChon.TenLoaiPhongKham}.",
                "Kết quả", MessageBoxButton.OK, MessageBoxImage.Information);
        }

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

        private void XoaForm()
        {
            DanhSachBenhNhan.Clear();
            DanhSachBenhNhan.Add(new BenhNhanDto { STT = 1 });
            LoaiPhongKhamDuocChon = null;
            MaLoaiPhongKham = string.Empty;
        }
    }

}
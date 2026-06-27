namespace QuanLyPhongKham.Models
{
    public class TieuChiTraCuu
    {
        // Lọc trực tiếp - bệnh nhân
        public string MaBenhNhan { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string GioiTinh { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public int? NamSinhTu { get; set; } = null;
        public int? NamSinhDen { get; set; } = null;

        // Lọc gián tiếp - khám bệnh & phòng khám
        public DateTime? NgayKhamTu { get; set; } = null;
        public DateTime? NgayKhamDen { get; set; } = null;
        public string MaLoaiPhongKham { get; set; } = string.Empty;

        // Lọc gián tiếp - phiếu khám bệnh
        public string MaPhieuKhamBenh { get; set; } = string.Empty;
        public string TrieuChung { get; set; } = string.Empty;
        public string MaLoaiBenh { get; set; } = string.Empty;

        // Lọc gián tiếp - thuốc
        public string MaLoaiThuoc { get; set; } = string.Empty;
        public string MaDonViTinh { get; set; } = string.Empty;
        public string MaCachDung { get; set; } = string.Empty;
        public int? SoLuongKeTu { get; set; } = null;
        public int? SoLuongKeDen { get; set; } = null;
    }
}
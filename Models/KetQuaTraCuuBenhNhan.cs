namespace QuanLyPhongKham.Models
{
    public class KetQuaTraCuuBenhNhan
    {
        // Thông tin bệnh nhân
        public string MaBenhNhan { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string GioiTinh { get; set; } = string.Empty;
        public int NamSinh { get; set; } = 0;
        public string DiaChi { get; set; } = string.Empty;

        // Thông tin khám bệnh
        public string MaKhamBenh { get; set; } = string.Empty;
        public DateTime NgayKham { get; set; } = DateTime.Today;

        // Thông tin loại phòng khám
        public string MaLoaiPhongKham { get; set; } = string.Empty;
        public string TenLoaiPhongKham { get; set; } = string.Empty;

        // Thông tin phiếu khám bệnh
        public string MaPhieuKhamBenh { get; set; } = string.Empty;
        public string TrieuChung { get; set; } = string.Empty;
        public string GhiChu { get; set; } = string.Empty;

        // Thông tin loại bệnh
        public string MaLoaiBenh { get; set; } = string.Empty;
        public string TenLoaiBenh { get; set; } = string.Empty;

        // Thông tin thuốc
        public string MaThuoc { get; set; } = string.Empty;
        public string TenLoaiThuoc { get; set; } = string.Empty;
    }
}
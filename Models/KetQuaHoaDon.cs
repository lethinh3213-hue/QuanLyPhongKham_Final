namespace QuanLyPhongKham.Models
{
    // Kết quả tính hóa đơn để hiển thị lên màn hình (gồm cả thông tin mô tả + số tiền).
    public class KetQuaHoaDon
    {
        public string MaPhieuKhamBenh { get; set; } = string.Empty;
        public string HoTenBenhNhan { get; set; } = string.Empty;
        public DateTime NgayKham { get; set; } = DateTime.Today;
        public string TenLoaiPhongKham { get; set; } = string.Empty;

        public decimal TienKham { get; set; } = 0;
        public decimal TienThuoc { get; set; } = 0;
        public decimal TongTien { get; set; } = 0;

        public bool CoDungThuoc { get; set; } = false;
    }
}

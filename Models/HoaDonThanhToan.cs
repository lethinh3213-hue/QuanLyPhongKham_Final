namespace QuanLyPhongKham.Models
{
    // Bảng mới Sprint 4: HOADONTHANHTOAN
    public class HoaDonThanhToan
    {
        public string MaHoaDon { get; set; } = string.Empty;
        public string MaPhieuKhamBenh { get; set; } = string.Empty;
        public decimal TienKham { get; set; } = 0;
        public decimal TienThuoc { get; set; } = 0;
        public decimal TongTien { get; set; } = 0;
    }
}

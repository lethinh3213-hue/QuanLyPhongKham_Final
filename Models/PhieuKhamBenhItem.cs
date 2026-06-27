namespace QuanLyPhongKham.Models
{
    // DTO dùng cho ComboBox "Mã phiếu khám bệnh" và dialog "Tìm phiếu khám bệnh".
    // Gom sẵn vài thông tin mô tả để người dùng dễ nhận biết phiếu.
    public class PhieuKhamBenhItem
    {
        public string MaPhieuKhamBenh { get; set; } = string.Empty;
        public string HoTenBenhNhan { get; set; } = string.Empty;
        public DateTime NgayKham { get; set; } = DateTime.Today;
        public string TenLoaiPhongKham { get; set; } = string.Empty;
        public string TenLoaiBenh { get; set; } = string.Empty;
    }
}

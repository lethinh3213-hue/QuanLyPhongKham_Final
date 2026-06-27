namespace QuanLyPhongKham.Models
{
    // Một dòng trong báo cáo sử dụng thuốc theo tháng (theo BM5.2)
    public class BaoCaoThuocItem
    {
        public int STT { get; set; }
        public string TenThuoc { get; set; } = string.Empty;
        public string DonViTinh { get; set; } = string.Empty;
        public int SoLuongTon { get; set; }
        public int SoLuongDung { get; set; }
    }
}

namespace QuanLyPhongKham.Models
{
    public class LoaiThuoc
    {
        public string MaLoaiThuoc { get; set; } = string.Empty;
        public string TenLoaiThuoc { get; set; } = string.Empty;
        public string MaDonViTinh { get; set; } = string.Empty;
        public string MaCachDung { get; set; } = string.Empty;
        public int SoLuongTon { get; set; } = 0;
        public decimal DonGia { get; set; } = 0;     // (*) MỚI Sprint 4
    }
}

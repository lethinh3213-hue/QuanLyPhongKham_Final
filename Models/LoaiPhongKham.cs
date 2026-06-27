namespace QuanLyPhongKham.Models
{
    public class LoaiPhongKham
    {
        public string MaLoaiPhongKham { get; set; } = string.Empty;
        public string TenLoaiPhongKham { get; set; } = string.Empty;
        public int SoLuongToiDa { get; set; } = 0;
        public decimal TienKham { get; set; } = 0;   // (*) MỚI Sprint 4
    }
}

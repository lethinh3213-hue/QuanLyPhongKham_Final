namespace QuanLyPhongKham.Models
{
    // Một dòng trong báo cáo hóa đơn theo tháng (theo BM5.1)
    public class BaoCaoHoaDonItem
    {
        public int STT { get; set; }
        public DateTime Ngay { get; set; }
        public int SoBenhNhan { get; set; }
        public decimal DoanhThu { get; set; }
        public double TiLe { get; set; }   // phần trăm so với tổng doanh thu
    }
}

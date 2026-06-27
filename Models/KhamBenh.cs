namespace QuanLyPhongKham.Models
{
    public class KhamBenh
    {
        public string MaKhamBenh { get; set; } = string.Empty;
        public DateTime NgayKham { get; set; } = DateTime.Today;
        public string MaLoaiPhongKham { get; set; } = string.Empty;
    }
}
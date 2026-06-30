using System.Windows;

namespace QuanLyPhongKham.Views
{
    public partial class MainMenuView : Window
    {
        public MainMenuView()
        {
            InitializeComponent();
        }

        // Sprint 1 - Tiếp nhận bệnh nhân
        private void TiepNhan_Click(object sender, RoutedEventArgs e)
        {
            new TiepNhanView().Show();
        }

        // Sprint 2 - Lập phiếu khám bệnh
        private void LapPhieu_Click(object sender, RoutedEventArgs e)
        {
            new LapPhieuView().Show();
        }

        // Sprint 3 - Tra cứu bệnh nhân
        private void TraCuu_Click(object sender, RoutedEventArgs e)
        {
            new TraCuuBenhNhanView().Show();
        }

        // Sprint 4 - Lập hóa đơn thanh toán
        private void LapHoaDon_Click(object sender, RoutedEventArgs e)
        {
            new LapHoaDonThanhToanView().Show();
        }

        // Sprint 5.1 - Lập báo cáo hóa đơn theo tháng
        private void BaoCaoHoaDon_Click(object sender, RoutedEventArgs e)
        {
            new LapBaoCaoHoaDonView().Show();
        }

        // Sprint 5.2 - Lập báo cáo sử dụng thuốc theo tháng
        private void BaoCaoThuoc_Click(object sender, RoutedEventArgs e)
        {
            new LapBaoCaoThuocView().Show();
        }

        private void ThayDoiQuyDinh_Click(object sender, RoutedEventArgs e)
        {
            new ThayDoiQuyDinhView().Show();
        }

        private void Thoat_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

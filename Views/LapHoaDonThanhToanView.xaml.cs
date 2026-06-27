using System.Windows;
using QuanLyPhongKham.ViewModels;

namespace QuanLyPhongKham.Views
{
    public partial class LapHoaDonThanhToanView : Window
    {
        public LapHoaDonThanhToanView()
        {
            InitializeComponent();
            DataContext = new LapHoaDonThanhToanViewModel();
        }
    }
}

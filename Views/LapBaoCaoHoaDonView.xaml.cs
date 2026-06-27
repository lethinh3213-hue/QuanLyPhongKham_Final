using System.Windows;
using QuanLyPhongKham.ViewModels;

namespace QuanLyPhongKham.Views
{
    public partial class LapBaoCaoHoaDonView : Window
    {
        public LapBaoCaoHoaDonView()
        {
            InitializeComponent();
            DataContext = new LapBaoCaoHoaDonViewModel();
        }
    }
}

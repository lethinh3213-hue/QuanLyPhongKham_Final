using System.Windows;
using QuanLyPhongKham.ViewModels;

namespace QuanLyPhongKham.Views
{
    public partial class LapBaoCaoThuocView : Window
    {
        public LapBaoCaoThuocView()
        {
            InitializeComponent();
            DataContext = new LapBaoCaoThuocViewModel();
        }
    }
}

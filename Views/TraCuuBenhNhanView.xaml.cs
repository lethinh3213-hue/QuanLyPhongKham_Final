using System.Windows;
using QuanLyPhongKham.ViewModels;

namespace QuanLyPhongKham.Views
{
    public partial class TraCuuBenhNhanView : Window
    {
        public TraCuuBenhNhanView()
        {
            InitializeComponent();
            DataContext = new TraCuuBenhNhanViewModel();
        }
    }
}
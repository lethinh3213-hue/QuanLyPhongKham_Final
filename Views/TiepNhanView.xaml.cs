using System.Windows;
using QuanLyPhongKham.ViewModels;

namespace QuanLyPhongKham.Views
{
    public partial class TiepNhanView : Window
    {
        public TiepNhanView()
        {
            InitializeComponent();
            DataContext = new TiepNhanViewModel();
        }
    }
}
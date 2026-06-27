using System.Windows;
using QuanLyPhongKham.ViewModels;

namespace QuanLyPhongKham.Views
{
    public partial class LapPhieuView : Window
    {
        public LapPhieuView()
        {
            InitializeComponent();
            DataContext = new LapPhieuViewModel();
        }
    }
}

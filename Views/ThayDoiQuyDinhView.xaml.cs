using System.Windows;
using QuanLyPhongKham.ViewModels;

namespace QuanLyPhongKham.Views
{
    public partial class ThayDoiQuyDinhView : Window
    {
        public ThayDoiQuyDinhView()
        {
            InitializeComponent();
            DataContext = new ThayDoiQuyDinhViewModel();
        }
    }
}

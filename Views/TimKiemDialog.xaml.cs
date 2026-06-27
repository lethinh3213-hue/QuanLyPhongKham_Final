using System.Windows;

namespace QuanLyPhongKham.Views
{
    public partial class TimKiemDialog : Window
    {
        // Từ khóa người dùng nhập vào
        public string TuKhoa { get; private set; } = string.Empty;

        public TimKiemDialog()
        {
            InitializeComponent();
            Loaded += (s, e) => txtTuKhoa.Focus();
        }

        private void BtnTim_Click(object sender, RoutedEventArgs e)
        {
            TuKhoa = txtTuKhoa.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

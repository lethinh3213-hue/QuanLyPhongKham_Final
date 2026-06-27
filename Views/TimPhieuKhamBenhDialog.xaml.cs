using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using QuanLyPhongKham.Models;

namespace QuanLyPhongKham.Views
{
    public partial class TimPhieuKhamBenhDialog : Window
    {
        private readonly List<PhieuKhamBenhItem> _tatCa;

        // Mã phiếu được người dùng chọn (rỗng nếu không chọn)
        public string MaPhieuDuocChon { get; private set; } = string.Empty;

        public TimPhieuKhamBenhDialog(IEnumerable<PhieuKhamBenhItem> danhSach)
        {
            InitializeComponent();
            _tatCa = danhSach?.ToList() ?? new List<PhieuKhamBenhItem>();
            dgPhieu.ItemsSource = new ObservableCollection<PhieuKhamBenhItem>(_tatCa);
        }

        // Lọc theo Mã phiếu hoặc Họ tên (không phân biệt hoa thường)
        private void txtTuKhoa_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string tuKhoa = (txtTuKhoa.Text ?? string.Empty).Trim().ToLower();

            var loc = string.IsNullOrEmpty(tuKhoa)
                ? _tatCa
                : _tatCa.Where(p =>
                        p.MaPhieuKhamBenh.ToLower().Contains(tuKhoa) ||
                        p.HoTenBenhNhan.ToLower().Contains(tuKhoa))
                    .ToList();

            dgPhieu.ItemsSource = new ObservableCollection<PhieuKhamBenhItem>(loc);
        }

        private void btnChon_Click(object sender, RoutedEventArgs e) => XacNhanChon();

        private void dgPhieu_MouseDoubleClick(object sender, MouseButtonEventArgs e) => XacNhanChon();

        private void XacNhanChon()
        {
            if (dgPhieu.SelectedItem is PhieuKhamBenhItem item)
            {
                MaPhieuDuocChon = item.MaPhieuKhamBenh;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một phiếu khám bệnh trong danh sách.",
                    "Tìm phiếu khám bệnh", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnDong_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

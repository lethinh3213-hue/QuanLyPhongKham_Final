using MySql.Data.MySqlClient;
using QuanLyPhongKham.Helpers;
using QuanLyPhongKham.Models;
using System.Windows;

namespace QuanLyPhongKham.Repositories
{
    public class DonViTinhRepository
    {
        private readonly DBHelper db = new DBHelper();

        public List<DonViTinh> GetAll()
        {
            var ds = new List<DonViTinh>();

            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                using var cmd = new MySqlCommand("SELECT * FROM DONVITINH", conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ds.Add(new DonViTinh
                    {
                        MaDonViTinh = reader.GetString("MaDonViTinh"),
                        TenDonViTinh = reader.GetString("TenDonViTinh")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách đơn vị tính: " + ex.Message);
            }

            return ds;
        }
    }
}

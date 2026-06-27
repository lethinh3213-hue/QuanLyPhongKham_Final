using MySql.Data.MySqlClient;
using QuanLyPhongKham.Helpers;
using QuanLyPhongKham.Models;
using System.Windows;

namespace QuanLyPhongKham.Repositories
{
    public class LoaiBenhRepository
    {
        private readonly DBHelper db = new DBHelper();

        public List<LoaiBenh> GetAll()
        {
            var ds = new List<LoaiBenh>();

            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                using var cmd = new MySqlCommand("SELECT * FROM LOAIBENH", conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ds.Add(new LoaiBenh
                    {
                        MaLoaiBenh = reader.GetString("MaLoaiBenh"),
                        TenLoaiBenh = reader.GetString("TenLoaiBenh")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách loại bệnh: " + ex.Message);
            }

            return ds;
        }
    }
}

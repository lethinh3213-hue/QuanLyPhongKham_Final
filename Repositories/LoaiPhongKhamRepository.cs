using MySql.Data.MySqlClient;
using QuanLyPhongKham.Helpers;
using QuanLyPhongKham.Models;
using System.Windows;

namespace QuanLyPhongKham.Repositories
{
    public class LoaiPhongKhamRepository
    {
        private readonly DBHelper db = new DBHelper();

        public List<LoaiPhongKham> GetAll()
        {
            var ds = new List<LoaiPhongKham>();

            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                using var cmd = new MySqlCommand("SELECT * FROM LOAIPHONGKHAM", conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ds.Add(new LoaiPhongKham
                    {
                        MaLoaiPhongKham = reader.GetString("MaLoaiPhongKham"),
                        TenLoaiPhongKham = reader.GetString("TenLoaiPhongKham"),
                        SoLuongToiDa = reader.GetInt32("SoLuongToiDa")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách loại phòng khám: " + ex.Message);
            }

            return ds;
        }
    }
}

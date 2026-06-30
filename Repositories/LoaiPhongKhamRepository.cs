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
                        SoLuongToiDa = reader.GetInt32("SoLuongToiDa"),
                        TienKham = reader.GetDecimal("TienKham")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách loại phòng khám: " + ex.Message);
            }

            return ds;
        }

        public bool CapNhat(LoaiPhongKham lpk)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = "UPDATE LOAIPHONGKHAM SET TenLoaiPhongKham = @ten, SoLuongToiDa = @sl, TienKham = @tien WHERE MaLoaiPhongKham = @ma";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ten", lpk.TenLoaiPhongKham);
                cmd.Parameters.AddWithValue("@sl", lpk.SoLuongToiDa);
                cmd.Parameters.AddWithValue("@tien", lpk.TienKham);
                cmd.Parameters.AddWithValue("@ma", lpk.MaLoaiPhongKham);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật loại phòng khám: " + ex.Message);
                return false;
            }
        }

    }
}
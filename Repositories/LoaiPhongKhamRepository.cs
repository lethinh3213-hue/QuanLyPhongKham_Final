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

        public bool Them(LoaiPhongKham lpk)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string maMoi = SinhMa(conn);
                string sql = @"INSERT INTO LOAIPHONGKHAM
                               (MaLoaiPhongKham, TenLoaiPhongKham, SoLuongToiDa, TienKham)
                               VALUES (@ma, @ten, @sl, @tien)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maMoi);
                cmd.Parameters.AddWithValue("@ten", lpk.TenLoaiPhongKham);
                cmd.Parameters.AddWithValue("@sl", lpk.SoLuongToiDa);
                cmd.Parameters.AddWithValue("@tien", lpk.TienKham);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm loại phòng khám: " + ex.Message);
                return false;
            }
        }

        public bool CapNhat(LoaiPhongKham lpk)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = @"UPDATE LOAIPHONGKHAM
                               SET TenLoaiPhongKham = @ten,
                                   SoLuongToiDa = @sl,
                                   TienKham = @tien
                               WHERE MaLoaiPhongKham = @ma";
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

        public bool Xoa(string ma)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = "DELETE FROM LOAIPHONGKHAM WHERE MaLoaiPhongKham = @ma";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa loại phòng khám: " + ex.Message);
                return false;
            }
        }

        private string SinhMa(MySqlConnection conn)
        {
            string sql = "SELECT IFNULL(MAX(CAST(SUBSTRING(MaLoaiPhongKham, 3) AS UNSIGNED)), 0) + 1 FROM LOAIPHONGKHAM";
            using var cmd = new MySqlCommand(sql, conn);
            int so = Convert.ToInt32(cmd.ExecuteScalar());
            return "PK" + so.ToString("D2");
        }
    }
}
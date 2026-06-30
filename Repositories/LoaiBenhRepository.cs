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

        public bool Them(LoaiBenh x)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string maMoi = SinhMa(conn);
                string sql = "INSERT INTO LOAIBENH (MaLoaiBenh, TenLoaiBenh) VALUES (@ma, @ten)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maMoi);
                cmd.Parameters.AddWithValue("@ten", x.TenLoaiBenh);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm dữ liệu: " + ex.Message);
                return false;
            }
        }

        public bool CapNhat(LoaiBenh x)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = "UPDATE LOAIBENH SET TenLoaiBenh = @ten WHERE MaLoaiBenh = @ma";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ten", x.TenLoaiBenh);
                cmd.Parameters.AddWithValue("@ma", x.MaLoaiBenh);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật dữ liệu: " + ex.Message);
                return false;
            }
        }

        public bool Xoa(string ma)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = "DELETE FROM LOAIBENH WHERE MaLoaiBenh = @ma";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Không thể xóa vì loại bệnh đang được dùng trong phiếu khám.");
                return false;
            }
        }

        private string SinhMa(MySqlConnection conn)
        {
            string sql = "SELECT IFNULL(MAX(CAST(SUBSTRING(MaLoaiBenh, 3) AS UNSIGNED)), 0) + 1 FROM LOAIBENH";
            using var cmd = new MySqlCommand(sql, conn);
            int so = Convert.ToInt32(cmd.ExecuteScalar());
            return "LB" + so.ToString("D2");
        }

    }
}

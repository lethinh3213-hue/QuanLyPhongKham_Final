using MySql.Data.MySqlClient;
using QuanLyPhongKham.Helpers;
using QuanLyPhongKham.Models;
using System.Windows;

namespace QuanLyPhongKham.Repositories
{
    public class CachDungRepository
    {
        private readonly DBHelper db = new DBHelper();

        public List<CachDung> GetAll()
        {
            var ds = new List<CachDung>();

            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                using var cmd = new MySqlCommand("SELECT * FROM CACHDUNG", conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ds.Add(new CachDung
                    {
                        MaCachDung = reader.GetString("MaCachDung"),
                        TenCachDung = reader.GetString("TenCachDung")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách cách dùng: " + ex.Message);
            }

            return ds;
        }

        public bool Them(CachDung x)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string maMoi = SinhMa(conn);
                string sql = "INSERT INTO CACHDUNG (MaCachDung, TenCachDung) VALUES (@ma, @ten)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maMoi);
                cmd.Parameters.AddWithValue("@ten", x.TenCachDung);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm dữ liệu: " + ex.Message);
                return false;
            }
        }

        public bool CapNhat(CachDung x)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = "UPDATE CACHDUNG SET TenCachDung = @ten WHERE MaCachDung = @ma";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ten", x.TenCachDung);
                cmd.Parameters.AddWithValue("@ma", x.MaCachDung);
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

                string sql = "DELETE FROM CACHDUNG WHERE MaCachDung = @ma";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Không thể xóa vì cách dùng đang được dùng cho thuốc.");
                return false;
            }
        }

        private string SinhMa(MySqlConnection conn)
        {
            string sql = "SELECT IFNULL(MAX(CAST(SUBSTRING(MaCachDung, 3) AS UNSIGNED)), 0) + 1 FROM CACHDUNG";
            using var cmd = new MySqlCommand(sql, conn);
            int so = Convert.ToInt32(cmd.ExecuteScalar());
            return "CD" + so.ToString("D2");
        }

    }
}

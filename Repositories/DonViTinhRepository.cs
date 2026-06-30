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

        public bool Them(DonViTinh x)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string maMoi = SinhMa(conn);
                string sql = "INSERT INTO DONVITINH (MaDonViTinh, TenDonViTinh) VALUES (@ma, @ten)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maMoi);
                cmd.Parameters.AddWithValue("@ten", x.TenDonViTinh);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm dữ liệu: " + ex.Message);
                return false;
            }
        }

        public bool CapNhat(DonViTinh x)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = "UPDATE DONVITINH SET TenDonViTinh = @ten WHERE MaDonViTinh = @ma";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ten", x.TenDonViTinh);
                cmd.Parameters.AddWithValue("@ma", x.MaDonViTinh);
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

                string sql = "DELETE FROM DONVITINH WHERE MaDonViTinh = @ma";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Không thể xóa vì đơn vị tính đang được dùng cho thuốc.");
                return false;
            }
        }

        private string SinhMa(MySqlConnection conn)
        {
            string sql = "SELECT IFNULL(MAX(CAST(SUBSTRING(MaDonViTinh, 3) AS UNSIGNED)), 0) + 1 FROM DONVITINH";
            using var cmd = new MySqlCommand(sql, conn);
            int so = Convert.ToInt32(cmd.ExecuteScalar());
            return "DV" + so.ToString("D2");
        }

    }
}

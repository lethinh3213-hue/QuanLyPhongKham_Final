using MySql.Data.MySqlClient;
using QuanLyPhongKham.Helpers;
using QuanLyPhongKham.Models;
using System.Windows;

namespace QuanLyPhongKham.Repositories
{
    public class LoaiThuocRepository
    {
        private readonly DBHelper db = new DBHelper();

        public List<LoaiThuoc> GetAll()
        {
            var ds = new List<LoaiThuoc>();

            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                using var cmd = new MySqlCommand("SELECT * FROM LOAITHUOC", conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ds.Add(new LoaiThuoc
                    {
                        MaLoaiThuoc = reader.GetString("MaLoaiThuoc"),
                        TenLoaiThuoc = reader.GetString("TenLoaiThuoc"),
                        MaDonViTinh = reader.GetString("MaDonViTinh"),
                        MaCachDung = reader.GetString("MaCachDung"),
                        SoLuongTon = reader.GetInt32("SoLuongTon"),
                        DonGia = reader.GetDecimal("DonGia")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách thuốc: " + ex.Message);
            }

            return ds;
        }

        // Cập nhật số lượng tồn sau khi kê thuốc
        public bool CapNhatSoLuongTon(string maThuoc, int soLuongTonMoi)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = "UPDATE LOAITHUOC SET SoLuongTon = @ton WHERE MaLoaiThuoc = @ma";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ton", soLuongTonMoi);
                cmd.Parameters.AddWithValue("@ma", maThuoc);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật số lượng tồn: " + ex.Message);
                return false;
            }
        }

        public bool Them(LoaiThuoc x)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string maMoi = SinhMa(conn);
                string sql = "INSERT INTO LOAITHUOC (MaLoaiThuoc, TenLoaiThuoc, MaDonViTinh, MaCachDung, SoLuongTon, DonGia) VALUES (@ma, @ten, @dvt, @cd, @ton, @gia)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maMoi);
                cmd.Parameters.AddWithValue("@ten", x.TenLoaiThuoc);
                cmd.Parameters.AddWithValue("@dvt", x.MaDonViTinh);
                cmd.Parameters.AddWithValue("@cd", x.MaCachDung);
                cmd.Parameters.AddWithValue("@ton", x.SoLuongTon);
                cmd.Parameters.AddWithValue("@gia", x.DonGia);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm thuốc: " + ex.Message);
                return false;
            }
        }

        public bool CapNhat(LoaiThuoc x)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = "UPDATE LOAITHUOC SET TenLoaiThuoc = @ten, MaDonViTinh = @dvt, MaCachDung = @cd, SoLuongTon = @ton, DonGia = @gia WHERE MaLoaiThuoc = @ma";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ten", x.TenLoaiThuoc);
                cmd.Parameters.AddWithValue("@dvt", x.MaDonViTinh);
                cmd.Parameters.AddWithValue("@cd", x.MaCachDung);
                cmd.Parameters.AddWithValue("@ton", x.SoLuongTon);
                cmd.Parameters.AddWithValue("@gia", x.DonGia);
                cmd.Parameters.AddWithValue("@ma", x.MaLoaiThuoc);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật thuốc: " + ex.Message);
                return false;
            }
        }

        public bool Xoa(string ma)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = "DELETE FROM LOAITHUOC WHERE MaLoaiThuoc = @ma";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Không thể xóa thuốc này vì đang được kê trong phiếu khám.");
                return false;
            }
        }

        private string SinhMa(MySqlConnection conn)
        {
            string sql = "SELECT IFNULL(MAX(CAST(SUBSTRING(MaLoaiThuoc, 2) AS UNSIGNED)), 0) + 1 FROM LOAITHUOC";
            using var cmd = new MySqlCommand(sql, conn);
            int so = Convert.ToInt32(cmd.ExecuteScalar());
            return "T" + so.ToString("D2");
        }

    }
}
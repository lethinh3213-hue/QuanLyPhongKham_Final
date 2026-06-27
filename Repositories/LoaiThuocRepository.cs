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
                        SoLuongTon = reader.GetInt32("SoLuongTon")
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
    }
}

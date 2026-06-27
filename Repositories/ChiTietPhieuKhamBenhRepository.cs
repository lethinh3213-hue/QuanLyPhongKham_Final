using MySql.Data.MySqlClient;
using QuanLyPhongKham.Helpers;
using QuanLyPhongKham.Models;
using System.Windows;

namespace QuanLyPhongKham.Repositories
{
    public class ChiTietPhieuKhamBenhRepository
    {
        private readonly DBHelper db = new DBHelper();

        // Lưu 1 dòng chi tiết thuốc của phiếu khám
        public bool ThemChiTiet(ChiTietPhieuKhamBenh ct)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = @"INSERT INTO CTPHIEUKHAMBENH 
                               (MaPhieuKhamBenh, MaThuoc, SoLuong)
                               VALUES (@pkb, @thuoc, @sl)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@pkb", ct.MaPhieuKhamBenh);
                cmd.Parameters.AddWithValue("@thuoc", ct.MaThuoc);
                cmd.Parameters.AddWithValue("@sl", ct.SoLuong);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu chi tiết phiếu khám: " + ex.Message);
                return false;
            }
        }
    }
}

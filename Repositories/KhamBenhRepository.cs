using MySql.Data.MySqlClient;
using QuanLyPhongKham.Helpers;
using QuanLyPhongKham.Models;
using System.Windows;

namespace QuanLyPhongKham.Repositories
{
    public class KhamBenhRepository
    {
        private readonly DBHelper _dbHelper = new DBHelper();

        public List<KhamBenh> GetAll()
        {
            List<KhamBenh> danhSach = new List<KhamBenh>();
            try
            {
                using (MySqlConnection conn = _dbHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT MaKhamBenh, NgayKham, MaLoaiPhongKham FROM KHAMBENH";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                danhSach.Add(new KhamBenh
                                {
                                    MaKhamBenh = reader.GetString("MaKhamBenh"),
                                    NgayKham = reader.GetDateTime("NgayKham"),
                                    MaLoaiPhongKham = reader.GetString("MaLoaiPhongKham")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách khám bệnh: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return danhSach;
        }

        public bool ThemKhamBenh(KhamBenh kb)
        {
            try
            {
                using (MySqlConnection conn = _dbHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO KHAMBENH 
                                    (MaKhamBenh, NgayKham, MaLoaiPhongKham) 
                                    VALUES (@Ma, @Ngay, @LoaiPK)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Ma", kb.MaKhamBenh);
                        cmd.Parameters.AddWithValue("@Ngay", kb.NgayKham);
                        cmd.Parameters.AddWithValue("@LoaiPK", kb.MaLoaiPhongKham);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm khám bệnh: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public int DemSoBenhNhanTrongNgay(DateTime ngayKham, string maLoaiPhongKham)
        {
            try
            {
                using (MySqlConnection conn = _dbHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                SELECT COUNT(*)
                FROM BENHNHAN bn
                INNER JOIN KHAMBENH kb ON bn.MaKhamBenh = kb.MaKhamBenh
                WHERE kb.NgayKham = @NgayKham
                  AND kb.MaLoaiPhongKham = @MaLoaiPhongKham";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NgayKham", ngayKham.Date);
                        cmd.Parameters.AddWithValue("@MaLoaiPhongKham", maLoaiPhongKham);

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đếm số bệnh nhân trong ngày: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }
    }
}
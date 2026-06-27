using MySql.Data.MySqlClient;
using QuanLyPhongKham.Helpers;
using QuanLyPhongKham.Models;
using System.Windows;

namespace QuanLyPhongKham.Repositories
{
    public class PhieuKhamBenhRepository
    {
        private readonly DBHelper db = new DBHelper();

        // Lưu phiếu khám bệnh
        public bool ThemPhieuKhamBenh(PhieuKhamBenh pkb)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = @"INSERT INTO PHIEUKHAMBENH 
                               (MaPhieuKhamBenh, MaBenhNhan, TrieuChung, MaLoaiBenh, GhiChu)
                               VALUES (@ma, @bn, @tc, @lb, @gc)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", pkb.MaPhieuKhamBenh);
                cmd.Parameters.AddWithValue("@bn", pkb.MaBenhNhan);
                cmd.Parameters.AddWithValue("@tc", pkb.TrieuChung);
                cmd.Parameters.AddWithValue("@lb", pkb.MaLoaiBenh);
                cmd.Parameters.AddWithValue("@gc", pkb.GhiChu);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu phiếu khám bệnh: " + ex.Message);
                return false;
            }
        }

        // Nạp toàn bộ phiếu khám bệnh (kèm thông tin mô tả) cho ComboBox và dialog tìm kiếm
        public List<PhieuKhamBenhItem> GetAll()
        {
            var ds = new List<PhieuKhamBenhItem>();

            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                var sql = @"
                    SELECT  pkb.MaPhieuKhamBenh,
                            bn.HoTen,
                            kb.NgayKham,
                            lpk.TenLoaiPhongKham,
                            IFNULL(lb.TenLoaiBenh, '') AS TenLoaiBenh
                    FROM PHIEUKHAMBENH pkb
                    INNER JOIN BENHNHAN      bn  ON pkb.MaBenhNhan      = bn.MaBenhNhan
                    INNER JOIN KHAMBENH      kb  ON bn.MaKhamBenh       = kb.MaKhamBenh
                    INNER JOIN LOAIPHONGKHAM lpk ON kb.MaLoaiPhongKham  = lpk.MaLoaiPhongKham
                    LEFT  JOIN LOAIBENH      lb  ON pkb.MaLoaiBenh      = lb.MaLoaiBenh
                    ORDER BY pkb.MaPhieuKhamBenh ";

                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ds.Add(new PhieuKhamBenhItem
                    {
                        MaPhieuKhamBenh = reader.GetString("MaPhieuKhamBenh"),
                        HoTenBenhNhan = reader.GetString("HoTen"),
                        NgayKham = reader.GetDateTime("NgayKham"),
                        TenLoaiPhongKham = reader.GetString("TenLoaiPhongKham"),
                        TenLoaiBenh = reader.GetString("TenLoaiBenh")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách phiếu khám bệnh: " + ex.Message);
            }

            return ds;
        }
    }
}

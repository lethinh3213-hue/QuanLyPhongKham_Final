using MySql.Data.MySqlClient;
using QuanLyPhongKham.Helpers;
using QuanLyPhongKham.Models;
using System.Windows;

namespace QuanLyPhongKham.Repositories
{
    public class BaoCaoRepository
    {
        private readonly DBHelper db = new DBHelper();

        // Lập báo cáo hóa đơn theo tháng cho một loại phòng khám
        public List<BaoCaoHoaDonItem> LapBaoCaoHoaDon(int nam, int thang, string maLoaiPhongKham)
        {
            var ds = new List<BaoCaoHoaDonItem>();

            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                // Mỗi ngày có khám bệnh: đếm số bệnh nhân và cộng doanh thu từ hóa đơn
                string sql = @"
                    SELECT kb.NgayKham,
                           COUNT(DISTINCT bn.MaBenhNhan)                  AS SoBenhNhan,
                           CAST(IFNULL(SUM(hd.TongTien), 0) AS DECIMAL(18,2)) AS DoanhThu
                    FROM KHAMBENH kb
                    INNER JOIN BENHNHAN        bn  ON bn.MaKhamBenh      = kb.MaKhamBenh
                    LEFT  JOIN PHIEUKHAMBENH   pkb ON pkb.MaBenhNhan     = bn.MaBenhNhan
                    LEFT  JOIN HOADONTHANHTOAN hd  ON hd.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
                    WHERE YEAR(kb.NgayKham)  = @nam
                      AND MONTH(kb.NgayKham) = @thang
                      AND kb.MaLoaiPhongKham = @loai
                    GROUP BY kb.NgayKham
                    ORDER BY kb.NgayKham ";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nam", nam);
                cmd.Parameters.AddWithValue("@thang", thang);
                cmd.Parameters.AddWithValue("@loai", maLoaiPhongKham);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ds.Add(new BaoCaoHoaDonItem
                    {
                        Ngay = reader.GetDateTime("NgayKham"),
                        SoBenhNhan = Convert.ToInt32(reader["SoBenhNhan"]),
                        DoanhThu = Convert.ToDecimal(reader["DoanhThu"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lập báo cáo: " + ex.Message);
            }

            // Tổng doanh thu của loại phòng khám trong tháng
            decimal tongDoanhThu = ds.Sum(x => x.DoanhThu);

            // Đánh STT và tính tỉ lệ doanh thu từng ngày so với tổng
            int stt = 1;
            foreach (var item in ds)
            {
                item.STT = stt++;
                item.TiLe = tongDoanhThu > 0 ? (double)(item.DoanhThu / tongDoanhThu) * 100 : 0;
            }

            return ds;
        }

        // Lập báo cáo sử dụng thuốc theo tháng cho từng loại thuốc có dùng
        public List<BaoCaoThuocItem> LapBaoCaoThuoc(int nam, int thang)
        {
            var ds = new List<BaoCaoThuocItem>();

            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                // Gom theo từng loại thuốc có dùng trong tháng: lấy số lượng tồn và cộng số lượng đã dùng
                string sql = @"
                    SELECT lt.TenLoaiThuoc,
                           dvt.TenDonViTinh,
                           lt.SoLuongTon,
                           SUM(ct.SoLuong) AS SoLuongDung
                    FROM CTPHIEUKHAMBENH ct
                    INNER JOIN PHIEUKHAMBENH pkb ON ct.MaPhieuKhamBenh = pkb.MaPhieuKhamBenh
                    INNER JOIN BENHNHAN      bn  ON pkb.MaBenhNhan     = bn.MaBenhNhan
                    INNER JOIN KHAMBENH      kb  ON bn.MaKhamBenh      = kb.MaKhamBenh
                    INNER JOIN LOAITHUOC     lt  ON ct.MaThuoc         = lt.MaLoaiThuoc
                    LEFT  JOIN DONVITINH     dvt ON lt.MaDonViTinh     = dvt.MaDonViTinh
                    WHERE YEAR(kb.NgayKham)  = @nam
                      AND MONTH(kb.NgayKham) = @thang
                    GROUP BY lt.MaLoaiThuoc, lt.TenLoaiThuoc, dvt.TenDonViTinh, lt.SoLuongTon
                    ORDER BY lt.MaLoaiThuoc ";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nam", nam);
                cmd.Parameters.AddWithValue("@thang", thang);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ds.Add(new BaoCaoThuocItem
                    {
                        TenThuoc = reader.GetString("TenLoaiThuoc"),
                        DonViTinh = reader["TenDonViTinh"] == DBNull.Value ? "" : reader.GetString("TenDonViTinh"),
                        SoLuongTon = reader.GetInt32("SoLuongTon"),
                        SoLuongDung = Convert.ToInt32(reader["SoLuongDung"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lập báo cáo: " + ex.Message);
            }

            // Đánh STT
            int stt2 = 1;
            foreach (var item in ds)
                item.STT = stt2++;

            return ds;
        }
    }
}

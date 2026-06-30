using MySql.Data.MySqlClient;
using QuanLyPhongKham.Helpers;
using QuanLyPhongKham.Models;
using System.Windows;

namespace QuanLyPhongKham.Repositories
{
    public class HoaDonThanhToanRepository
    {
        private readonly DBHelper db = new DBHelper();

        // Tính hóa đơn thanh toán theo mã phiếu khám bệnh (trả về null nếu mã không tồn tại)
        public KetQuaHoaDon? TinhHoaDon(string maPhieuKhamBenh)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                // Lấy thông tin bệnh nhân, loại phòng khám và tiền khám theo mã phiếu
                var sqlInfo = @"
                    SELECT  bn.HoTen,
                            kb.NgayKham,
                            lpk.TenLoaiPhongKham,
                            lpk.TienKham
                    FROM PHIEUKHAMBENH pkb
                    INNER JOIN BENHNHAN      bn  ON pkb.MaBenhNhan      = bn.MaBenhNhan
                    INNER JOIN KHAMBENH      kb  ON bn.MaKhamBenh       = kb.MaKhamBenh
                    INNER JOIN LOAIPHONGKHAM lpk ON kb.MaLoaiPhongKham  = lpk.MaLoaiPhongKham
                    WHERE pkb.MaPhieuKhamBenh = @ma ";

                var kq = new KetQuaHoaDon { MaPhieuKhamBenh = maPhieuKhamBenh };

                using (var cmdInfo = new MySqlCommand(sqlInfo, conn))
                {
                    cmdInfo.Parameters.AddWithValue("@ma", maPhieuKhamBenh);
                    using var rd = cmdInfo.ExecuteReader();

                    // Mã phiếu không tồn tại trong CSDL
                    if (!rd.Read())
                        return null;

                    kq.HoTenBenhNhan = rd.GetString("HoTen");
                    kq.NgayKham = rd.GetDateTime("NgayKham");
                    kq.TenLoaiPhongKham = rd.GetString("TenLoaiPhongKham");
                    kq.TienKham = rd.GetDecimal("TienKham");
                }

                // Lấy danh sách thuốc đã kê (số lượng + đơn giá) của phiếu
                var sqlThuoc = @"
                    SELECT ct.SoLuong, lt.DonGia
                    FROM CTPHIEUKHAMBENH ct
                    INNER JOIN LOAITHUOC lt ON ct.MaThuoc = lt.MaLoaiThuoc
                    WHERE ct.MaPhieuKhamBenh = @ma ";

                decimal tongTienThuoc = 0;
                bool coDungThuoc = false;

                using (var cmdThuoc = new MySqlCommand(sqlThuoc, conn))
                {
                    cmdThuoc.Parameters.AddWithValue("@ma", maPhieuKhamBenh);
                    using var rd = cmdThuoc.ExecuteReader();

                    // Cộng dồn tiền thuốc từng loại = số lượng * đơn giá
                    while (rd.Read())
                    {
                        coDungThuoc = true;
                        int soLuong = rd.GetInt32("SoLuong");
                        decimal donGia = rd.GetDecimal("DonGia");
                        tongTienThuoc += soLuong * donGia;
                    }
                }

                // Không kê thuốc thì tiền thuốc = 0
                kq.CoDungThuoc = coDungThuoc;
                kq.TienThuoc = coDungThuoc ? tongTienThuoc : 0;

                // Tổng tiền = tiền khám + tiền thuốc
                kq.TongTien = kq.TienKham + kq.TienThuoc;

                return kq;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tính hóa đơn: " + ex.Message);
                return null;
            }
        }

        public bool DaCoHoaDon(string maPhieuKhamBenh)
        {
            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = "SELECT COUNT(*) FROM HOADONTHANHTOAN WHERE MaPhieuKhamBenh = @ma";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maPhieuKhamBenh);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kiểm tra hóa đơn: " + ex.Message);
                return false;
            }
        }

        // Lưu hóa đơn xuống CSDL (mỗi phiếu chỉ 1 hóa đơn)
        public string LuuHoaDon(HoaDonThanhToan hd)
        {
            try
            {
                if (DaCoHoaDon(hd.MaPhieuKhamBenh))
                {
                    MessageBox.Show("Phiếu khám bệnh này đã có hóa đơn thanh toán, không thể lập thêm.");
                    return string.Empty;
                }

                using var conn = db.GetConnection();
                conn.Open();

                string maMoi = SinhMaHoaDon(conn);
                using var cmdIns = new MySqlCommand(@"
                    INSERT INTO HOADONTHANHTOAN (MaHoaDon, MaPhieuKhamBenh, TienKham, TienThuoc, TongTien)
                    VALUES (@mhd, @ma, @tk, @tt, @tong)", conn);
                cmdIns.Parameters.AddWithValue("@mhd", maMoi);
                cmdIns.Parameters.AddWithValue("@ma", hd.MaPhieuKhamBenh);
                cmdIns.Parameters.AddWithValue("@tk", hd.TienKham);
                cmdIns.Parameters.AddWithValue("@tt", hd.TienThuoc);
                cmdIns.Parameters.AddWithValue("@tong", hd.TongTien);
                cmdIns.ExecuteNonQuery();
                return maMoi;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu hóa đơn: " + ex.Message);
                return string.Empty;
            }
        }

        // Sinh mã hóa đơn dạng HDxxx dựa trên mã lớn nhất hiện có
        private string SinhMaHoaDon(MySqlConnection conn)
        {
            using var cmd = new MySqlCommand(
                "SELECT MaHoaDon FROM HOADONTHANHTOAN ORDER BY MaHoaDon DESC LIMIT 1", conn);
            var obj = cmd.ExecuteScalar();

            int so = 0;
            if (obj != null)
            {
                string maCu = obj.ToString() ?? "";
                string phanSo = new string(maCu.Where(char.IsDigit).ToArray());
                int.TryParse(phanSo, out so);
            }
            return "HD" + (so + 1).ToString("D3");
        }
    }
}

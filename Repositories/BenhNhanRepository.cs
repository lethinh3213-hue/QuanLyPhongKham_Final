using MySql.Data.MySqlClient;
using QuanLyPhongKham.Helpers;
using QuanLyPhongKham.Models;
using System.Text;
using System.Windows;

namespace QuanLyPhongKham.Repositories
{
    public class BenhNhanRepository
    {
        private readonly DBHelper db = new DBHelper();

        public List<BenhNhan> GetByKhamBenh(string maKhamBenh)
        {
            List<BenhNhan> danhSach = new List<BenhNhan>();
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT MaBenhNhan, HoTen, GioiTinh, 
                                    NamSinh, DiaChi, MaKhamBenh 
                                    FROM BENHNHAN 
                                    WHERE MaKhamBenh = @Ma";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Ma", maKhamBenh);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                danhSach.Add(new BenhNhan
                                {
                                    MaBenhNhan = reader.GetString("MaBenhNhan"),
                                    HoTen = reader.GetString("HoTen"),
                                    GioiTinh = reader.GetString("GioiTinh"),
                                    NamSinh = reader.GetInt32("NamSinh"),
                                    DiaChi = reader.GetString("DiaChi"),
                                    MaKhamBenh = reader.GetString("MaKhamBenh")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách bệnh nhân: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return danhSach;
        }

        public bool ThemBenhNhan(BenhNhan bn)
        {
            try
            {
                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO BENHNHAN 
                                    (MaBenhNhan, HoTen, GioiTinh, NamSinh, DiaChi, MaKhamBenh) 
                                    VALUES (@Ma, @Ten, @GT, @NS, @DC, @KB)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Ma", bn.MaBenhNhan);
                        cmd.Parameters.AddWithValue("@Ten", bn.HoTen);
                        cmd.Parameters.AddWithValue("@GT", bn.GioiTinh);
                        cmd.Parameters.AddWithValue("@NS", bn.NamSinh);
                        cmd.Parameters.AddWithValue("@DC", bn.DiaChi);
                        cmd.Parameters.AddWithValue("@KB", bn.MaKhamBenh);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm bệnh nhân: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Lấy DS bệnh nhân theo ngày khám + loại phòng khám
        public List<BenhNhan> GetByNgayVaLoaiPhongKham(DateTime ngayKham, string maLoaiPhongKham)
        {
            var ds = new List<BenhNhan>();

            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                string sql = @"SELECT bn.* FROM BENHNHAN bn
                               INNER JOIN KHAMBENH kb ON bn.MaKhamBenh = kb.MaKhamBenh
                               WHERE kb.NgayKham = @ngay AND kb.MaLoaiPhongKham = @loai";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ngay", ngayKham.Date);
                cmd.Parameters.AddWithValue("@loai", maLoaiPhongKham);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ds.Add(new BenhNhan
                    {
                        MaBenhNhan = reader.GetString("MaBenhNhan"),
                        HoTen = reader.GetString("HoTen"),
                        GioiTinh = reader.GetString("GioiTinh"),
                        NamSinh = reader.GetInt32("NamSinh"),
                        DiaChi = reader.GetString("DiaChi"),
                        MaKhamBenh = reader.GetString("MaKhamBenh")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách bệnh nhân: " + ex.Message);
            }

            return ds;
        }

        // Tra cứu bệnh nhân theo các tiêu chí lọc
        public List<KetQuaTraCuuBenhNhan> TraCuu(TieuChiTraCuu tc)
        {
            var ds = new List<KetQuaTraCuuBenhNhan>();

            try
            {
                using var conn = db.GetConnection();
                conn.Open();

                // Câu SQL gốc: JOIN tất cả các bảng cần thiết
                var sql = new StringBuilder(@"
                    SELECT DISTINCT
                        bn.MaBenhNhan, bn.HoTen, bn.GioiTinh, bn.NamSinh, bn.DiaChi,
                        kb.MaKhamBenh, kb.NgayKham,
                        lpk.MaLoaiPhongKham, lpk.TenLoaiPhongKham,
                        IFNULL(pkb.MaPhieuKhamBenh, '') AS MaPhieuKhamBenh,
                        IFNULL(pkb.TrieuChung, '')      AS TrieuChung,
                        IFNULL(pkb.GhiChu, '')          AS GhiChu,
                        IFNULL(lb.MaLoaiBenh, '')       AS MaLoaiBenh,
                        IFNULL(lb.TenLoaiBenh, '')      AS TenLoaiBenh,
                        IFNULL(ct.MaThuoc, '')          AS MaThuoc,
                        IFNULL(lt.TenLoaiThuoc, '')     AS TenLoaiThuoc
                    FROM BENHNHAN bn
                    INNER JOIN KHAMBENH       kb  ON bn.MaKhamBenh        = kb.MaKhamBenh
                    INNER JOIN LOAIPHONGKHAM  lpk ON kb.MaLoaiPhongKham   = lpk.MaLoaiPhongKham
                    LEFT  JOIN PHIEUKHAMBENH  pkb ON bn.MaBenhNhan        = pkb.MaBenhNhan
                    LEFT  JOIN LOAIBENH       lb  ON pkb.MaLoaiBenh       = lb.MaLoaiBenh
                    LEFT  JOIN CTPHIEUKHAMBENH ct ON pkb.MaPhieuKhamBenh  = ct.MaPhieuKhamBenh
                    LEFT  JOIN LOAITHUOC      lt  ON ct.MaThuoc           = lt.MaLoaiThuoc
                    WHERE 1=1 ");

                using var cmd = new MySqlCommand();
                cmd.Connection = conn;

                // Lọc bệnh nhân trực tiếp
                if (!string.IsNullOrWhiteSpace(tc.MaBenhNhan))
                {
                    sql.Append(" AND bn.MaBenhNhan LIKE @MaBenhNhan ");
                    cmd.Parameters.AddWithValue("@MaBenhNhan", "%" + tc.MaBenhNhan + "%");
                }
                if (!string.IsNullOrWhiteSpace(tc.HoTen))
                {
                    sql.Append(" AND bn.HoTen LIKE @HoTen ");
                    cmd.Parameters.AddWithValue("@HoTen", "%" + tc.HoTen + "%");
                }
                if (!string.IsNullOrWhiteSpace(tc.GioiTinh))
                {
                    sql.Append(" AND bn.GioiTinh = @GioiTinh ");
                    cmd.Parameters.AddWithValue("@GioiTinh", tc.GioiTinh);
                }
                if (!string.IsNullOrWhiteSpace(tc.DiaChi))
                {
                    sql.Append(" AND bn.DiaChi LIKE @DiaChi ");
                    cmd.Parameters.AddWithValue("@DiaChi", "%" + tc.DiaChi + "%");
                }
                if (tc.NamSinhTu.HasValue)
                {
                    sql.Append(" AND bn.NamSinh >= @NamSinhTu ");
                    cmd.Parameters.AddWithValue("@NamSinhTu", tc.NamSinhTu.Value);
                }
                if (tc.NamSinhDen.HasValue)
                {
                    sql.Append(" AND bn.NamSinh <= @NamSinhDen ");
                    cmd.Parameters.AddWithValue("@NamSinhDen", tc.NamSinhDen.Value);
                }

                // Lọc khám bệnh & phòng khám
                if (tc.NgayKhamTu.HasValue)
                {
                    sql.Append(" AND kb.NgayKham >= @NgayKhamTu ");
                    cmd.Parameters.AddWithValue("@NgayKhamTu", tc.NgayKhamTu.Value.Date);
                }
                if (tc.NgayKhamDen.HasValue)
                {
                    sql.Append(" AND kb.NgayKham <= @NgayKhamDen ");
                    cmd.Parameters.AddWithValue("@NgayKhamDen", tc.NgayKhamDen.Value.Date);
                }
                if (!string.IsNullOrWhiteSpace(tc.MaLoaiPhongKham))
                {
                    sql.Append(" AND kb.MaLoaiPhongKham = @MaLoaiPhongKham ");
                    cmd.Parameters.AddWithValue("@MaLoaiPhongKham", tc.MaLoaiPhongKham);
                }

                // Lọc phiếu khám bệnh
                if (!string.IsNullOrWhiteSpace(tc.MaPhieuKhamBenh))
                {
                    sql.Append(" AND pkb.MaPhieuKhamBenh LIKE @MaPhieuKhamBenh ");
                    cmd.Parameters.AddWithValue("@MaPhieuKhamBenh", "%" + tc.MaPhieuKhamBenh + "%");
                }
                if (!string.IsNullOrWhiteSpace(tc.TrieuChung))
                {
                    sql.Append(" AND pkb.TrieuChung LIKE @TrieuChung ");
                    cmd.Parameters.AddWithValue("@TrieuChung", "%" + tc.TrieuChung + "%");
                }
                if (!string.IsNullOrWhiteSpace(tc.MaLoaiBenh))
                {
                    sql.Append(" AND pkb.MaLoaiBenh = @MaLoaiBenh ");
                    cmd.Parameters.AddWithValue("@MaLoaiBenh", tc.MaLoaiBenh);
                }

                // Lọc thuốc
                if (!string.IsNullOrWhiteSpace(tc.MaLoaiThuoc))
                {
                    sql.Append(" AND ct.MaThuoc = @MaLoaiThuoc ");
                    cmd.Parameters.AddWithValue("@MaLoaiThuoc", tc.MaLoaiThuoc);
                }
                if (!string.IsNullOrWhiteSpace(tc.MaDonViTinh))
                {
                    sql.Append(" AND lt.MaDonViTinh = @MaDonViTinh ");
                    cmd.Parameters.AddWithValue("@MaDonViTinh", tc.MaDonViTinh);
                }
                if (!string.IsNullOrWhiteSpace(tc.MaCachDung))
                {
                    sql.Append(" AND lt.MaCachDung = @MaCachDung ");
                    cmd.Parameters.AddWithValue("@MaCachDung", tc.MaCachDung);
                }
                if (tc.SoLuongKeTu.HasValue)
                {
                    sql.Append(" AND ct.SoLuong >= @SoLuongKeTu ");
                    cmd.Parameters.AddWithValue("@SoLuongKeTu", tc.SoLuongKeTu.Value);
                }
                if (tc.SoLuongKeDen.HasValue)
                {
                    sql.Append(" AND ct.SoLuong <= @SoLuongKeDen ");
                    cmd.Parameters.AddWithValue("@SoLuongKeDen", tc.SoLuongKeDen.Value);
                }

                sql.Append(" ORDER BY bn.MaBenhNhan ");

                cmd.CommandText = sql.ToString();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ds.Add(new KetQuaTraCuuBenhNhan
                    {
                        MaBenhNhan = reader.GetString("MaBenhNhan"),
                        HoTen = reader.GetString("HoTen"),
                        GioiTinh = reader.GetString("GioiTinh"),
                        NamSinh = reader.GetInt32("NamSinh"),
                        DiaChi = reader.GetString("DiaChi"),
                        MaKhamBenh = reader.GetString("MaKhamBenh"),
                        NgayKham = reader.GetDateTime("NgayKham"),
                        MaLoaiPhongKham = reader.GetString("MaLoaiPhongKham"),
                        TenLoaiPhongKham = reader.GetString("TenLoaiPhongKham"),
                        MaPhieuKhamBenh = reader.GetString("MaPhieuKhamBenh"),
                        TrieuChung = reader.GetString("TrieuChung"),
                        GhiChu = reader.GetString("GhiChu"),
                        MaLoaiBenh = reader.GetString("MaLoaiBenh"),
                        TenLoaiBenh = reader.GetString("TenLoaiBenh"),
                        MaThuoc = reader.GetString("MaThuoc"),
                        TenLoaiThuoc = reader.GetString("TenLoaiThuoc")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tra cứu bệnh nhân: " + ex.Message);
            }

            return ds;
        }
    }
}

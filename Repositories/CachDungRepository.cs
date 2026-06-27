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
    }
}

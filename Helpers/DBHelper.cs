using MySql.Data.MySqlClient;

namespace QuanLyPhongKham.Helpers
{
    public class DBHelper
    {
        // Nhớ đổi Pwd thành mật khẩu MySQL của máy bạn
        private readonly string connStr = "Server=localhost;Database=QUANLYPHONGKHAM;Uid=root;Pwd=123456789;";

        public MySqlConnection GetConnection() => new MySqlConnection(connStr);
    }
}

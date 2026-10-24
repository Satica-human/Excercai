using MySql.Data.MySqlClient;

namespace QuanLySinhVien_.DataAccess;

/// <summary>
/// Tạo kết nối MySQL dùng chung cho ứng dụng.
/// Cập nhật tài khoản và mật khẩu tại đây trước khi kết nối.
/// </summary>
public static class MySqlConnectionFactory
{
    private const string ConnectionString =
        "Server=127.0.0.1;Port=3306;Database=QLSV;User ID=TEN_TAI_KHOAN;Password=MAT_KHAU;";

    public static MySqlConnection CreateConnection() => new(ConnectionString);

    public static async Task<string> TestConnectionAsync()
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var command = new MySqlCommand("SELECT VERSION();", connection);
        object? version = await command.ExecuteScalarAsync();
        return version?.ToString() ?? "Đã kết nối MySQL.";
    }
}

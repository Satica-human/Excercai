using MySql.Data.MySqlClient;

namespace QuanLySinhVien_.DataAccess;

public sealed class LopHocDataAccess
{
    public List<LopHoc> LayDanhSach()
    {
        using var connection = MySqlConnectionFactory.CreateConnection();
        connection.Open();
        using var command = new MySqlCommand(
            "SELECT `MaLop`, `TenLop` FROM `LopHoc` ORDER BY `MaLop`", connection);
        using var reader = command.ExecuteReader();

        var lopHocs = new List<LopHoc>();
        while (reader.Read())
        {
            lopHocs.Add(new LopHoc
            {
                MaLop = reader["MaLop"]?.ToString() ?? string.Empty,
                TenLop = reader["TenLop"]?.ToString() ?? string.Empty
            });
        }

        return lopHocs;
    }

    public LopHoc? TimTheoMa(string maLop)
    {
        using var connection = MySqlConnectionFactory.CreateConnection();
        connection.Open();
        using var command = new MySqlCommand(
            "SELECT `MaLop`, `TenLop` FROM `LopHoc` WHERE `MaLop` = @MaLop LIMIT 1", connection);
        command.Parameters.AddWithValue("@MaLop", maLop);
        using var reader = command.ExecuteReader();

        if (!reader.Read())
            return null;

        return new LopHoc
        {
            MaLop = reader["MaLop"]?.ToString() ?? string.Empty,
            TenLop = reader["TenLop"]?.ToString() ?? string.Empty
        };
    }

    public void Them(LopHoc lop)
    {
        ArgumentNullException.ThrowIfNull(lop);
        using var connection = MySqlConnectionFactory.CreateConnection();
        connection.Open();
        using var command = new MySqlCommand(
            "INSERT INTO `LopHoc` (`MaLop`, `TenLop`) VALUES (@MaLop, @TenLop)", connection);
        command.Parameters.AddWithValue("@MaLop", lop.MaLop);
        command.Parameters.AddWithValue("@TenLop", lop.TenLop);

        if (command.ExecuteNonQuery() != 1)
            throw new InvalidOperationException("Không thêm được lớp học vào cơ sở dữ liệu.");
    }

    public void Sua(LopHoc lop)
    {
        ArgumentNullException.ThrowIfNull(lop);
        using var connection = MySqlConnectionFactory.CreateConnection();
        connection.Open();
        using var command = new MySqlCommand(
            "UPDATE `LopHoc` SET `TenLop` = @TenLop WHERE `MaLop` = @MaLop", connection);
        command.Parameters.AddWithValue("@MaLop", lop.MaLop);
        command.Parameters.AddWithValue("@TenLop", lop.TenLop);
        int affected = command.ExecuteNonQuery();

        if (affected == 0 && !TonTai(lop.MaLop))
            throw new KeyNotFoundException($"Không tìm thấy lớp {lop.MaLop} để sửa.");
    }

    public void Xoa(string maLop)
    {
        using var connection = MySqlConnectionFactory.CreateConnection();
        connection.Open();
        using var command = new MySqlCommand(
            "DELETE FROM `LopHoc` WHERE `MaLop` = @MaLop", connection);
        command.Parameters.AddWithValue("@MaLop", maLop);

        if (command.ExecuteNonQuery() == 0)
            throw new KeyNotFoundException($"Không tìm thấy lớp {maLop} để xóa.");
    }

    private static bool TonTai(string maLop)
    {
        using var connection = MySqlConnectionFactory.CreateConnection();
        connection.Open();
        using var command = new MySqlCommand(
            "SELECT 1 FROM `LopHoc` WHERE `MaLop` = @MaLop LIMIT 1", connection);
        command.Parameters.AddWithValue("@MaLop", maLop);
        return command.ExecuteScalar() is not null;
    }
}

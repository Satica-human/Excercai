using MySql.Data.MySqlClient;
using QuanLySinhVien_;

namespace QuanLySinhVien_.DataAccess;

/// <summary>MySQL data access for student records.</summary>
public sealed class SinhVienDAL
{
    public List<SinhVien> getAllSinhVien1() => GetAllSinhVien();

    public List<SinhVien> GetAllSinhVien() =>
        ReadStudents("SELECT * FROM `SinhVien`");

    public List<SinhVien> GetAllSinhVien1() => GetAllSinhVien();

    public SinhVien? GetSinhVienByMaSV(string maSV) =>
        ReadStudents("SELECT * FROM `SinhVien` WHERE `MaSV` = @MaSV LIMIT 1", ("@MaSV", maSV))
            .FirstOrDefault();

    public List<SinhVien> GetSinhViensByMaLop(string maLop) =>
        ReadStudents("SELECT * FROM `SinhVien` WHERE `MaLop` = @MaLop", ("@MaLop", maLop));

    public void AddSinhVien(SinhVien sv)
    {
        ArgumentNullException.ThrowIfNull(sv);

        HashSet<string> columns = GetStudentColumns();
        string phoneColumn = GetPhoneColumn(columns);
        bool hasStatusColumn = columns.Contains("TrangThai");

        var insertColumns = new List<string>
        {
            "MaSV", "MaLop", "HoTen", "GioiTinh", "NgaySinh", "Diem", phoneColumn, "Email"
        };
        var parameterNames = new List<string>
        {
            "@MaSV", "@MaLop", "@HoTen", "@GioiTinh", "@NgaySinh", "@Diem", "@DienThoai", "@Email"
        };

        if (hasStatusColumn)
        {
            insertColumns.Add("TrangThai");
            parameterNames.Add("@TrangThai");
        }

        string sql = $"INSERT INTO `SinhVien` ({string.Join(", ", insertColumns.Select(QuoteIdentifier))}) " +
                     $"VALUES ({string.Join(", ", parameterNames)})";

        int inserted = ExecuteCommand(sql, BuildStudentParameters(sv, hasStatusColumn).ToArray());
        if (inserted != 1)
            throw new InvalidOperationException($"Không thêm được sinh viên {sv.MaSV}.");
    }

    public void UpdateSinhVien(SinhVien sv)
    {
        ArgumentNullException.ThrowIfNull(sv);

        HashSet<string> columns = GetStudentColumns();
        string phoneColumn = GetPhoneColumn(columns);
        bool hasStatusColumn = columns.Contains("TrangThai");

        var assignments = new List<string>
        {
            "`MaLop` = @MaLop",
            "`HoTen` = @HoTen",
            "`GioiTinh` = @GioiTinh",
            "`NgaySinh` = @NgaySinh",
            "`Diem` = @Diem",
            $"{QuoteIdentifier(phoneColumn)} = @DienThoai",
            "`Email` = @Email"
        };

        if (hasStatusColumn)
            assignments.Add("`TrangThai` = @TrangThai");

        string sql = $"UPDATE `SinhVien` SET {string.Join(", ", assignments)} WHERE `MaSV` = @MaSV";
        int updated = ExecuteCommand(sql, BuildStudentParameters(sv, hasStatusColumn).ToArray());
        if (updated == 0 && GetSinhVienByMaSV(sv.MaSV) is null)
            throw new KeyNotFoundException($"Không tìm thấy sinh viên {sv.MaSV} để sửa.");
    }

    public void DeleteSinhVien(string maSV)
    {
        int deleted = ExecuteCommand(
            "DELETE FROM `SinhVien` WHERE `MaSV` = @MaSV",
            ("@MaSV", maSV));
        if (deleted == 0)
            throw new KeyNotFoundException($"Không tìm thấy sinh viên {maSV} để xóa.");
    }

    public SinhVien? GetSinhVienById(string maSV) => GetSinhVienByMaSV(maSV);

    private static List<SinhVien> ReadStudents(
        string sql,
        params (string Name, object Value)[] parameters)
    {
        using var connection = MySqlConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new MySqlCommand(sql, connection);
        AddParameters(command, parameters);

        using var reader = command.ExecuteReader();
        var students = new List<SinhVien>();
        while (reader.Read())
            students.Add(MapStudent(reader));

        return students;
    }

    private static int ExecuteCommand(
        string sql,
        params (string Name, object Value)[] parameters)
    {
        using var connection = MySqlConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new MySqlCommand(sql, connection);
        AddParameters(command, parameters);
        return command.ExecuteNonQuery();
    }

    private static void AddParameters(
        MySqlCommand command,
        IEnumerable<(string Name, object Value)> parameters)
    {
        foreach ((string name, object value) in parameters)
            command.Parameters.AddWithValue(name, value);
    }

    private static List<(string Name, object Value)> BuildStudentParameters(
        SinhVien sv,
        bool includeStatus)
    {
        var parameters = new List<(string Name, object Value)>
        {
            ("@MaSV", sv.MaSV),
            ("@MaLop", sv.MaLop),
            ("@HoTen", sv.HoTen),
            ("@GioiTinh", sv.GioiTinh),
            ("@NgaySinh", sv.NgaySinh),
            ("@Diem", sv.Diem),
            ("@DienThoai", sv.DienThoai),
            ("@Email", sv.Email)
        };

        if (includeStatus)
            parameters.Add(("@TrangThai", sv.TrangThai));

        return parameters;
    }

    private static HashSet<string> GetStudentColumns()
    {
        using var connection = MySqlConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new MySqlCommand("SELECT * FROM `SinhVien` LIMIT 0", connection);
        using var reader = command.ExecuteReader();

        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < reader.FieldCount; i++)
            columns.Add(reader.GetName(i));

        return columns;
    }

    private static string GetPhoneColumn(HashSet<string> columns)
    {
        if (columns.Contains("SoDienThoai"))
            return "SoDienThoai";
        if (columns.Contains("DienThoai"))
            return "DienThoai";

        throw new InvalidOperationException(
            "Bảng SinhVien cần có cột SoDienThoai hoặc DienThoai.");
    }

    private static SinhVien MapStudent(MySqlDataReader reader)
    {
        int statusOrdinal = FindOrdinal(reader, "TrangThai");

        return new SinhVien
        {
            MaSV = ReadString(reader, "MaSV"),
            HoTen = ReadString(reader, "HoTen"),
            GioiTinh = ReadString(reader, "GioiTinh"),
            NgaySinh = Convert.ToDateTime(ReadValue(reader, "NgaySinh")),
            DienThoai = ReadString(reader, "DienThoai", "SoDienThoai"),
            MaLop = ReadString(reader, "MaLop"),
            Diem = Convert.ToDecimal(ReadValue(reader, "Diem")),
            Email = ReadString(reader, "Email"),
            TrangThai = statusOrdinal < 0 ? "Đang học" : ReadString(reader, statusOrdinal)
        };
    }

    private static string ReadString(MySqlDataReader reader, params string[] columnNames)
    {
        int ordinal = FindOrdinal(reader, columnNames);
        if (ordinal < 0)
            throw new InvalidOperationException(
                $"Không tìm thấy cột {string.Join("/", columnNames)} trong bảng SinhVien.");

        return ReadString(reader, ordinal);
    }

    private static string ReadString(MySqlDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal)
            ? string.Empty
            : Convert.ToString(reader.GetValue(ordinal)) ?? string.Empty;
    }

    private static object ReadValue(MySqlDataReader reader, string columnName)
    {
        int ordinal = FindOrdinal(reader, columnName);
        if (ordinal < 0 || reader.IsDBNull(ordinal))
            throw new InvalidOperationException($"Thiếu dữ liệu ở cột {columnName} trong bảng SinhVien.");

        return reader.GetValue(ordinal);
    }

    private static int FindOrdinal(MySqlDataReader reader, params string[] columnNames)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (columnNames.Any(name =>
                    string.Equals(reader.GetName(i), name, StringComparison.OrdinalIgnoreCase)))
                return i;
        }

        return -1;
    }

    private static string QuoteIdentifier(string identifier) => $"`{identifier}`";
}

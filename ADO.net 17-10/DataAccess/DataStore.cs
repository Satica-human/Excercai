using System.Text.Json;

namespace QuanLySinhVien_.DataAccess;

/// <summary>DataList dùng trong ứng dụng, tự nạp và lưu vào file JSON cục bộ.</summary>
internal static class DataStore
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "QuanLySinhVien", "data.json");

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private static readonly StoredData InitialData = NapDuLieu();

    internal static readonly List<LopHoc> LopHocs = InitialData.LopHocs
        .Select(x => new LopHoc { MaLop = x.MaLop, TenLop = x.TenLop }).ToList();

    internal static readonly List<SinhVien> SinhViens = InitialData.SinhViens
        .Select(x => new SinhVien
        {
            MaSV = x.MaSV, HoTen = x.HoTen, NgaySinh = x.NgaySinh, GioiTinh = x.GioiTinh,
            Email = x.Email, DienThoai = x.DienThoai, Diem = x.Diem,
            TrangThai = x.TrangThai, MaLop = x.MaLop
        }).ToList();

    internal static void LuuDuLieu()
    {
        var data = new StoredData
        {
            LopHocs = LopHocs.Select(x => new StoredLopHoc { MaLop = x.MaLop, TenLop = x.TenLop }).ToList(),
            SinhViens = SinhViens.Select(x => new StoredSinhVien
            {
                MaSV = x.MaSV, HoTen = x.HoTen, NgaySinh = x.NgaySinh, GioiTinh = x.GioiTinh,
                Email = x.Email, DienThoai = x.DienThoai, Diem = x.Diem,
                TrangThai = x.TrangThai, MaLop = x.MaLop
            }).ToList()
        };

        string? tempPath = null;
        try
        {
            string? directory = Path.GetDirectoryName(FilePath);
            if (directory is null) throw new IOException("Không xác định được thư mục lưu dữ liệu.");
            Directory.CreateDirectory(directory);
            tempPath = FilePath + ".tmp";
            File.WriteAllText(tempPath, JsonSerializer.Serialize(data, JsonOptions));
            File.Move(tempPath, FilePath, overwrite: true);
        }
        finally
        {
            if (tempPath is not null && File.Exists(tempPath)) File.Delete(tempPath);
        }
    }

    private static StoredData NapDuLieu()
    {
        if (File.Exists(FilePath))
        {
            try
            {
                var saved = JsonSerializer.Deserialize<StoredData>(File.ReadAllText(FilePath), JsonOptions);
                if (saved is not null && saved.LopHocs is not null && saved.SinhViens is not null)
                    return saved;
                throw new InvalidDataException("File dữ liệu không có nội dung hợp lệ.");
            }
            catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
            {
                throw new InvalidDataException($"Không đọc được dữ liệu sinh viên tại:\n{FilePath}\n\n{ex.Message}", ex);
            }
        }

        var data = new StoredData
        {
            LopHocs = new()
            {
                new() { MaLop = "CNTT1", TenLop = "Công nghệ thông tin 1" },
                new() { MaLop = "CNTT2", TenLop = "Công nghệ thông tin 2" },
                new() { MaLop = "QTKD1", TenLop = "Quản trị kinh doanh 1" }
            },
            SinhViens = new()
            {
                new() { MaSV="SV000001", HoTen="Nguyễn Minh An", NgaySinh=new DateTime(2004,3,12), GioiTinh="Nam", Email="an@example.com", DienThoai="0912345678", Diem=8.5m, TrangThai="Đang học", MaLop="CNTT1" },
                new() { MaSV="SV000002", HoTen="Trần Thu Hà", NgaySinh=new DateTime(2004,8,24), GioiTinh="Nữ", Email="ha@example.com", DienThoai="0987654321", Diem=9m, TrangThai="Đang học", MaLop="CNTT1" },
                new() { MaSV="SV000003", HoTen="Lê Quốc Bảo", NgaySinh=new DateTime(2003,11,5), GioiTinh="Nam", Email="bao@example.com", DienThoai="0901234567", Diem=7.5m, TrangThai="Đang học", MaLop="CNTT2" }
            }
        };

        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(data, JsonOptions));
        return data;
    }

    private sealed class StoredData
    {
        public List<StoredLopHoc> LopHocs { get; set; } = new();
        public List<StoredSinhVien> SinhViens { get; set; } = new();
    }

    private sealed class StoredLopHoc
    {
        public string MaLop { get; set; } = string.Empty;
        public string TenLop { get; set; } = string.Empty;
    }

    private sealed class StoredSinhVien
    {
        public string MaSV { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
        public decimal Diem { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public string MaLop { get; set; } = string.Empty;
    }
}

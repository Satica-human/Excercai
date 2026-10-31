# Entity Framework Core Code First

Thư mục này là ví dụ Code First ánh xạ hai entity `SinhVien` và `LopHoc` sang MySQL.

## Cấu hình kết nối

Mở `QuanLySinhVienDbContext.cs` và thay `TEN_TAI_KHOAN`, `MAT_KHAU` trong connection string bằng thông tin MySQL của bạn. Đảm bảo MySQL đang chạy.

## Tạo migration và bảng

Mở terminal tại thư mục này và chạy:

```bash
dotnet tool install --global dotnet-ef --version 9.0.0
dotnet restore
dotnet ef migrations add InitialCreate --project QuanLySinhVien.CodeFirst.csproj
dotnet ef database update --project QuanLySinhVien.CodeFirst.csproj
```

EF Core sẽ tạo migration từ model trong `QuanLySinhVienDbContext` và áp dụng schema vào database `QLSV`.

Quan hệ được ánh xạ: một lớp học có nhiều sinh viên; `SinhVien.MaLop` là khóa ngoại tham chiếu `LopHoc.MaLop`.

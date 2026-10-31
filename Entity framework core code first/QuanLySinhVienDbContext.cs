using Microsoft.EntityFrameworkCore;

namespace QuanLySinhVien_.DataAccess;

/// <summary>
/// DbContext Code First: EF Core dùng hai entity SinhVien và LopHoc để tạo schema MySQL.
/// </summary>
public sealed class QuanLySinhVienDbContext : DbContext
{
    public DbSet<SinhVien> SinhViens => Set<SinhVien>();
    public DbSet<LopHoc> LopHocs => Set<LopHoc>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySQL(
                "Server=127.0.0.1;Port=3306;Database=QLSV;User ID=TEN_TAI_KHOAN;Password=MAT_KHAU;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LopHoc>(entity =>
        {
            entity.ToTable("LopHoc");
            entity.HasKey(x => x.MaLop);
            entity.Property(x => x.MaLop).HasMaxLength(20).IsRequired();
            entity.Property(x => x.TenLop).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.ToTable("SinhVien");
            entity.HasKey(x => x.MaSV);
            entity.Property(x => x.MaSV).HasMaxLength(8).IsRequired();
            entity.Property(x => x.HoTen).HasMaxLength(100).IsRequired();
            entity.Property(x => x.GioiTinh).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(254).IsRequired();
            entity.Property(x => x.DienThoai).HasMaxLength(11).IsRequired();
            entity.Property(x => x.Diem).HasPrecision(4, 2);
            entity.Property(x => x.TrangThai).HasMaxLength(50).IsRequired();
            entity.Property(x => x.MaLop).HasMaxLength(20).IsRequired();

            entity.HasOne(x => x.LopHoc)
                .WithMany(x => x.DanhSachSinhVien)
                .HasForeignKey(x => x.MaLop)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}


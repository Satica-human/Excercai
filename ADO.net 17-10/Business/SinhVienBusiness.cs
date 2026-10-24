using System.ComponentModel.DataAnnotations;
using QuanLySinhVien_.DataAccess;

namespace QuanLySinhVien_.Business;

public sealed class SinhVienBusiness
{
    private readonly SinhVienDataAccess _data = new();
    private readonly LopHocDataAccess _lopData = new();

    public List<LopHoc> LayDanhSachLop() => _lopData.LayDanhSach();

    public List<SinhVien> LayDanhSachTheoLop(string maLop) => _data.LayTheoLop(maLop);

    public SinhVien? TimTheoMa(string ma) => _data.TimTheoMa(ma);

    public void Them(SinhVien sv)
    {
        GanVaKiemTra(sv);
        if (_data.TimTheoMa(sv.MaSV) is not null)
            throw new InvalidOperationException($"Mã sinh viên {sv.MaSV} đã tồn tại.");

        _data.Them(sv);
    }

    public void Sua(SinhVien sv)
    {
        GanVaKiemTra(sv);
        if (_data.TimTheoMa(sv.MaSV) is null)
            throw new KeyNotFoundException($"Không tìm thấy sinh viên {sv.MaSV} để sửa.");

        _data.Sua(sv);
    }

    public void Xoa(string ma) => _data.Xoa(ma);

    private void GanVaKiemTra(SinhVien sv)
    {
        ArgumentNullException.ThrowIfNull(sv);
        sv.LopHoc = _lopData.TimTheoMa(sv.MaLop)
            ?? throw new ValidationException($"Không tìm thấy lớp {sv.MaLop}.");

        if (!sv.KiemTraHopLe(out List<string> errors))
            throw new ValidationException(string.Join(Environment.NewLine, errors));
    }
}

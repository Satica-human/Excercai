using System.ComponentModel.DataAnnotations;
using QuanLySinhVien_.DataAccess;

namespace QuanLySinhVien_.Business;

public sealed class LopHocBusiness
{
    private readonly LopHocDataAccess _data = new();
    private readonly SinhVienDataAccess _sinhVienData = new();

    public List<LopHoc> LayDanhSach() => _data.LayDanhSach();

    public LopHoc? TimTheoMa(string ma) => _data.TimTheoMa(ma);

    public void Them(LopHoc lop)
    {
        KiemTra(lop);
        if (_data.TimTheoMa(lop.MaLop) is not null)
            throw new InvalidOperationException($"Mã lớp {lop.MaLop} đã tồn tại.");

        _data.Them(lop);
    }

    public void Sua(LopHoc lop)
    {
        KiemTra(lop);
        if (_data.TimTheoMa(lop.MaLop) is null)
            throw new KeyNotFoundException($"Không tìm thấy lớp {lop.MaLop} để sửa.");

        _data.Sua(lop);
    }

    public void Xoa(string ma)
    {
        if (_sinhVienData.LayTheoLop(ma).Count > 0)
            throw new InvalidOperationException("Không thể xóa lớp đang có sinh viên.");

        _data.Xoa(ma);
    }

    private static void KiemTra(LopHoc lop)
    {
        ArgumentNullException.ThrowIfNull(lop);
        if (!lop.KiemTraHopLe(out List<string> errors))
            throw new ValidationException(string.Join(Environment.NewLine, errors));
    }
}

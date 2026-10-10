using QuanLySinhVien_.DataAccess;

namespace QuanLySinhVien_.Business;

public sealed class SinhVienBusiness
{
    private readonly SinhVienDataAccess _data = new();
    private readonly LopHocDataAccess _lopData = new();
    public List<LopHoc> LayDanhSachLop() => _lopData.LayDanhSach();
    public List<SinhVien> LayDanhSachTheoLop(string maLop) => _data.LayTheoLop(maLop);
    public SinhVien? TimTheoMa(string ma) => _data.TimTheoMa(ma);
    public bool Them(SinhVien sv, out string loi) => Luu(sv, true, out loi);
    public bool Sua(SinhVien sv, out string loi) => Luu(sv, false, out loi);
    private bool Luu(SinhVien sv, bool them, out string loi)
    {
        sv.LopHoc = _lopData.TimTheoMa(sv.MaLop);
        if (!sv.KiemTraHopLe(out var errors)) { loi=string.Join("\n",errors); return false; }
        bool ok=them?_data.Them(sv):_data.Sua(sv);
        loi=ok?"":them?"Mã sinh viên đã tồn tại.":"Không tìm thấy sinh viên.";
        return ok;
    }
    public bool Xoa(string ma) => _data.Xoa(ma);
}

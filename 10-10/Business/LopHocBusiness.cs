using QuanLySinhVien_.DataAccess;

namespace QuanLySinhVien_.Business;

public sealed class LopHocBusiness
{
    private readonly LopHocDataAccess _data = new();
    public List<LopHoc> LayDanhSach() => _data.LayDanhSach();
    public LopHoc? TimTheoMa(string ma) => _data.TimTheoMa(ma);
    public bool Them(LopHoc lop, out string loi) { if(!lop.KiemTraHopLe(out var e)){loi=string.Join("\n",e);return false;} bool ok=_data.Them(lop); loi=ok?"":"Mã lớp đã tồn tại.";return ok; }
    public bool Sua(LopHoc lop, out string loi) { if(!lop.KiemTraHopLe(out var e)){loi=string.Join("\n",e);return false;} bool ok=_data.Sua(lop);loi=ok?"":"Không tìm thấy lớp.";return ok; }
    public bool Xoa(string ma) { if(new SinhVienDataAccess().LayTheoLop(ma).Count>0)return false;return _data.Xoa(ma); }
}

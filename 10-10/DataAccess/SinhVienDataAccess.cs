namespace QuanLySinhVien_.DataAccess;

public sealed class SinhVienDataAccess
{
    public List<SinhVien> LayDanhSach() => DataStore.SinhViens.ToList();
    public SinhVien? TimTheoMa(string maSV) => DataStore.SinhViens.FirstOrDefault(x => x.MaSV.Equals(maSV, StringComparison.OrdinalIgnoreCase));
    public List<SinhVien> LayTheoLop(string maLop) => DataStore.SinhViens.Where(x => x.MaLop.Equals(maLop, StringComparison.OrdinalIgnoreCase)).ToList();
    public bool Them(SinhVien sv)
    {
        if (TimTheoMa(sv.MaSV) != null) return false;
        DataStore.SinhViens.Add(sv);
        try { DataStore.LuuDuLieu(); return true; }
        catch { DataStore.SinhViens.Remove(sv); throw; }
    }
    public bool Sua(SinhVien sv)
    {
        var cu=TimTheoMa(sv.MaSV); if(cu==null)return false;
        var banSao = new SinhVien { MaSV=cu.MaSV, HoTen=cu.HoTen, NgaySinh=cu.NgaySinh, GioiTinh=cu.GioiTinh, Email=cu.Email, DienThoai=cu.DienThoai, Diem=cu.Diem, TrangThai=cu.TrangThai, MaLop=cu.MaLop, LopHoc=cu.LopHoc };
        CapNhat(cu,sv);
        try { DataStore.LuuDuLieu(); return true; }
        catch { CapNhat(cu,banSao); throw; }
    }
    public bool Xoa(string maSV)
    {
        var sv=TimTheoMa(maSV); if(sv==null)return false;
        DataStore.SinhViens.Remove(sv);
        try { DataStore.LuuDuLieu(); return true; }
        catch { DataStore.SinhViens.Add(sv); throw; }
    }
    private static void CapNhat(SinhVien d, SinhVien s) { d.HoTen=s.HoTen; d.NgaySinh=s.NgaySinh; d.GioiTinh=s.GioiTinh; d.Email=s.Email; d.DienThoai=s.DienThoai; d.Diem=s.Diem; d.TrangThai=s.TrangThai; d.MaLop=s.MaLop; d.LopHoc=s.LopHoc; }
}

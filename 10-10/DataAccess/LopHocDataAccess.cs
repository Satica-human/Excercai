namespace QuanLySinhVien_.DataAccess;

public sealed class LopHocDataAccess
{
    public List<LopHoc> LayDanhSach() => DataStore.LopHocs.ToList();
    public LopHoc? TimTheoMa(string maLop) => DataStore.LopHocs.FirstOrDefault(x => x.MaLop.Equals(maLop, StringComparison.OrdinalIgnoreCase));
    public bool Them(LopHoc lop) { if (TimTheoMa(lop.MaLop) != null) return false; DataStore.LopHocs.Add(lop); try { DataStore.LuuDuLieu(); return true; } catch { DataStore.LopHocs.Remove(lop); throw; } }
    public bool Sua(LopHoc lop) { var cu=TimTheoMa(lop.MaLop); if(cu==null)return false; var tenCu=cu.TenLop; cu.TenLop=lop.TenLop; try { DataStore.LuuDuLieu(); return true; } catch { cu.TenLop=tenCu; throw; } }
    public bool Xoa(string maLop) { var lop=TimTheoMa(maLop); if(lop==null)return false; DataStore.LopHocs.Remove(lop); try { DataStore.LuuDuLieu(); return true; } catch { DataStore.LopHocs.Add(lop); throw; } }
}

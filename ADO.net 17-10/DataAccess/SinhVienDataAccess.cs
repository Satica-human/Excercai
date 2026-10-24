namespace QuanLySinhVien_.DataAccess;

/// <summary>ADO.NET data access for student records, backed by MySQL.</summary>
public sealed class SinhVienDataAccess
{
    private readonly SinhVienDAL _dal = new();

    public List<SinhVien> LayDanhSach() => _dal.GetAllSinhVien();

    public SinhVien? TimTheoMa(string maSV) => _dal.GetSinhVienByMaSV(maSV);

    public List<SinhVien> LayTheoLop(string maLop) => _dal.GetSinhViensByMaLop(maLop);

    public void Them(SinhVien sv) => _dal.AddSinhVien(sv);

    public void Sua(SinhVien sv) => _dal.UpdateSinhVien(sv);

    public void Xoa(string maSV) => _dal.DeleteSinhVien(maSV);
}

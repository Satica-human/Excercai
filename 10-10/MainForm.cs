using System.ComponentModel.DataAnnotations;
using QuanLySinhVien_.Business;

namespace QuanLySinhVien_;

public sealed class MainForm : Form
{
    private readonly SinhVienBusiness _business = new();
    private readonly ErrorProvider _errors = new();
    private readonly ComboBox _lop = new(), _locLop = new(), _gioiTinh = new(), _trangThai = new();
    private readonly TextBox _search = new(), _ma = new(), _ten = new(), _email = new(), _dienThoai = new();
    private readonly DateTimePicker _ngaySinh = new();
    private readonly NumericUpDown _diem = new(), _diemTu = new();
    private readonly DataGridView _grid = new();
    private readonly Label _tongSo = new();
    private Button _btnThem = null!, _btnSua = null!, _btnXoa = null!;
    private bool _dangNap, _bindingGrid;

    public MainForm()
    {
        Text = "Ứng dụng quản lý sinh viên"; StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1000, 700); Size = new Size(1200, 790); Font = new Font("Segoe UI", 9F);
        BackColor = Color.FromArgb(245, 248, 251); _errors.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        DungGiaoDien(); NapDanhSachLop();
        Shown += (_, _) => _ma.Focus();
    }

    private void DungGiaoDien()
    {
        var root = new TableLayoutPanel { Dock=DockStyle.Fill, Padding=new Padding(0), ColumnCount=1, RowCount=6, BackColor=BackColor };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 202)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); root.RowStyles.Add(new RowStyle(SizeType.Absolute, 25)); Controls.Add(root);

        var brand = new Panel { Dock=DockStyle.Fill, BackColor=Color.FromArgb(28,75,117), Padding=new Padding(18,0,10,0) };
        brand.Controls.Add(new Label { Text="▣  Ứng dụng quản lý sinh viên", Dock=DockStyle.Fill, ForeColor=Color.White, Font=new Font("Segoe UI",12,FontStyle.Bold), TextAlign=ContentAlignment.MiddleLeft }); root.Controls.Add(brand,0,0);
        var heading = new Panel { Dock=DockStyle.Fill, BackColor=Color.White, Padding=new Padding(22,0,18,0) };
        heading.Controls.Add(new Label { Text="QUẢN LÝ SINH VIÊN", Dock=DockStyle.Left, Width=370, ForeColor=Color.FromArgb(28,65,101), Font=new Font("Segoe UI",16,FontStyle.Bold), TextAlign=ContentAlignment.MiddleLeft });
        heading.Controls.Add(new Label { Text="Bài tập Windows Forms • Quản lý sinh viên theo lớp", Dock=DockStyle.Right, Width=390, ForeColor=Color.FromArgb(60,75,90), TextAlign=ContentAlignment.MiddleRight }); root.Controls.Add(heading,0,1);

        var info = new GroupBox { Text="▏ Thông tin sinh viên", Dock=DockStyle.Fill, Margin=new Padding(15,2,15,5), Padding=new Padding(12,8,12,8), ForeColor=Color.FromArgb(28,75,117), Font=new Font("Segoe UI",9,FontStyle.Bold), BackColor=Color.White };
        var fields = new TableLayoutPanel { Dock=DockStyle.Fill, ColumnCount=6, RowCount=4, Padding=new Padding(0,4,0,0), BackColor=Color.White };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,95)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,90)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25));
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,75)); fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,25));
        for(int i=0;i<3;i++) fields.RowStyles.Add(new RowStyle(SizeType.Absolute,38)); fields.RowStyles.Add(new RowStyle(SizeType.Absolute,42));
        SetupInput(_ma); _ma.CharacterCasing=CharacterCasing.Upper; _ma.TextChanged+=Ma_TextChanged;
        _ma.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; _ngaySinh.Focus(); } };
        SetupInput(_ten); SetupInput(_email); SetupInput(_dienThoai);
        _ngaySinh.Dock=DockStyle.Fill; _ngaySinh.Format=DateTimePickerFormat.Short; _ngaySinh.MaxDate=DateTime.Today;
        SetupCombo(_gioiTinh); _gioiTinh.Items.AddRange(new object[]{"Nam","Nữ","Khác"});
        SetupCombo(_trangThai); _trangThai.Items.AddRange(new object[]{"Đang học","Bảo lưu","Đã tốt nghiệp","Đã thôi học"}); _trangThai.SelectedIndex=0;
        SetupCombo(_lop); _lop.DisplayMember=nameof(LopHoc.TenLop); _lop.ValueMember=nameof(LopHoc.MaLop); _lop.SelectedIndexChanged+=(_,_)=>HienThiDanhSach();
        _diem.Dock=DockStyle.Fill; _diem.DecimalPlaces=1; _diem.Minimum=0; _diem.Maximum=10; _diem.Increment=0.1m;
        AddFieldRow(fields,0,"Mã sinh viên *",_ma,"Họ và tên *",_ten,"Lớp học *",_lop);
        AddFieldRow(fields,1,"Ngày sinh *",_ngaySinh,"Giới tính *",_gioiTinh,"Điểm *",_diem);
        AddFieldRow(fields,2,"Email *",_email,"Điện thoại *",_dienThoai,"Trạng thái *",_trangThai);
        var buttons=new FlowLayoutPanel{Dock=DockStyle.Fill,FlowDirection=FlowDirection.RightToLeft,WrapContents=false,Padding=new Padding(0,4,0,0)};
        buttons.Controls.Add(ActionButton("↻ Làm mới",Color.FromArgb(112,130,148),(_,_)=>LamMoi()));
        _btnXoa=ActionButton("Xóa",Color.FromArgb(205,72,82),(_,_)=>Xoa());
        _btnSua=ActionButton("✎ Sửa",Color.FromArgb(42,111,170),(_,_)=>Luu(false));
        _btnThem=ActionButton("Thêm",Color.FromArgb(40,143,104),(_,_)=>Luu(true));
        buttons.Controls.Add(_btnXoa); buttons.Controls.Add(_btnSua); buttons.Controls.Add(_btnThem);
        _ma.TabIndex=0; _ten.TabIndex=1; _lop.TabIndex=2; _ngaySinh.TabIndex=3; _gioiTinh.TabIndex=4;
        _diem.TabIndex=5; _email.TabIndex=6; _dienThoai.TabIndex=7; _trangThai.TabIndex=8;
        _btnThem.TabIndex=9; _btnSua.TabIndex=10; _btnXoa.TabIndex=11;
        fields.Controls.Add(buttons,0,3); fields.SetColumnSpan(buttons,6); info.Controls.Add(fields); root.Controls.Add(info,0,2);

        var searchPanel=new Panel{Dock=DockStyle.Fill,Margin=new Padding(15,1,15,5),BackColor=Color.White,Padding=new Padding(12,10,12,5)};
        var searchFlow=new FlowLayoutPanel{Dock=DockStyle.Fill,WrapContents=false,FlowDirection=FlowDirection.LeftToRight};
        searchFlow.Controls.Add(InlineLabel("Từ khóa")); _search.Width=235; _search.PlaceholderText="Mã, họ tên, email hoặc điện thoại"; _search.Margin=new Padding(5,2,12,2); _search.TextChanged+=(_,_)=>HienThiDanhSach(); searchFlow.Controls.Add(_search);
        searchFlow.Controls.Add(InlineLabel("Lớp")); SetupCombo(_locLop); _locLop.DisplayMember=nameof(LopHoc.TenLop); _locLop.ValueMember=nameof(LopHoc.MaLop); _locLop.Width=185; _locLop.Margin=new Padding(5,2,12,2); _locLop.SelectedIndexChanged+=(_,_)=>HienThiDanhSach(); searchFlow.Controls.Add(_locLop);
        searchFlow.Controls.Add(InlineLabel("Điểm từ")); _diemTu.Width=78; _diemTu.DecimalPlaces=1; _diemTu.Minimum=0; _diemTu.Maximum=10; _diemTu.Margin=new Padding(5,2,12,2); _diemTu.ValueChanged+=(_,_)=>HienThiDanhSach(); searchFlow.Controls.Add(_diemTu);
        searchFlow.Controls.Add(ActionButton("⌕ Tìm kiếm",Color.FromArgb(42,111,170),(_,_)=>HienThiDanhSach()));
        searchFlow.Controls.Add(ActionButton("Hiển thị tất cả",Color.FromArgb(232,240,247),(_,_)=>{_search.Clear();_locLop.SelectedIndex=0;_diemTu.Value=0;HienThiDanhSach();},Color.FromArgb(28,75,117)));
        searchPanel.Controls.Add(searchFlow); root.Controls.Add(searchPanel,0,3);

        var listPanel=new Panel{Dock=DockStyle.Fill,Margin=new Padding(15,0,15,5),BackColor=Color.White,Padding=new Padding(0)};
        var listHeader=new Panel{Dock=DockStyle.Top,Height=34,Padding=new Padding(12,0,12,0),BackColor=Color.White};
        listHeader.Controls.Add(new Label{Text="▤  Danh sách sinh viên",Dock=DockStyle.Left,Width=280,ForeColor=Color.FromArgb(28,75,117),Font=new Font("Segoe UI",10,FontStyle.Bold),TextAlign=ContentAlignment.MiddleLeft});
        _tongSo.Text="Tổng số: 0 sinh viên";_tongSo.Dock=DockStyle.Right;_tongSo.Width=165;_tongSo.TextAlign=ContentAlignment.MiddleRight;_tongSo.ForeColor=Color.FromArgb(45,75,85);listHeader.Controls.Add(_tongSo);listPanel.Controls.Add(listHeader);
        _grid.Dock=DockStyle.Fill;_grid.ReadOnly=true;_grid.AllowUserToAddRows=false;_grid.AllowUserToDeleteRows=false;_grid.MultiSelect=false;_grid.SelectionMode=DataGridViewSelectionMode.FullRowSelect;_grid.AutoGenerateColumns=false;_grid.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;_grid.RowHeadersVisible=false;_grid.BackgroundColor=Color.White;_grid.BorderStyle=BorderStyle.None;_grid.GridColor=Color.FromArgb(226,233,239);_grid.ColumnHeadersHeight=32;_grid.RowTemplate.Height=29;_grid.EnableHeadersVisualStyles=false;_grid.ColumnHeadersDefaultCellStyle.BackColor=Color.FromArgb(232,241,248);_grid.ColumnHeadersDefaultCellStyle.ForeColor=Color.FromArgb(35,68,94);_grid.ColumnHeadersDefaultCellStyle.Font=new Font("Segoe UI",8,FontStyle.Bold);_grid.DefaultCellStyle.SelectionBackColor=Color.FromArgb(207,230,246);_grid.DefaultCellStyle.SelectionForeColor=Color.FromArgb(25,48,67);_grid.AlternatingRowsDefaultCellStyle.BackColor=Color.FromArgb(249,251,253);
        AddColumn("Mã SV",nameof(SinhVien.MaSV));AddColumn("Họ và tên",nameof(SinhVien.HoTen));AddColumn("Ngày sinh",nameof(SinhVien.NgaySinh),"dd/MM/yyyy");AddColumn("Giới tính",nameof(SinhVien.GioiTinh));AddColumn("Email",nameof(SinhVien.Email));AddColumn("Điện thoại",nameof(SinhVien.DienThoai));AddColumn("Điểm",nameof(SinhVien.Diem),"0.0");AddColumn("Lớp",nameof(SinhVien.MaLop));AddColumn("Trạng thái",nameof(SinhVien.TrangThai));
        _grid.SelectionChanged+=(_,_)=>{if(!_bindingGrid&&_grid.SelectedRows.Count>0&&_grid.SelectedRows[0].DataBoundItem is SinhVien sv)HienThiChiTiet(sv);};listPanel.Controls.Add(_grid);_grid.BringToFront();
        listPanel.Controls.Add(new Label{Text="Chọn một dòng để xem, sửa hoặc xóa thông tin sinh viên.",Dock=DockStyle.Bottom,Height=24,Padding=new Padding(12,4,0,0),ForeColor=Color.FromArgb(70,80,90)});root.Controls.Add(listPanel,0,4);
        var footer=new Panel{Dock=DockStyle.Fill,BackColor=Color.FromArgb(232,238,243),Padding=new Padding(15,0,15,0)};footer.Controls.Add(new Label{Text="Bài tập: Xây dựng Windows Forms quản lý sinh viên theo lớp",Dock=DockStyle.Left,Width=560,ForeColor=Color.FromArgb(55,70,82),TextAlign=ContentAlignment.MiddleLeft});footer.Controls.Add(new Label{Text="Thêm • Sửa • Xóa • Tìm kiếm • Lọc theo lớp",Dock=DockStyle.Right,Width=380,ForeColor=Color.FromArgb(55,70,82),TextAlign=ContentAlignment.MiddleRight});root.Controls.Add(footer,0,5);
    }

    private static Label InlineLabel(string text)=>new(){Text=text,AutoSize=true,Margin=new Padding(2,6,0,0),ForeColor=Color.FromArgb(38,57,75),Font=new Font("Segoe UI",8.5f,FontStyle.Bold)};
    private static void SetupInput(Control c)=>c.Dock=DockStyle.Fill;
    private static void SetupCombo(ComboBox c){c.Dock=DockStyle.Fill;c.DropDownStyle=ComboBoxStyle.DropDownList;}
    private static void AddFieldRow(TableLayoutPanel p,int row,string l1,Control c1,string l2,Control c2,string l3,Control c3){AddField(p,0,row,l1,c1);AddField(p,2,row,l2,c2);AddField(p,4,row,l3,c3);}
    private static void AddField(TableLayoutPanel p,int col,int row,string label,Control c){p.Controls.Add(new Label{Text=label,AutoSize=true,Anchor=AnchorStyles.Right,ForeColor=Color.FromArgb(37,57,78),Font=new Font("Segoe UI",8,FontStyle.Bold),Margin=new Padding(2,8,5,2)},col,row);p.Controls.Add(c,col+1,row);}
    private static Button ActionButton(string text,Color back,EventHandler click,Color? fore=null){var b=new Button{Text=text,BackColor=back,ForeColor=fore??Color.White,FlatStyle=FlatStyle.Flat,AutoSize=true,Height=30,Margin=new Padding(4,0,4,0),Padding=new Padding(8,0,8,0),Font=new Font("Segoe UI",8.5f,FontStyle.Bold)};b.FlatAppearance.BorderSize=0;b.Click+=click;return b;}
    private void AddColumn(string title,string property,string? format=null)=>_grid.Columns.Add(new DataGridViewTextBoxColumn{HeaderText=title,DataPropertyName=property,DefaultCellStyle=new DataGridViewCellStyle{Format=format??""}});
    private void NapDanhSachLop(){var lop=_business.LayDanhSachLop();_lop.DataSource=lop.ToList();_locLop.DataSource=new[]{new LopHoc{MaLop="",TenLop="Tất cả lớp"}}.Concat(lop).ToList();_locLop.SelectedIndex=0;if(_lop.Items.Count>0)_lop.SelectedIndex=0;CapNhatTrangThaiNut(true);HienThiDanhSach();}
    private void HienThiDanhSach(){if(_locLop.SelectedValue is not string maLoc)return;var key=_search.Text.Trim();var list=_business.LayDanhSachLop().SelectMany(l=>_business.LayDanhSachTheoLop(l.MaLop)).Where(s=>(string.IsNullOrEmpty(maLoc)||s.MaLop==maLoc)&&s.Diem>=_diemTu.Value&&(string.IsNullOrWhiteSpace(key)||s.MaSV.Contains(key,StringComparison.OrdinalIgnoreCase)||s.HoTen.Contains(key,StringComparison.OrdinalIgnoreCase)||s.Email.Contains(key,StringComparison.OrdinalIgnoreCase)||s.DienThoai.Contains(key,StringComparison.OrdinalIgnoreCase))).ToList();_bindingGrid=true;_grid.DataSource=list;_grid.ClearSelection();_bindingGrid=false;_tongSo.Text=$"Tổng số: {list.Count} sinh viên";}
    private void Ma_TextChanged(object? sender,EventArgs e)
    {
        if(_dangNap)return;
        _errors.SetError(_ma,"");
        var value=_ma.Text.Trim();
        var sv=string.IsNullOrWhiteSpace(value)?null:_business.TimTheoMa(value);
        if(sv!=null)
        {
            HienThiChiTiet(sv);
            CapNhatTrangThaiNut(false);
            return;
        }

        // Mã chưa tồn tại có thể là mã mới hoặc người dùng đang sửa mã vừa nhập.
        // Giữ lại các trường đã nhập để tránh mất dữ liệu khi sửa một ký tự của mã.
        CapNhatTrangThaiNut(true);
    }
    private void HienThiChiTiet(SinhVien sv){_dangNap=true;_ma.Text=sv.MaSV;_ten.Text=sv.HoTen;_ngaySinh.Value=sv.NgaySinh<DateTimePicker.MinimumDateTime?DateTimePicker.MinimumDateTime:sv.NgaySinh;_gioiTinh.SelectedItem=sv.GioiTinh;_email.Text=sv.Email;_dienThoai.Text=sv.DienThoai;_diem.Value=Math.Clamp(sv.Diem,_diem.Minimum,_diem.Maximum);_trangThai.SelectedItem=sv.TrangThai;if(_lop.SelectedValue is string m&&m!=sv.MaLop)_lop.SelectedValue=sv.MaLop;_dangNap=false;CapNhatTrangThaiNut(false);}
    private void XoaThongTin(bool clearCode=true){_dangNap=true;if(clearCode)_ma.Clear();_ten.Clear();_email.Clear();_dienThoai.Clear();_ngaySinh.Value=DateTime.Today.AddYears(-18);_gioiTinh.SelectedIndex=-1;_trangThai.SelectedIndex=0;_diem.Value=0;_dangNap=false;}
    private void CapNhatTrangThaiNut(bool dangThem){_btnThem.Enabled=dangThem;_btnSua.Enabled=!dangThem;_btnXoa.Enabled=!dangThem;}
    private SinhVien TaoSinhVien()=>new(){MaSV=_ma.Text.Trim(),HoTen=_ten.Text.Trim(),NgaySinh=_ngaySinh.Value.Date,GioiTinh=_gioiTinh.SelectedItem?.ToString()??"",Email=_email.Text.Trim(),DienThoai=_dienThoai.Text.Trim(),Diem=_diem.Value,TrangThai=_trangThai.SelectedItem?.ToString()??"",MaLop=_lop.SelectedValue?.ToString()??""};
    private bool KiemTra(SinhVien sv){_errors.Clear();var map=new Dictionary<string,Control>{{nameof(SinhVien.MaSV),_ma},{nameof(SinhVien.HoTen),_ten},{nameof(SinhVien.NgaySinh),_ngaySinh},{nameof(SinhVien.GioiTinh),_gioiTinh},{nameof(SinhVien.Email),_email},{nameof(SinhVien.DienThoai),_dienThoai},{nameof(SinhVien.Diem),_diem},{nameof(SinhVien.TrangThai),_trangThai},{nameof(SinhVien.MaLop),_lop}};bool ok=true;foreach(var(name,control)in map){var context=new ValidationContext(sv){MemberName=name};var results=new List<ValidationResult>();var value=typeof(SinhVien).GetProperty(name)!.GetValue(sv);if(!Validator.TryValidateProperty(value,context,results)){_errors.SetError(control,results.FirstOrDefault()?.ErrorMessage??"Dữ liệu không hợp lệ.");ok=false;}}return ok;}
    private void Luu(bool them){if(them&&_business.TimTheoMa(_ma.Text.Trim())!=null){_errors.SetError(_ma,"Mã sinh viên đã tồn tại; hãy chọn Sửa.");return;}if(!them&&_business.TimTheoMa(_ma.Text.Trim())==null){_errors.SetError(_ma,"Không tìm thấy sinh viên cần sửa.");return;}var sv=TaoSinhVien();if(!KiemTra(sv))return;try{bool ok;string error;if(them)ok=_business.Them(sv,out error);else ok=_business.Sua(sv,out error);if(!ok){_errors.SetError(_ma,error);return;}HienThiDanhSach();MessageBox.Show(them?"Đã thêm sinh viên.":"Đã cập nhật sinh viên.","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Information);if(them)LamMoi();}catch(Exception ex){MessageBox.Show($"Không thể lưu dữ liệu.\n{ex.Message}","Lỗi lưu dữ liệu",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
    private void Xoa(){var ma=_ma.Text.Trim();var sv=_business.TimTheoMa(ma);if(sv==null){_errors.SetError(_ma,"Không tìm thấy sinh viên.");return;}if(MessageBox.Show($"Bạn sắp xóa sinh viên:\n{sv.MaSV} - {sv.HoTen}\n\nBạn có chắc chắn không?","Xác nhận xóa",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)!=DialogResult.Yes)return;try{_business.Xoa(sv.MaSV);LamMoi();HienThiDanhSach();}catch(Exception ex){MessageBox.Show($"Không thể lưu thay đổi khi xóa.\n{ex.Message}","Lỗi lưu dữ liệu",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
    private void LamMoi(){_errors.Clear();XoaThongTin();_grid.ClearSelection();CapNhatTrangThaiNut(true);_ma.Focus();}
}

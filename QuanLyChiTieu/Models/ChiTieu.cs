namespace QuanLyChiTieu.Models
{
    public class ChiTieu
    {
        public int Id { get; set; }
        public int Id_PhanLoai { get; set; }
        public int Id_DanhMuc { get; set; }
        public int Id_User { get; set; }
        public int Id_LoaiTienTe { get; set; }
        public decimal Total { get; set; }
        public DateTime CreateAt { get; set; }
        public string Notes { get; set; }
    }

}

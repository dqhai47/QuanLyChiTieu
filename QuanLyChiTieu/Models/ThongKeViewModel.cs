namespace QuanLyChiTieu.Models
{
    public class ThongKeViewModel
    {
        public decimal TongThu { get; set; }
        public decimal TongChi { get; set; }
        public decimal ChenhLech => TongThu - TongChi;

        public int TongLoaiChiTieu { get; set; }
        public string? LoaiChiTieuPhoBien { get; set; }
        public int SoLanSuDung { get; set; }

        public List<ChiTieuTheoLoai>? ChiTieuTheoLoaiList { get; set; } = new List<ChiTieuTheoLoai>();
        public List<DanhMucThongKe> DanhMucThongKes { get; set; } = new List<DanhMucThongKe>();
    }

    public class ChiTieuTheoLoai
    {
        public string? TenLoai { get; set; }
        public decimal TongTien { get; set; }
    }

    public class DanhMucThongKe
    {
        public string? TenDanhMuc { get; set; }
        public decimal TongTien { get; set; }
    }
}

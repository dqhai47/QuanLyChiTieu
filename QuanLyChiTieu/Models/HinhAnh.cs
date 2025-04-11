namespace QuanLyChiTieu.Models
{
    public class HinhAnh
    {
        public int Id { get; set; }
        public int Id_ChiTieu { get; set; }
        public string UrlImage { get; set; }
        public DateTime CreateAt { get; set; }
        public string Notes { get; set; }
    }

}

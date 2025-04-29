using System.ComponentModel.DataAnnotations;

namespace QuanLyChiTieu.Models
{
    public class ChiTieu
    {
        internal string? ten_loaichitieu;
        internal string? ten_danhmuc;
        internal string? ten_loaitiente;
        [Key]
        public int id { get; set; }
        public string? titles { get; set; }
        public int id_loaichitieu { get; set; }
        public int id_danhmuc { get; set; }
        public int id_user { get; set; }
        public int id_loaitiente { get; set; }
        public decimal total { get; set; }
        public DateTime create_at { get; set; }
        public string? notes { get; set; }
    }

}

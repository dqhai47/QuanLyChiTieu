using System.ComponentModel.DataAnnotations;

namespace QuanLyChiTieu.Models
{
    public class HinhAnh
    {
        [Key]
        public int id { get; set; }
        public int id_chitieu { get; set; }
        public string? url_image { get; set; }
        public DateTime create_at { get; set; }
        public string? notes { get; set; }
    }

}

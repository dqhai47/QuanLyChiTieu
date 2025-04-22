using System.ComponentModel.DataAnnotations;

namespace QuanLyChiTieu.Models
{
    public class DanhMuc
    {
        [Key]  // Quan trọng!
        public int id { get; set; }
        public string sname { get; set; }
        public string scode { get; set; }
        public bool sdefault { get; set; }
        public string descriptions { get; set; }
    }

}

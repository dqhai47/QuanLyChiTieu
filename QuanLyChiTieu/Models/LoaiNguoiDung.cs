using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChiTieu.Models
{
    public class LoaiNguoiDung
    {
        [Key]  // Quan trọng!
        public int id { get; set; }
        public string? sname { get; set; }
        public string? scode { get; set; }
        public string? descriptions { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace QuanLyChiTieu.Models
{
    public class LoaiTienTe
    {
        [Key]  // Quan trọng!
        public int id { get; set; }
        public string? sname { get; set; }
        public string? scode { get; set; }
        public bool sdefault { get; set; }
        public string? converts { get; set; }
        public string? descriptions { get; set; }
    }

}

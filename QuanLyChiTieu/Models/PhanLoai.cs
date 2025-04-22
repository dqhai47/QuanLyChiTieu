using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace QuanLyChiTieu.Models
{
    public class PhanLoai
    {
        [Key]  // Quan trọng!
        public int id { get; set; }
        public string sname { get; set; }
        public string scode { get; set; }
        public string descriptions { get; set; }
    }
}

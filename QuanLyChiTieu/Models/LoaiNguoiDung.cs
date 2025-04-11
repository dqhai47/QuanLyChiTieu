using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChiTieu.Models
{
    public class LoaiNguoiDung
    {
        public int Id { get; set; }
        public string Sname { get; set; }
        public string Scode { get; set; }
        public string Descriptions { get; set; }
    }
}

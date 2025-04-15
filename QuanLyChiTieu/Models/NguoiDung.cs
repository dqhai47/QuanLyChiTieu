using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace QuanLyChiTieu.Models
{
    public class NguoiDung
    {
        internal string? ten_loai;

        public int id { get; set; }

        [Required(ErrorMessage = "Họ không được để trống")]
        public string fname { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string lname { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có đúng 10 chữ số")]
        [Remote(action: "IsPhoneInUse", controller: "Auth", ErrorMessage = "Số điện thoại đã tồn tại")]
        public string phone { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Remote(action: "IsEmailInUse", controller: "Auth", ErrorMessage = "Email đã tồn tại")]
        public string email { get; set; }

        public string gender { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(25, ErrorMessage = "Tên đăng nhập không được vượt quá 25 ký tự")]
        [Remote(action: "IsUsernameInUse", controller: "Auth", ErrorMessage = "Tên đăng nhập đã tồn tại")]
        public string username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[A-Z])(?=.*[\W_]).{6,}$",
            ErrorMessage = "Mật khẩu phải có chữ cái, chữ số, ít nhất 1 chữ viết hoa và 1 ký tự đặc biệt")]
        public string pwd { get; set; }

        public bool status_account { get; set; } = true;
        public DateTime create_at { get; set; } = DateTime.Now;

        public int id_loainguoidung { get; set; } = 2; // Người dùng thường
        //public string ten_loai { get; set; } // 👈 Thêm dòng này

        public string? descriptions { get; set; }
    }
}

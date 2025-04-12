using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace QuanLyChiTieu.Models
{
    public class NguoiDung
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Họ không được để trống")]
        public string fname { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string lname { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Remote(action: "IsPhoneInUse", controller: "Auth", ErrorMessage = "Số điện thoại đã tồn tại")]
        public string phone { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Remote(action: "IsEmailInUse", controller: "Auth", ErrorMessage = "Email đã tồn tại")]
        public string email { get; set; }

        public string gender { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [Remote(action: "IsUsernameInUse", controller: "Auth", ErrorMessage = "Tên đăng nhập đã tồn tại")]
        public string username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string pwd { get; set; }

        public bool status_account { get; set; } = true;
        public DateTime create_at { get; set; } = DateTime.Now;

        public int id_loainguoidung { get; set; } = 2; // Giả định 2 là role người dùng thường

        public string? descriptions { get; set; }
    }
}

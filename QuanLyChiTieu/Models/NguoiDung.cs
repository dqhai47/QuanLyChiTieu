namespace QuanLyChiTieu.Models
{
    public class NguoiDung
    {
        public int id { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string username { get; set; }
        public string pwd { get; set; }
        public bool status_account { get; set; }
        public DateTime create_at { get; set; }
        public int id_loainguoidung { get; set; }
        public string descriptions { get; set; }
    }

}

using Microsoft.EntityFrameworkCore;

namespace QuanLyChiTieu.Models
{
    public class QuanLyChiTieuContext : DbContext
    {
        public QuanLyChiTieuContext(DbContextOptions<QuanLyChiTieuContext> options)
            : base(options)
        {
        }

        public DbSet<LoaiNguoiDung> LoaiNguoiDung { get; set; }
        public DbSet<LoaiChiTieu> PhanLoai { get; set; }
        public DbSet<LoaiTienTe> LoaiTienTe { get; set; }
        public DbSet<DanhMuc> DanhMuc { get; set; }
        public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<ChiTieu> ChiTieu { get; set; }
        public DbSet<HinhAnh> HinhAnh { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ánh xạ tên bảng nếu cần
            modelBuilder.Entity<LoaiNguoiDung>().ToTable("LoaiNguoiDung");
            modelBuilder.Entity<LoaiChiTieu>().ToTable("PhanLoai");
            modelBuilder.Entity<LoaiTienTe>().ToTable("LoaiTienTe");
            modelBuilder.Entity<DanhMuc>().ToTable("DanhMuc");
            modelBuilder.Entity<NguoiDung>().ToTable("NguoiDung");
            modelBuilder.Entity<ChiTieu>().ToTable("ChiTieu");
            modelBuilder.Entity<HinhAnh>().ToTable("HinhAnh");
        }
    }
}

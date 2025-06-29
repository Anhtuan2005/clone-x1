using Microsoft.EntityFrameworkCore;
using bike.Models;

namespace bike.Repository
{
    // DbContext là class quản lý kết nối và tương tác với database
    public class BikeDbContext : DbContext
    {
        // Constructor nhận vào options để cấu hình database
        public BikeDbContext(DbContextOptions<BikeDbContext> options)
            : base(options)
        {
        }
        // DbSet đại diện cho bảng Xe trong database
        // Tên DbSet sẽ là tên bảng trong SQL Server
        public DbSet<Xe> Xe { get; set; }
        public DbSet<LoaiXe> LoaiXe {  get; set;}
        public DbSet<User> Users { get; set; }
        public DbSet<DatCho> DatCho { get; set; }   
        public DbSet<HopDong> HopDong { get;set; }

        public DbSet<ChiTieu> ChiTieu { get; set; }

        // cấu hình thêm cho database (nếu cần)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChiTieu>(e => e.Property(p => p.SoTien).HasColumnType("decimal(18, 2)"));
            modelBuilder.Entity<HopDong>(e => {
                e.Property(p => p.GiaThueNgay).HasColumnType("decimal(18, 2)");
                e.Property(p => p.PhuPhi).HasColumnType("decimal(18, 2)");
                e.Property(p => p.TienCoc).HasColumnType("decimal(18, 2)");
                e.Property(p => p.TongTien).HasColumnType("decimal(18, 2)");
            });
            modelBuilder.Entity<Xe>(e => e.Property(p => p.GiaThue).HasColumnType("decimal(18, 2)"));
        }
    }
}
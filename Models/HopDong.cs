using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bike.Models
{
    public class HopDong
    {
        [Key]
        public int MaHopDong { get; set; }

        // Liên kết với phiếu đặt chỗ
        [Display(Name = "Phiếu đặt chỗ")]
        public int? MaDatCho { get; set; }

        [ForeignKey("MaDatCho")]
        public DatCho? DatCho { get; set; }

        // Thông tin xe
        [Display(Name = "Xe")]
        [Required]
        public int MaXe { get; set; }

        [ForeignKey("MaXe")]
        public Xe? Xe { get; set; }

        // Thông tin khách hàng
        [Display(Name = "Họ tên khách")]
        [Required]
        [StringLength(100)]
        public string ?HoTenKhach { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required]
        [StringLength(20)]
        public string ?SoDienThoai { get; set; }

        [Display(Name = "CCCD/CMND")]
        [Required]
        [StringLength(20)]
        public string ?SoCCCD { get; set; }

        [Display(Name = "Địa chỉ")]
        [StringLength(255)]
        public string? DiaChi { get; set; }

        // Thời gian thuê
        [Display(Name = "Ngày nhận xe")]
        [Required]
        public DateTime NgayNhanXe { get; set; }

        [Display(Name = "Ngày trả xe dự kiến")]
        [Required]
        public DateTime NgayTraXeDuKien { get; set; }

        [Display(Name = "Ngày trả xe thực tế")]
        public DateTime? NgayTraXeThucTe { get; set; }

        // Tài chính
        [Display(Name = "Giá thuê/ngày")]
        [Required]
        public decimal GiaThueNgay { get; set; }

        [Display(Name = "Tiền cọc")]
        [Required]
        public decimal TienCoc { get; set; }

        [Display(Name = "Phụ phí")]
        public decimal PhuPhi { get; set; } = 0;

        [Display(Name = "Tổng tiền")]
        public decimal TongTien { get; set; }

        // Thông tin khác
        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? GhiChu { get; set; }

        [Display(Name = "Ngày tạo hợp đồng")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Đang thuê"; // Đang thuê, Hoàn thành, Hủy

        [Display(Name = "Người tạo")]
        public int? MaNguoiTao { get; set; }

        [ForeignKey("MaNguoiTao")]
        public User? NguoiTao { get; set; }

        // Tính toán
        [NotMapped]
        public int SoNgayThue => NgayTraXeThucTe.HasValue
            ? (NgayTraXeThucTe.Value - NgayNhanXe).Days + 1
            : (NgayTraXeDuKien - NgayNhanXe).Days + 1;

        [NotMapped]
        public decimal TongTienDuKien => GiaThueNgay * SoNgayThue + PhuPhi;
    }
}
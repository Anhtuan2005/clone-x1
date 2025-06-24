using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bike.Models
{
    public class DatCho
    {
        [Key]
        public int MaDatCho { get; set; }

        // Thông tin xe
        [Display(Name = "Xe")]
        public int MaXe { get; set; }

        [ForeignKey("MaXe")]
        public Xe? Xe { get; set; }

        // Thông tin người đặt
        [Display(Name = "Người đặt")]
        public int? MaUser { get; set; }

        [ForeignKey("MaUser")]
        public User? User { get; set; }

        // Thông tin liên hệ (nếu không đăng nhập)
        [Display(Name = "Họ tên")]
        [StringLength(100)]
        public string? HoTen { get; set; }

        [Display(Name = "Số điện thoại")]
        [StringLength(20)]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Email")]
        [StringLength(100)]
        public string? Email { get; set; }

        // Thời gian thuê
        [Required(ErrorMessage = "Vui lòng chọn ngày nhận xe")]
        [Display(Name = "Ngày nhận xe dự kiến")]
        [DataType(DataType.Date)]
        public DateTime NgayNhanXe { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày trả xe")]
        [Display(Name = "Ngày trả xe dự kiến")]
        [DataType(DataType.Date)]
        public DateTime NgayTraXe { get; set; }

        // Thông tin khác
        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? GhiChu { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime NgayDat { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Chờ xác nhận"; // Chờ xác nhận, Đang giữ chỗ, Đã xử lý, Hủy

        // Tính số ngày thuê
        [NotMapped]
        public int SoNgayThue => (NgayTraXe - NgayNhanXe).Days + 1;

        // Tính tổng tiền dự kiến
        [NotMapped]
        public decimal TongTienDuKien => Xe != null ? Xe.GiaThue * SoNgayThue : 0;
    }
}
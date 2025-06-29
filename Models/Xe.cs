using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using bike.Attributes;

namespace bike.Models
{
    public class Xe
    {
        [Key]
        public int MaXe { get; set; }

        [Required(ErrorMessage = "Tên xe là bắt buộc")]
        [Display(Name = "Tên xe")]
        [StringLength(100)]
        public string TenXe { get; set; }

        [Required(ErrorMessage = "Biển số xe là bắt buộc")]
        [Display(Name = "Biển số")]
        [StringLength(20)]
        public string BienSoXe { get; set; }

        [Required(ErrorMessage = "Hãng xe là bắt buộc")]
        [Display(Name = "Hãng xe")]
        [StringLength(50)]
        public string HangXe { get; set; }

        [Required(ErrorMessage = "Dòng xe là bắt buộc")]
        [Display(Name = "Dòng xe")]
        [StringLength(50)]
        public string? DongXe { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [Display(Name = "Trạng thái")]
        [StringLength(20)]
        public string? TrangThai { get; set; } // Sẵn sàng, Đang thuê, Bảo trì

        [Display(Name = "Giá thuê/ngày")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá thuê phải lớn hơn 0")]
        public decimal GiaThue { get; set; }

        [Display(Name = "Hình ảnh")]
        [StringLength(255)]
        public string? HinhAnhXe { get; set; }

        // Thêm thuộc tính khóa ngoại
        [Display(Name = "Mã loại xe")]
        public int MaLoaiXe { get; set; }

        // Khai báo navigation property
        [ForeignKey("MaLoaiXe")]
        public LoaiXe? LoaiXe { get; set; }

        public virtual ICollection<ChiTieu> ChiTieu { get; set; } = new List<ChiTieu>();
    }
}

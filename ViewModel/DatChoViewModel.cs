using System.ComponentModel.DataAnnotations;

namespace bike.ViewModels
{
    public class DatChoViewModel
    {
        // Thông tin xe (readonly)
        public int MaXe { get; set; }
        public string? TenXe { get; set; }
        public string? BienSoXe { get; set; }
        public decimal GiaThue { get; set; }
        public string? HinhAnhXe { get; set; }

        // Thông tin người đặt
        [Display(Name = "Họ và tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTen { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        // Thời gian thuê
        [Display(Name = "Ngày nhận xe")]
        [Required(ErrorMessage = "Vui lòng chọn ngày nhận xe")]
        [DataType(DataType.Date)]
        public DateTime NgayNhanXe { get; set; } = DateTime.Now.AddDays(1);

        [Display(Name = "Ngày trả xe")]
        [Required(ErrorMessage = "Vui lòng chọn ngày trả xe")]
        [DataType(DataType.Date)]
        public DateTime NgayTraXe { get; set; } = DateTime.Now.AddDays(3);

        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? GhiChu { get; set; }

        // Validation tùy chỉnh
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NgayNhanXe < DateTime.Now.Date)
            {
                yield return new ValidationResult("Ngày nhận xe phải từ hôm nay trở đi", new[] { nameof(NgayNhanXe) });
            }

            if (NgayTraXe <= NgayNhanXe)
            {
                yield return new ValidationResult("Ngày trả xe phải sau ngày nhận xe", new[] { nameof(NgayTraXe) });
            }
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace bike.Models 
{
    public class ChiTieu
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung chi tiêu")]
        [Display(Name = "Nội dung chi tiêu")]
        public string NoiDung { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        [DataType(DataType.Currency)]
        [Display(Name = "Số tiền")]
        public decimal SoTien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày chi")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày chi")]
        public DateTime NgayChi { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; } 


        [Display(Name = "Xe liên quan")]
        public int? MaXe { get; set; } 

        [ForeignKey("MaXe")]
        public virtual Xe? Xe { get; set; } 
    }
}
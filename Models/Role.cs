using System.ComponentModel.DataAnnotations;

namespace bike.Models
{
    public class Role
    {
        [Key]
        public int MaVaiTro { get; set; }

        [Required(ErrorMessage = "Tên vai trò là bắt buộc")]
        [Display(Name = "Tên vai trò")]
        [StringLength(50)]
        public string TenVaiTro { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(255)]
        public string? MoTa { get; set; }

        // Navigation property
        public ICollection<User>? Users { get; set; }
    }
}
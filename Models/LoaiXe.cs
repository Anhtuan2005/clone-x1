using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace bike.Models
{
    public class LoaiXe
    {
        [Key]
        public int MaLoaiXe { get; set; }

        [Required(ErrorMessage = "Tên loại xe là bắt buộc")]
        [StringLength(50)]
        public string TenLoaiXe { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? NgayCapNhat { get; set; }

        // Navigation property cho 1-nhiều
        public ICollection<Xe> Xes { get; set; } = new List<Xe>();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using bike.Models;
using System.ComponentModel.DataAnnotations;

namespace bike.ViewModel
{
    public class XeMayLoaiXe
    {
        // Danh sách loại xe để hiển thị tabs
        public List<LoaiXe> DanhSachLoaiXe { get; set; }

        // Danh sách xe máy để hiển thị
        public List<Xe> DanhSachXeMay { get; set; }

        // Constructor
        public XeMayLoaiXe()
        {
            DanhSachLoaiXe = new List<LoaiXe>();
            DanhSachXeMay = new List<Xe>();
        }
    }
}

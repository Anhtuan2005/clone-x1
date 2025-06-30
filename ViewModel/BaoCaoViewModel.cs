using System;
using System.Collections.Generic;

namespace bike.ViewModels
{
    public class BaoCaoViewModel
    {
        // Thời gian lọc
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        // Dữ liệu cho các thẻ tài chính
        public decimal TongDoanhThu { get; set; }
        public decimal TongChiPhi { get; set; }
        public decimal LoiNhuan { get; set; }

        // Thống kê tổng quan
        public int TongDonDatXe { get; set; }

        public int DonChoXuLy { get; set; }
        public decimal DoanhThuHomNay { get; set; }
        public int XeDangChoThue { get; set; }

        // Thống kê so với kỳ trước
        public double PhanTramDonDat { get; set; } // % tăng/giảm so với kỳ trước
        public double PhanTramDoanhThu { get; set; }

        // Dữ liệu cho biểu đồ
        public List<BieuDoItem> BieuDoDoanhThu { get; set; }
        public List<BieuDoItem> BieuDoDonDat { get; set; }

        // Top xe được thuê nhiều
        public List<XeThueNhieuItem> TopXeThueNhieu { get; set; }

        // Danh sách đơn gần đây
        public List<DonDatGanDayItem> DonDatGanDay { get; set; }

       
        public BaoCaoViewModel()
        {
            BieuDoDoanhThu = new List<BieuDoItem>();
            BieuDoDonDat = new List<BieuDoItem>();
            TopXeThueNhieu = new List<XeThueNhieuItem>();
            DonDatGanDay = new List<DonDatGanDayItem>();
        }
    }

    // Class cho item biểu đồ
    public class BieuDoItem
    {
        public string ?Label { get; set; } // Ngày/Tháng
        public decimal Value { get; set; } // Giá trị
    }

    // Class cho xe thuê nhiều
    public class XeThueNhieuItem
    {
        public string ?TenXe { get; set; }
        public string ?BienSo { get; set; }
        public int SoLanThue { get; set; }
        public decimal DoanhThu { get; set; }
    }

    // Class cho đơn đặt gần đây
    public class DonDatGanDayItem
    {
        public int MaDatCho { get; set; }
        public string ?TenKhach { get; set; }
        public string ?TenXe { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayTra { get; set; }
        public string ?TrangThai { get; set; }
        public decimal TongTien { get; set; }
    }

}
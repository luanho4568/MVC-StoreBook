using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcBookStore.Models
{
    public class DoanhThuViewModel
    {
        public int MaDonHang { get; set; }
        public string TenKhachHang { get; set; }
        public string NgayDat { get; set; }
        public int SoLuong { get; set; }
        public decimal TongTien { get; set; }
    }

}
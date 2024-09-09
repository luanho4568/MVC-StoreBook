using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcBookStore.Models
{
    public partial class DONDATHANG
    {
        // Các thuộc tính hiện tại...
        // Trong lớp DONDATHANG
        public string TenKhachHang
        {
            get
            {
                if (this.KHACHHANG != null) // Giả sử bạn có một quan hệ với bảng KHACHHANG
                    return this.KHACHHANG.HoTen; // Thay bằng tên thuộc tính thích hợp
                return "Khách hàng không xác định"; // Hoặc trả về một giá trị mặc định khác
            }
        }
    
    public bool DathanhtoanValue => Dathanhtoan.HasValue && Dathanhtoan.Value;
        public string DathanhtoanText => DathanhtoanValue ? "Đã thanh toán" : "Chưa thanh toán";

        public bool TinhtranggiaohangValue => Tinhtranggiaohang.HasValue && Tinhtranggiaohang.Value;
        public string TinhtranggiaohangText => TinhtranggiaohangValue ? "Đã giao hàng" : "Chưa giao hàng";

        public decimal Tongtien { get; set; }

        // Thêm thuộc tính Soluongdon
        public int Soluongdon { get; set; }
    }
}

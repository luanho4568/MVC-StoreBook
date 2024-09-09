using MvcBookStore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcBookStore.Controllers
{
    public class YeuThichController : Controller
    {
        private QLBANSACHDataContext data = new QLBANSACHDataContext("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=QLBANSACH;Integrated Security=True;Encrypt=False");

        public List<Giohang> LayYeuThich()
        {
            List<Giohang> lstYeuThich = Session["YeuThich"] as List<Giohang>;

            if (lstYeuThich == null)
            {
                lstYeuThich = new List<Giohang>();
                Session["YeuThich"] = lstYeuThich;
            }

            return lstYeuThich;
        }

        // Thêm vào controller
        private int TongSoLuongYeuThich()
        {
            int iTongSoLuong = 0;

            List<Giohang> lstYeuThich = Session["YeuThich"] as List<Giohang>;
            if (lstYeuThich != null)
            {
                iTongSoLuong = lstYeuThich.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }

        // Thêm vào controller
        private double TongTienYeuThich()
        {
            double iTongTien = 0;
            List<Giohang> lstYeuThich = Session["YeuThich"] as List<Giohang>;
            if (lstYeuThich != null)
            {
                iTongTien = lstYeuThich.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }

        public ActionResult ThemYeuThich(int iMasach, string strURL)
        {
            List<Giohang> lstYeuThich = LayYeuThich();

            Giohang sanpham = lstYeuThich.Find(n => n.iMasach == iMasach);

            if (sanpham == null)
            {
                sanpham = new Giohang(iMasach);
                lstYeuThich.Add(sanpham);

                return Redirect(strURL);
            }
            else
            {
                return Redirect(strURL);
            }
        }

        public ActionResult YeuThich()
        {
            List<Giohang> lstYeuThich = LayYeuThich();
            ViewBag.Tongsoluong = TongSoLuongYeuThich();
            ViewBag.Tongtien = TongTienYeuThich();

            return View(lstYeuThich);
        }

        public ActionResult XoaYeuThich(int iMaSP)
        {
            List<Giohang> lstYeuThich = LayYeuThich();
            Giohang sanpham = lstYeuThich.SingleOrDefault(n => n.iMasach == iMaSP);

            if (sanpham != null)
            {
                lstYeuThich.RemoveAll(n => n.iMasach == iMaSP);
                return RedirectToAction("YeuThich");
            }

            return RedirectToAction("YeuThich");
        }

        public ActionResult XoaTatcaYeuThich()
        {
            List<Giohang> lstYeuThich = LayYeuThich();
            lstYeuThich.Clear();
            return RedirectToAction("YeuThich");
        }

        public ActionResult ChuyenVaoGioHang()
        {
            List<Giohang> lstYeuThich = LayYeuThich();
            List<Giohang> lstGioHang = Laygiohang();

            lstGioHang.AddRange(lstYeuThich);
            lstYeuThich.Clear();

            return RedirectToAction("GioHang", "Giohang");
        }
        [HttpPost]
        public ActionResult ChuyenVaoGioHang(string[] selectedBooks)
        {
            if (selectedBooks != null && selectedBooks.Any())
            {
                List<Giohang> lstYeuThich = LayYeuThich();

                foreach (var masach in selectedBooks)
                {
                    int iMasach;
                    if (int.TryParse(masach, out iMasach))
                    {
                        Giohang sanpham = lstYeuThich.Find(n => n.iMasach == iMasach);

                        if (sanpham != null)
                        {
                            // Kiểm tra xem sách đã có trong giỏ hàng hay chưa
                            List<Giohang> lstGiohang = Laygiohang();
                            Giohang existingBook = lstGiohang.Find(n => n.iMasach == iMasach);

                            if (existingBook == null)
                            {
                                // Nếu sách chưa có trong giỏ hàng thì thêm vào
                                lstGiohang.Add(sanpham);
                            }
                            else
                            {
                                // Nếu sách đã có trong giỏ hàng thì tăng số lượng lên
                                existingBook.iSoluong += sanpham.iSoluong;
                            }
                        }
                    }
                }

                // Lọc những quyển sách đã chọn khỏi danh sách yêu thích
                lstYeuThich.RemoveAll(x => selectedBooks.Contains(x.iMasach.ToString()));

                return RedirectToAction("GioHang", "Giohang");
            }

            // Nếu không có quyển sách nào được chọn, có thể xử lý hoặc thông báo tùy ý
            return RedirectToAction("YeuThich");
        }
        // Thêm vào YeuThichController.cs
        private List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;

            if (lstGiohang == null)
            {
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }

            return lstGiohang;
        }

    }
}

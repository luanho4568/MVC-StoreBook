using MvcBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcBookStore.Controllers
{
    public class GiohangController : Controller
    {
        //Tao doi tuong data chua du lieu tu model dbBansach da tao
        QLBANSACHDataContext data = new QLBANSACHDataContext("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=QLBANSACH;Integrated Security=True;Encrypt=False");
        //Lay gio hang

        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                //Neu gio hang chua ton tai thoi khoi tao listGiohang
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        //Them vao gio hang
        public ActionResult ThemGiohang(int iMasach, string strURL)
        {
            //Lay ra Session gio hang
            List<Giohang> lstGiohang = Laygiohang();
            //Kiem tra sach nay ton tai trong Session["Giohang'] hay chua
            Giohang sanpham = lstGiohang.Find(n => n.iMasach == iMasach);
            if (sanpham == null)
            {
                sanpham = new Giohang(iMasach);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                Session["SL"] = sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        //Tong so luong
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;

            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;

        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGiohang = Session["GioHang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }
        //Xay dung trang Gio hang
        public ActionResult GioHang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            bool isCartEmpty = lstGiohang.Count == 0;

            if (isCartEmpty)
            {
                ViewBag.CartMessage = "Giỏ hàng của bạn chưa có quyển sách nào.";
            }
            else
            {
                ViewBag.CartMessage = $"Giỏ hàng của bạn có {lstGiohang.Count} quyển sách.";
            }

            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            ViewBag.IsCartEmpty = isCartEmpty;
            return View(lstGiohang);

        }
        public ActionResult XoaGiohang(int iMaSP)
        {
            //Lay gio hang Session
            List<Giohang> lstGiohang = Laygiohang();
            //Kiem tra danh sach da co trong Session["Giohang"]
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == iMaSP);
            //Neu ton tai san pham thi cho sua so luong
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iMasach == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            return RedirectToAction("GioHang");
        }
        //Cap nhat Gio hang
        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            //Lay gio hang tu Session
            List<Giohang> lstGiohang = Laygiohang();
            //Kiem tra sach da co trong Session["Gohang"]
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == iMaSP);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }
        public ActionResult XoaTatcaGiohang()
        {
            //Lay gio hang tu Session
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            Session["SL"] = null;
            return RedirectToAction("Index", "BookStore");
        }
        //Hien thi View DatHang de cap nhat cac thong tin cho Don Hang
        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiem tra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "BookStore");
            }
            //Lay gio hang tu Session
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        public ActionResult DatHang(FormCollection collection)
        {
            //Them Don Hang
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<Giohang> gh = Laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Now.AddDays(3); // Bạn có thể đặt giá trị theo nhu cầu
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Them chi tiet don hang
            foreach (var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.Masach = item.iMasach;
                ctdh.Soluong = item.iSoluong;
                ctdh.Dongia = (decimal)item.dDonggia;
                data.CHITIETDONTHANGs.InsertOnSubmit(ctdh);

                // Giảm số lượng tồn của sách
                var sach = data.SACHes.SingleOrDefault(s => s.Masach == item.iMasach);
                if (sach != null)
                {
                    sach.Soluongton -= item.iSoluong;
                }
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }
        public ActionResult Xacnhandonhang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return View();
        }

    }
}
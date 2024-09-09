using MvcBookStore.Models;
using PagedList;
using System;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MvcBookStore.Controllers
{
    public class AdminController : Controller
    {
        QLBANSACHDataContext db = new QLBANSACHDataContext("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=QLBANSACH;Integrated Security=True;Encrypt=False");
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Sach(int? page)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            //return View(db.SACHes.ToList());
            return View(db.SACHes.ToList().OrderBy(n => n.Masach).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            //Gan cac gia tri nguoi dung nhap lieu cac bien
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập!";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu!";
            }
            else
            {
                //Gan gia tri cho doi tuong duoc tao moi
                Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["Ten"] = ad.Hoten;
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng!";
            }
            return View();
        }
        public ActionResult Chitietsach(int id)
        {
            //Lay doi tuong sach theo ma
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        public ActionResult Xoasach(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost, ActionName("Xoasach")]
        public ActionResult Xacnhanxoa(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SACHes.DeleteOnSubmit(sach);
            db.SubmitChanges();
            return RedirectToAction("Sach");
        }
        //Đơn hàng
        public ActionResult Chitietdonhang(int id)
        {
            var orderDetails = db.CHITIETDONTHANGs.Where(d => d.MaDonHang == id).ToList();
            return View(orderDetails);
        }
        //Phân loại sách
        public ActionResult Loai(int? page)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(db.Loais.ToList().OrderBy(n => n.MaLoai).ToPagedList(pageNumber, pageSize));
        }
        //Phân loại sách
        public ActionResult NXB(int? page)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            //return View(db.SACHes.ToList());
            return View(db.NHAXUATBANs.ToList().OrderBy(n => n.MaNXB).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemmoiLoai()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ThemmoiLoai(Loai loai)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            if (ModelState.IsValid)
            {
                // Assuming LoaiSach is the model for book categories
                db.Loais.InsertOnSubmit(loai);
                db.SubmitChanges();

                return RedirectToAction("Loai"); // Redirect to the list of book categories after adding a new one
            }

            return View();
        }
        [HttpGet]
        public ActionResult Sualoai(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            Loai loai = db.Loais.SingleOrDefault(n => n.MaLoai == id);

            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(loai);
        }

        [HttpPost]
        public ActionResult Sualoai(Loai loai)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            if (ModelState.IsValid)
            {
                // Cập nhật loại sách đã tồn tại trong cơ sở dữ liệu
                db.Loais.Attach(loai);
                db.Refresh(RefreshMode.KeepCurrentValues, loai);
                db.SubmitChanges();

                return RedirectToAction("Loai");
            }

            return View(loai);
        }
        public ActionResult Xoaloai(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            Loai loai = db.Loais.SingleOrDefault(n => n.MaLoai == id);

            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(loai);
        }

        [HttpPost, ActionName("Xoaloai")]
        public ActionResult Xacnhanxoaloai(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            Loai loai = db.Loais.SingleOrDefault(n => n.MaLoai == id);

            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Xóa loại sách khỏi cơ sở dữ liệu
            db.Loais.DeleteOnSubmit(loai);
            db.SubmitChanges();

            return RedirectToAction("Loai");
        }
        [HttpGet]
        public ActionResult ThemmoiSach()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            // Dua du lieu vao dropdownList
            // Lay ds tu table LOAI, sap xep theo ten loai, cho lay gia tri Ma Loai, hien thi Tenloai
            ViewBag.MaLoai = new SelectList(db.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemmoiSach(SACH sach, HttpPostedFileBase fileupload)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            // Dua du lieu vao dropdownload
            ViewBag.MaLoai = new SelectList(db.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            // Kiem tra duong dan file
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh nền!";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    // Luu ten file, luu y bo sung thu vien
                    var fileName = Path.GetFileName(fileupload.FileName);
                    // Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/images/anhsp/"), fileName);

                    // Kiem tra anh da ton tai hay chua
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại tên tương tự! Vui lòng đặt tên khác!";
                    }
                    else
                    {
                        // Luu hinh anh vao duong dan
                        fileupload.SaveAs(path);
                    }

                    sach.Anhbia = fileName;

                    // Luu vao CSDL
                    db.SACHes.InsertOnSubmit(sach);
                    db.SubmitChanges();

                    return RedirectToAction("Sach");
                }
            }

            return View();
        }
        // Existing code...

        [HttpGet]
        public ActionResult SuaSach(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            // Dua du lieu vao dropdownList
            // Lay ds tu table LOAI, sap xep theo ten loai, cho lay gia tri Ma Loai, hien thi Tenloai
            ViewBag.MaLoai = new SelectList(db.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            // Get the book by ID
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);

            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(sach);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaSach(SACH sach, HttpPostedFileBase fileupload)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            // Dua du lieu vao dropdownload
            ViewBag.MaLoai = new SelectList(db.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            // Get the original book from the database
            SACH originalSach = db.SACHes.SingleOrDefault(n => n.Masach == sach.Masach);

            // Update the properties with new values
            originalSach.Tensach = sach.Tensach;
            originalSach.Giaban = sach.Giaban;
            originalSach.Mota = sach.Mota;
            originalSach.Ngaycapnhat = DateTime.Now;
            originalSach.Soluongton = sach.Soluongton;
            originalSach.MaLoai = sach.MaLoai;
            originalSach.MaNXB = sach.MaNXB;

            // Check if the user uploaded a new image
            if (fileupload != null && fileupload.ContentLength > 0)
            {
                // Luu ten file, luu y bo sung thu vien
                var fileName = Path.GetFileName(fileupload.FileName);
                // Luu duong dan cua file
                var path = Path.Combine(Server.MapPath("~/images/anhsp/"), fileName);

                // Kiem tra anh da ton tai hay chua
                if (System.IO.File.Exists(path))
                {
                    ViewBag.Thongbao = "Hình ảnh đã tồn tại tên tương tự! Vui lòng đặt tên khác!";
                }
                else
                {
                    // Luu hinh anh vao duong dan
                    fileupload.SaveAs(path);
                }

                originalSach.Anhbia = fileName;
            }

            // Luu vao CSDL
            db.SubmitChanges();

            return RedirectToAction("Sach");
        }

        [HttpGet]
        public ActionResult ThemmoiNXB()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ThemmoiNXB(NHAXUATBAN nxb)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            if (ModelState.IsValid)
            {
                db.NHAXUATBANs.InsertOnSubmit(nxb);
                db.SubmitChanges();

                return RedirectToAction("NXB");
            }

            return View();
        }
        [HttpGet]
        public ActionResult SuaNXB(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            NHAXUATBAN nxb = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == id);

            if (nxb == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(nxb);
        }

        [HttpPost]
        public ActionResult SuaNXB(NHAXUATBAN nxb)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            if (ModelState.IsValid)
            {
                db.NHAXUATBANs.Attach(nxb);
                db.Refresh(RefreshMode.KeepCurrentValues, nxb);
                db.SubmitChanges();

                return RedirectToAction("NXB");
            }

            return View(nxb);
        }
        [HttpGet]
        public ActionResult XoaNXB(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            NHAXUATBAN nxb = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == id);

            if (nxb == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(nxb);
        }

        [HttpPost, ActionName("XoaNXB")]
        public ActionResult XacnhanXoaNXB(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            NHAXUATBAN nxb = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == id);

            if (nxb == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            db.NHAXUATBANs.DeleteOnSubmit(nxb);
            db.SubmitChanges();

            return RedirectToAction("NXB");
        }
        public ActionResult QuanLyDonHang()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }
        public ActionResult ChuaThanhToan()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            //lấy ds đơn hàng chưa duyệt
            var lst = db.DONDATHANGs.Where(n => n.Dathanhtoan == false).OrderBy(n => n.Ngaydat);
            return View(lst);
        }

        public ActionResult ChuaGiao()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            //lấy ds đơn hàng chưa giao
            var lstDSDHCG = db.DONDATHANGs.Where(n => n.Tinhtranggiaohang == false && n.Dathanhtoan == true).OrderBy(n => n.Ngaygiao);
            return View(lstDSDHCG);
        }

        public ActionResult DaGiaoDaThanhToan()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            //lấy ds đơn hàng đã giao và thanh toán
            var lstDSDHCG = db.DONDATHANGs.Where(n => n.Tinhtranggiaohang == true && n.Dathanhtoan == true).OrderBy(n => n.Ngaygiao);
            return View(lstDSDHCG);
        }
        [HttpGet]
        public ActionResult DuyetDonHang(int? id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            DONDATHANG model = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);

            if (model == null)
            {
                return HttpNotFound();
            }

            var lstChiTietDH = db.CHITIETDONTHANGs.Where(n => n.MaDonHang == id);
            ViewBag.ListChiTietDH = lstChiTietDH;
            ViewBag.TenKH = model.KHACHHANG.HoTen;
            return View(model);
        }

        [HttpPost]
        public ActionResult DuyetDonHang(int id, DONDATHANG ddh)
        {
            DONDATHANG ddhUpdate = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);

            if (ddhUpdate != null)
            {
                // Update only the required properties
                ddhUpdate.Dathanhtoan = ddh.Dathanhtoan;
                ddhUpdate.Tinhtranggiaohang = ddh.Tinhtranggiaohang;

                db.SubmitChanges();

                var lstChiTietDH = db.CHITIETDONTHANGs.Where(n => n.MaDonHang == id);
                ViewBag.ListChiTietDH = lstChiTietDH;

                return RedirectToAction("QuanLyDonHang ", "Admin");
            }
            else
            {
                return HttpNotFound();
            }
        }

        //Giải phóng dung lượng biến db, để ở cuối controller
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult KhachHangList()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            var customerList = db.KHACHHANGs.ToList();
            return View(customerList);
        }
        public ActionResult ChiTietKhachHang(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            //Lay doi tuong nguoi dung theo ma
            KHACHHANG nguoidung = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = nguoidung.MaKH;
            if (nguoidung == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(nguoidung);

        }
        public ActionResult DonHangKhachHang(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }

            var orders = db.DONDATHANGs.Where(d => d.MaKH == id).ToList();
            return View(orders);
        }
        public ActionResult ThongKeDoanhThuTheoNgay(DateTime? ngay = null)
        {
            if (ngay == null)
            {
                ngay = DateTime.Now.Date; // Mặc định là hôm nay
            }

            var donDatHangs = db.DONDATHANGs
                                .Where(d => d.Ngaydat.Value.Date == ngay.Value.Date && d.Dathanhtoan == true)
                                .ToList();

            // Tính tổng doanh thu và số lượng đơn hàng
            decimal tongDoanhThu = 0;
            int tongSoDon = donDatHangs.Count();

            foreach (var ddh in donDatHangs)
            {
                ddh.Soluongdon = ddh.CHITIETDONTHANGs.Sum(ct => ct.Soluong.Value); // Giả sử bạn có thuộc tính SoLuong trong CHITIETDONTHANG
                ddh.Tongtien = ddh.CHITIETDONTHANGs.Sum(ct => ct.ThanhTien ?? 0); // Giả sử bạn có thuộc tính ThanhTien trong CHITIETDONTHANG

                foreach (var ct in ddh.CHITIETDONTHANGs)
                {
                    tongDoanhThu += ct.ThanhTien ?? 0;
                }
            }

            ViewBag.Ngay = ngay.Value.ToString("yyyy-MM-dd");
            ViewBag.TongSoDon = tongSoDon;
            ViewBag.TongDoanhThu = tongDoanhThu;

            // Trả về danh sách chi tiết đơn hàng
            return View(donDatHangs);
        }
        public ActionResult ThongKeDoanhThuTheoThang(int? thang, int? nam)
        {
            if (!thang.HasValue || !nam.HasValue)
            {
                thang = DateTime.Now.Month; // Lấy tháng hiện tại
                nam = DateTime.Now.Year;    // Lấy năm hiện tại
            }

            // Lấy danh sách đơn hàng của tháng và năm được chọn
            var donDatHangs = db.DONDATHANGs
                                .Where(d => d.Ngaydat.Value.Month == thang && d.Ngaydat.Value.Year == nam && d.Dathanhtoan == true)
                                .ToList();

            decimal tongDoanhThu = 0;
            int tongSoDon = donDatHangs.Count();

            foreach (var ddh in donDatHangs)
            {
                // Tính tổng số lượng và tổng tiền của từng đơn hàng
                ddh.Soluongdon = ddh.CHITIETDONTHANGs.Sum(ct => ct.Soluong.Value);
                ddh.Tongtien = ddh.CHITIETDONTHANGs.Sum(ct => ct.ThanhTien ?? 0);

                // Cộng tổng tiền của từng đơn hàng vào tổng doanh thu
                tongDoanhThu += ddh.Tongtien;
            }

            ViewBag.Thang = thang;
            ViewBag.Nam = nam;
            ViewBag.ListThang = Enumerable.Range(1, 12).ToList(); // Danh sách tháng từ 1 đến 12
            ViewBag.TongSoDon = tongSoDon;
            ViewBag.TongDoanhThu = tongDoanhThu;

            return View(donDatHangs);
        }
        public ActionResult ThongKeDoanhThuTheoNam(int? nam)
        {
            if (!nam.HasValue)
            {
                nam = DateTime.Now.Year; // Lấy năm hiện tại nếu không có giá trị năm được cung cấp
            }

            // Lấy danh sách đơn hàng của năm được chọn
            var donDatHangs = db.DONDATHANGs
                                .Where(d => d.Ngaydat.Value.Year == nam && d.Dathanhtoan == true)
                                .ToList();

            decimal tongDoanhThu = 0;
            int tongSoDon = donDatHangs.Count();

            foreach (var ddh in donDatHangs)
            {
                // Tính tổng số lượng và tổng tiền của từng đơn hàng
                ddh.Soluongdon = ddh.CHITIETDONTHANGs.Sum(ct => ct.Soluong.Value);
                ddh.Tongtien = ddh.CHITIETDONTHANGs.Sum(ct => ct.ThanhTien ?? 0);

                // Cộng tổng tiền của từng đơn hàng vào tổng doanh thu
                tongDoanhThu += ddh.Tongtien;
            }

            ViewBag.Nam = nam;
            ViewBag.ListNam = Enumerable.Range(1900, DateTime.Now.Year - 1900 + 1).Reverse().ToList(); // Danh sách năm từ 1900 đến năm hiện tại
            ViewBag.TongSoDon = tongSoDon;
            ViewBag.TongDoanhThu = tongDoanhThu;

            return View(donDatHangs);
        }

        public ActionResult ThongKeDoanhThu()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }
    }
}
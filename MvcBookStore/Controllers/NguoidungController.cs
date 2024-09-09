using MvcBookStore.Models;
using PagedList;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MvcBookStore.Controllers
{
    public class NguoidungController : Controller
    {
        // GET: Nguoidung
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }

        //POST: Ham Dangky(post) nhan du lieu tu trang Dangky và thuc hien viec thuc hien tao moi du lieu
        [HttpPost]
        public ActionResult Dangky(FormCollection collection, KHACHHANG kh)
        {
            //Gan cac gia tri nguoi dung nhap nhap du lieu cho cac bien
            var hoten = collection["HotenKH"];
            var tendn = collection["TenDN"];
            var dienthoai = collection["Dienthoai"];
            var email = collection["Email"];
            var matkhau = collection["Matkhau"];
            var matkhaunhaplai = collection["Matkhaunhaplai"];
            var diachi = collection["Diachi"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["Ngaysinh"]);
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên khách hàng không được để trống!";
            }
            else if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Tên đăng nhập không thể để trống!";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Mật khẩu không thể để trống!";
            }
            else if (String.IsNullOrEmpty(matkhaunhaplai))
            {
                ViewData["Loi4"] = "Mật khẩu nhập lại không thể để trống!";
            }
            else if (matkhaunhaplai != matkhau)
            {
                ViewData["Loi4"] = "Mật khẩu nhập lại khác mật khẩu";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "Email không thể để trống!";
            }
            else if (String.IsNullOrEmpty(diachi))
            {
                ViewData["Loi6"] = "Địa chỉ không thể để trống!";
            }
            else if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi7"] = "Số điện thoại không thể để trống!";
            }
            if (db.KHACHHANGs.Any(n => n.Taikhoan == tendn))
            {
                ViewData["Loi2"] = "Tên đăng nhập đã được sử dụng. Vui lòng chọn tên đăng nhập khác.";
            }
            // Check if email already exists
            else if (db.KHACHHANGs.Any(n => n.Email == email))
            {
                ViewData["Loi5"] = "Email đã được sử dụng. Vui lòng nhập địa chỉ email khác.";
            }
            // Check if phone number already exists
            else if (db.KHACHHANGs.Any(n => n.DienthoaiKH == dienthoai))
            {
                ViewData["Loi7"] = "Số điện thoại đã được sử dụng. Vui lòng nhập số điện thoại khác.";
            }
            else
            {
                QLBANSACHDataContext db = new QLBANSACHDataContext("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=QLBANSACH;Integrated Security=True;Encrypt=False");
                //Gan gia tri cho doi tuong duoc tao moi (kh)
                kh.HoTen = hoten;
                kh.Taikhoan = tendn;
                kh.Matkhau = matkhau;
                kh.Email = email;
                kh.DiachiKH = diachi;
                kh.DienthoaiKH = dienthoai;
                kh.Ngaysinh = DateTime.Parse(ngaysinh);
                db.KHACHHANGs.InsertOnSubmit(kh);
                db.SubmitChanges();
                return RedirectToAction("Dangnhap");
            }
            return this.Dangky();
        }
        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            //Gan cac gia tri nguoi dung nhap vao cac bien
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Chưa nhập tên đăng nhập!";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Chưa nhập mật khẩu!";
            }
            else
            {
                QLBANSACHDataContext db = new QLBANSACHDataContext("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=QLBANSACH;Integrated Security=True;Encrypt=False");
                //Gan cac doi tuong duoc tao moi(kh)
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.Taikhoan == tendn && n.Matkhau == matkhau);
                if (kh != null)
                {
                    //ViewBag.Thongbao = "Chúc mừng đăng nhập thành công!";
                    Session["Taikhoan"] = kh;
                    Session["TDN"] = kh.HoTen;
                    return RedirectToAction("Index", "BookStore");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng!";
            }
            return View();
        }
        QLBANSACHDataContext db = new QLBANSACHDataContext("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=QLBANSACH;Integrated Security=True;Encrypt=False");
        public ActionResult DonHang(int? page)
        {
            if (Session["TDN"] == null)
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            //return View(db.SACHes.ToList());
            return View(db.DONDATHANGs.ToList().OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ThongTinCaNhan()
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (Session["Taikhoan"] == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Dangnhap", "Nguoidung");
            }

            // Lấy thông tin người dùng từ Session
            KHACHHANG nguoiDung = (KHACHHANG)Session["Taikhoan"];

            // Truyền thông tin người dùng đến view
            return View(nguoiDung);
        }
        [HttpGet]
        public ActionResult CapNhatThongTin()
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }

            // Lấy thông tin người dùng từ Session
            KHACHHANG nguoiDung = (KHACHHANG)Session["Taikhoan"];

            return View(nguoiDung);
        }

        [HttpPost]
        public ActionResult CapNhatThongTin(KHACHHANG nguoiDung)
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }

            // Lấy thông tin người dùng từ Session
            KHACHHANG nguoiDungHienTai = (KHACHHANG)Session["Taikhoan"];

            // Kiểm tra xem ModelState có hợp lệ không
            if (ModelState.IsValid)
            {
                // Cập nhật thông tin mới
                nguoiDungHienTai.HoTen = nguoiDung.HoTen;
                nguoiDungHienTai.Email = nguoiDung.Email;
                nguoiDungHienTai.DiachiKH = nguoiDung.DiachiKH;
                nguoiDungHienTai.DienthoaiKH = nguoiDung.DienthoaiKH;

                // Cập nhật vào CSDL hoặc lưu vào Session nếu sử dụng Session làm đối tượng lưu trữ
                // Ví dụ sử dụng CSDL:
                using (var db = new QLBANSACHDataContext("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=QLBANSACH;Integrated Security=True;Encrypt=False"))
                {
                    var nguoiDungTrongCSDL = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == nguoiDungHienTai.MaKH);

                    if (nguoiDungTrongCSDL != null)
                    {
                        nguoiDungTrongCSDL.Email = nguoiDungHienTai.Email;
                        nguoiDungTrongCSDL.DiachiKH = nguoiDungHienTai.DiachiKH;
                        nguoiDungTrongCSDL.DienthoaiKH = nguoiDungHienTai.DienthoaiKH;

                        db.SubmitChanges();
                    }
                }

                // Cập nhật lại Session
                Session["Taikhoan"] = nguoiDungHienTai;

                // Chuyển hướng về trang thông tin cá nhân
                return RedirectToAction("ThongTinCaNhan", "Nguoidung");
            }

            // Nếu ModelState không hợp lệ, trả về View với dữ liệu và thông báo lỗi
            return View(nguoiDung);
        }
        [HttpGet]
        public ActionResult CapNhatMatKhau()
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }

            // Lấy thông tin người dùng từ Session
            KHACHHANG nguoiDungHienTai = (KHACHHANG)Session["Taikhoan"];

            // Kiểm tra nếu có TempData thì đặt vào ModelState
            if (TempData["ErrorMatKhauCu"] != null)
            {
                ModelState.AddModelError("MatKhauCu", TempData["ErrorMatKhauCu"].ToString());
            }
            if (TempData["ErrorMatKhauMoi"] != null)
            {
                ModelState.AddModelError("MatKhauMoi", TempData["ErrorMatKhauMoi"].ToString());
            }
            if (TempData["ErrorXacNhanMatKhau"] != null)
            {
                ModelState.AddModelError("XacNhanMatKhau", TempData["ErrorXacNhanMatKhau"].ToString());
            }

            return View(nguoiDungHienTai);
        }
        [HttpPost]
        public ActionResult CapNhatMatKhau(string MatKhauCu, string MatKhauMoi, string XacNhanMatKhau)
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if (MatKhauMoi.Length < 6)
            {
                TempData["ErrorMatKhauMoi"] = "Mật khẩu mới tối đa 6 ký tự.";
                return RedirectToAction("CapNhatMatKhau", "Nguoidung");
            }
            // Lấy thông tin người dùng từ Session
            KHACHHANG nguoiDungHienTai = Session["Taikhoan"] as KHACHHANG;

            // Kiểm tra xem nguoiDungHienTai có null không
            if (nguoiDungHienTai == null)
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }

            // Kiểm tra mật khẩu cũ
            if (nguoiDungHienTai.Matkhau != MatKhauCu)
            {
                TempData["ErrorMatKhauCu"] = "Mật khẩu cũ không đúng.";
                TempData["ErrorMessage"] = "Đổi mật khẩu thất bại. Vui lòng kiểm tra lại mật khẩu cũ !.";
                return RedirectToAction("CapNhatMatKhau", "Nguoidung");
            }

            // Kiểm tra mật khẩu mới không trùng với mật khẩu cũ
            if (MatKhauMoi == MatKhauCu)
            {
                TempData["ErrorMatKhauMoi"] = "Mật khẩu mới không được trùng với mật khẩu cũ.";
                TempData["ErrorMessage"] = "Đổi mật khẩu thất bại. Vui lòng kiểm tra lại mật khẩu mới !.";
                return RedirectToAction("CapNhatMatKhau", "Nguoidung");
            }

            // Kiểm tra xác nhận mật khẩu mới
            if (MatKhauMoi != XacNhanMatKhau)
            {
                TempData["ErrorXacNhanMatKhau"] = "Xác nhận mật khẩu mới không chính xác. Vui lòng nhập lại xác nhận mật khẩu mới.";
                TempData["ErrorMessage"] = "Đổi mật khẩu thất bại. Vui lòng kiểm tra lại đã trùng với mật khẩu mới chưa !.";
                return RedirectToAction("CapNhatMatKhau", "Nguoidung");
            }

            // Cập nhật mật khẩu mới
            nguoiDungHienTai.Matkhau = MatKhauMoi;

            // Cập nhật vào CSDL hoặc lưu vào Session nếu sử dụng Session làm đối tượng lưu trữ
            // Ví dụ sử dụng CSDL:
            using (var db = new QLBANSACHDataContext("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=QLBANSACH;Integrated Security=True;Encrypt=False"))
            {
                var nguoiDungTrongCSDL = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == nguoiDungHienTai.MaKH);

                if (nguoiDungTrongCSDL != null)
                {
                    nguoiDungTrongCSDL.Matkhau = nguoiDungHienTai.Matkhau;

                    db.SubmitChanges();
                }
            }

            // Cập nhật lại Session
            Session["Taikhoan"] = nguoiDungHienTai;

            // Thêm thông báo đổi mật khẩu thành công
            TempData["SuccessMessage"] = "Đổi mật khẩu thành công.";

            // Chuyển hướng về trang thông tin cá nhân
            return RedirectToAction("ThongTinCaNhan", "Nguoidung");
        }
        [HttpGet]
        public ActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QuenMatKhau(string PhuongThuc, string ThongTinLayLaiMatKhau)
        {
            if (PhuongThuc == "Email")
            {
                // Kiểm tra thông tin Email có tồn tại trong CSDL không
                var khachHang = db.KHACHHANGs.SingleOrDefault(kh => kh.Email == ThongTinLayLaiMatKhau);

                if (khachHang != null)
                {
                    ViewBag.TenDN = khachHang.Taikhoan;
                    return RedirectToAction("DoiMatKhauMoi", new { TenDN = khachHang.Taikhoan });
                }
            }
            else if (PhuongThuc == "SoDienThoai")
            {
                // Kiểm tra thông tin Số Điện Thoại có tồn tại trong CSDL không
                var khachHang = db.KHACHHANGs.SingleOrDefault(kh => kh.DienthoaiKH == ThongTinLayLaiMatKhau);

                if (khachHang != null)
                {
                    ViewBag.TenDN = khachHang.Taikhoan;
                    return RedirectToAction("DoiMatKhauMoi", new { TenDN = khachHang.Taikhoan });
                }
            }

            // Thông báo lỗi nếu thông tin nhập sai
            ViewBag.ThongBaoQuenMatKhau = "Thông tin nhập không chính xác. Vui lòng kiểm tra lại.";

            return View();
        }

        [HttpGet]
        public ActionResult DoiMatKhauMoi(string TenDN)
        {
            ViewBag.TenDN = TenDN;
            return View();
        }

        [HttpPost]
        public ActionResult DoiMatKhauMoi(string TenDN, string MatKhauMoi, string XacNhanMatKhau)
        {
            // Lấy thông tin người dùng từ CSDL
            var nguoiDung = db.KHACHHANGs.SingleOrDefault(kh => kh.Taikhoan == TenDN);

            if (nguoiDung == null)
            {
                // Người dùng không tồn tại, xử lý lỗi (ví dụ: chuyển hướng hoặc thông báo lỗi)
                return HttpNotFound();
            }

            // Kiểm tra mật khẩu mới và xác nhận mật khẩu
            if (MatKhauMoi != XacNhanMatKhau)
            {
                ViewBag.Thongbao = "Xác nhận mật khẩu mới không chính xác. Vui lòng kiểm tra lại.";
                return View();
            }

            // Cập nhật mật khẩu mới
            nguoiDung.Matkhau = MatKhauMoi;
            db.SubmitChanges();

            ViewBag.Thongbao = "Đổi mật khẩu thành công.";

            return RedirectToAction("Dangnhap");
        }


    }
}
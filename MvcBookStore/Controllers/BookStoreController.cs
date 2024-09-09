using MvcBookStore.Models;
//list
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace MvcBookStore.Controllers
{
    public class BookStoreController : Controller
    {
        //Tao 1 doi tuong chua tona bo CSDL tu dbQLBansach
        QLBANSACHDataContext data = new QLBANSACHDataContext("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=QLBANSACH;Integrated Security=True;Encrypt=False");

        private List<SACH> Laysachmoi()
        {
            //Sap xep giam dan theo Ngaycapnhat , lay gia tri count dong dau
            return data.SACHes.OrderByDescending(a => a.Ngaycapnhat).ToList();
        }
        // GET: BookStore
        public ActionResult Index(int? page, string sortOrder)
        {
            int pageSize = 8;
            int pageNum = (page ?? 1);

            var sach = data.SACHes.AsQueryable(); // Start with all books

            // Add sorting logic
            switch (sortOrder)
            {
                case "price_desc":
                    sach = sach.OrderByDescending(s => s.Giaban);
                    break;
                case "price_asc":
                    sach = sach.OrderBy(s => s.Giaban);
                    break;
                default:
                    sach = sach.OrderByDescending(s => s.Ngaycapnhat); // Default sorting by update date
                    break;
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentPage = pageNum; // Pass the current page to the view

            return View(sach.ToPagedList(pageNum, pageSize));
        }



        //Them chu de voi tung muc nav
        public ActionResult Loai()
        {
            var loai = from cd in data.Loais select cd;
            return PartialView(loai);
        }
        //Them nha xuat ban voi tung muc nav
        public ActionResult Nhaxuatban()
        {
            var nhaxuatban = from nxb in data.NHAXUATBANs select nxb;
            return PartialView(nhaxuatban);
        }
        //Chia san pham theo chu de
        public ActionResult SPTheoloai(int id, int? page)
        {
            //Tao bien quy dinh so san pham tren 1 trang
            int pageSize = 8;
            //Tao bien so trang
            int pageNum = (page ?? 1);
            var sach = from s in data.SACHes where s.MaLoai == id select s;
            return View(sach.ToPagedList(pageNum, pageSize));
        }
        //Chia san pham theo Nha Xuat Ban
        public ActionResult SPTheoNXB(int id, int? page)
        {
            //Tao bien quy dinh so san pham tren 1 trang
            int pageSize = 8;
            //Tao bien so trang
            int pageNum = (page ?? 1);
            var sach = from s in data.SACHes where s.MaNXB == id select s;
            return View(sach.ToPagedList(pageNum, pageSize));
        }
        //Lay thong tin chi tiet sach
        public ActionResult Details(int id)
        {
            var sach = from s in data.SACHes
                       where s.Masach == id
                       select s;
            return View(sach.Single());
        }
        public ActionResult Thoat()
        {
            Session["Taikhoan"] = null;
            return View();
        }
        public ActionResult TimKiem(string search)
        {
            // Thực hiện logic tìm kiếm dựa trên tham số 'search'
            // Ở đây, tôi giả sử bạn muốn tìm kiếm theo Tensach
            var ketQuaTimKiem = data.SACHes.Where(s => s.Tensach.Contains(search)).ToList();

            // Truyền kết quả tìm kiếm đến view
            return View(ketQuaTimKiem);
        }

    }
}
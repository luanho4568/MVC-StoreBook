using System;
using System.Linq;
namespace MvcBookStore.Models
{
    //Tao doi tuong data chua du lieu tu model dbBansach da tao
    public class Giohang
    {
        QLBANSACHDataContext data = new QLBANSACHDataContext("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=QLBANSACH;Integrated Security=True;Encrypt=False");
        public int iMasach { set; get; }
        public string sTensach { set; get; }
        public string sAnhbia { set; get; }
        public Double dDonggia { set; get; }
        public int iSoluong { set; get; }
        public Double dThanhtien
        {
            get { return iSoluong * dDonggia; }
        }
        public Giohang(int Masach)
        {
            iMasach = Masach;
            SACH sach = data.SACHes.Single(n => n.Masach == iMasach);
            sTensach = sach.Tensach;
            sAnhbia = sach.Anhbia;
            dDonggia = double.Parse(sach.Giaban.ToString());
            iSoluong = 1;
        }
    }
}
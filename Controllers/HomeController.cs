using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Đồ_Án_Web.Models;

using PagedList;
using PagedList.Mvc;
namespace Đồ_Án_Web.Controllers
{
    public class HomeController : Controller
    {
        WebBanKinhDataContext data = new WebBanKinhDataContext();
        private List<Kinh> LayKinhMoi(int count)
        {
            //sắp xếp xe cập nhật theo ngay(Ngaynhap) top 6
            return data.Kinhs.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        public ActionResult TrangChu()
        {          
            var KinhMoi = LayKinhMoi(12); //tổng số xe hiện lên trong phần chia trang
            return View(KinhMoi);
        }


        public ActionResult SanPham(int ? page)
        {
            int pageSize = 12;
            int pageNum = (page ?? 1);

            var sp = (from s in data.Kinhs select s).ToList();
            return View(sp.ToPagedList(pageNum, pageSize));
        }

        //private List<Kinh> LayKinhNu(int maloai = 1)
        //{
            
        //    return data.Kinhs.OrderByDescending(a => a.MaLoai==maloai).Take(maloai).ToList();
        //}
        public ActionResult MatKinhNu(int id = 1)
        {
            var knu = from kn in data.Kinhs where kn.MaLoai == id select kn;
            return View(knu);
        }


        public ActionResult MatKinhNam(int id = 2)
        {
            var knam = from kn in data.Kinhs where kn.MaLoai == id select kn;
            return View(knam);
        }

        public ActionResult MatKinhTreEm(int id = 3)
        {
            var kte = from kn in data.Kinhs where kn.MaLoai == id select kn;
            return View(kte);
        }

        /*
        public ActionResult PhuKien()
        {
            var pk = (from s in data.PhuKiens select s).ToList();
            return View(pk);
        }*/
        public ActionResult PhuKien(int ? page)
        {
            int pageSize = 8;
            int pageNum = (page ?? 1);

            var sp = (from s in data.PhuKiens select s).ToList();
            return View(sp.ToPagedList(pageNum, pageSize));
        }


        public ActionResult NuocLauKinh(int id = 3)
        {
            var nlk = from kn in data.PhuKiens where kn.MaLPK == id select kn;
            return View(nlk);
        }


        public ActionResult HopKinh(int id = 2)
        {
            var hk = from kn in data.PhuKiens where kn.MaLPK == id select kn;
            return View(hk);
        }
        public ActionResult DayDeoKinh(int id = 1)
        {
            var dk = from kn in data.PhuKiens where kn.MaLPK == id select kn;
            return View(dk);
        }

        public ActionResult ChiTiet(int id)
        {
            var sp = from ct in data.Kinhs where ct.MaKinh == id select ct;
            return View(sp.Single());
        }

        public ActionResult ChiTietpk(int id)
        {
            var pk = from ct in data.PhuKiens where ct.MaPK == id select ct;
            return View(pk.Single());
        }

        public ActionResult popupSP(int id)
        {
            var sp = from ct in data.Kinhs where ct.MaKinh == id select ct;
            return View(sp.Single());
        }




        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
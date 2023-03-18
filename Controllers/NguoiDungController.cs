using Đồ_Án_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Đồ_Án_Web.Controllers
{
    public class NguoiDungController : Controller
    {
        WebBanKinhDataContext db = new WebBanKinhDataContext();
        // GET: NguoiDung
        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection collection, TTKhachHang tTKhachHang)
        {
            var ht = collection["HoTen"];
            var tk = collection["TaiKhoan"];
            var mk = collection["MatKhau"];
            var xnmk = collection["XacNhanMatKhau"];
            var email = collection["Email"];
            var dtkh = collection["DienThoaiKH"];
            var dckh = collection["DiaChiKH"];
            if (string.IsNullOrEmpty(ht)) {
                ViewData["loi1"] = "Họ tên không được để trống";
            }
            if (string.IsNullOrEmpty(tk))
            {
                ViewData["loi2"] = "Tài khoản không được để trống";
            }
            if (string.IsNullOrEmpty(mk))
            {
                ViewData["loi3"] = "Mật khẩu không được để trống";
            }
            if (string.IsNullOrEmpty(xnmk))
            {
                ViewData["loi4"] = "Xác nhận mật khẩu không được để trống";
            }
            if (string.IsNullOrEmpty(email))
            {
                ViewData["loi5"] = "Email không được để trống";
            }
            if (string.IsNullOrEmpty(dtkh))
            {
                ViewData["loi6"] = "Điện thoại không được để trống";
            }
            if (string.IsNullOrEmpty(dckh))
            {
                ViewData["loi7"] = "Địa chỉ không được để trống";
            }
            else
            {
                tTKhachHang.HoTen = ht;
                tTKhachHang.TaiKhoan = tk;
                tTKhachHang.MatKhau = mk;
                tTKhachHang.Email = email;
                tTKhachHang.DienThoaiKH = dtkh;
                tTKhachHang.DiaChiKH = dckh;
                db.TTKhachHangs.InsertOnSubmit(tTKhachHang);
                db.SubmitChanges();
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            return View();
        }



        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendn = collection["TaiKhoan"];
            var matkhau = collection["MatKhau"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Bạn phải nhập tên đăng nhập";
            }
            if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Bạn phải nhập mật khẩu";
            }
            else
            {
                //Gán giá trị cho đối tượng được tạo mới (kh)
                TTKhachHang kh = db.TTKhachHangs.SingleOrDefault(n => n.TaiKhoan == tendn && n.MatKhau == matkhau);
                if (kh != null)
                {
                    ViewBag.ThongBao = "Đăng nhập thành công!";
                    Session["MaKH"] = kh.MaKH.ToString();
                    Session["TaikhoanKH"] = tendn;
                    Session["Taikhoan"] = kh;
                    return RedirectToAction("GioHang", "GioHang");
                }
                else ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng!";
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session["TaiKhoan"] = null;
            Session.Abandon();
            return RedirectToAction("TrangChu", "Home");
        }

        public ActionResult TTKhachHang(int id)
        {
            var kh = from k in db.TTKhachHangs where k.MaKH == id select k;
            return View(kh.Single());
        }
        //sửa tt khách
        [HttpGet]
        public ActionResult SuaTTKhachHang(int id)
        {
            //LẤY RA ĐỐI TƯỢNG SẢN PHẨM THEO MÃ
            TTKhachHang ttKhachHang = db.TTKhachHangs.SingleOrDefault(n => n.MaKH == id);
            if (ttKhachHang == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(ttKhachHang);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaTTKhachHang(int id, FormCollection collection, HttpPostedFileBase fileUpload)
        {
            var ttKhachHang = db.TTKhachHangs.FirstOrDefault(n => n.MaKH == id);
            ttKhachHang.MaKH = id; 
            //Them vao CSDL
            if (ModelState.IsValid)
            {   
                ttKhachHang.MaKH = id;
                //luu vao CSDL
                UpdateModel(ttKhachHang);
                db.SubmitChanges();
                return RedirectToAction("DangNhap");
            }
            return this.SuaTTKhachHang(id);         
        }
    }
}

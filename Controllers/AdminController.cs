using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Đồ_Án_Web.Models;
using PagedList;
using PagedList.Mvc;

namespace Đồ_Án_Web.Controllers
{
    public class AdminController : Controller
    {
        WebBanKinhDataContext db = new WebBanKinhDataContext(); 
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] != null)
            {
                return View();
            }
            else
            {
                //Login:hàm, admin:Controller
                return RedirectToAction("Login", "Admin");
            }
        }

        public static string CreateMD5(string matkhau)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(matkhau);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToBase64String(hashBytes); // .NET 5 +

                //string s = Convert.ToBase64String(hashBytes);
                //return s;
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            //gán các giá trị người dùng nhập liệu cho các biến
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";

            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                //gán  giá trị cho đối tượng được tạo mới(ad)
                Admin nv = db.Admins.SingleOrDefault(n => n.TK == tendn && n.MK == matkhau);
                if (nv != null)
                {
                    //ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoanadmin"] = nv;
                    Session["TKAdmin"] = tendn;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();

        }
        public ActionResult Logout()
        {
            Session["TaiKhoanadmin"] = null;
            Session.Abandon();
            return RedirectToAction("Login", "Admin");
        }

        //Kính
        public ActionResult Kinh(int ? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.Kinhs.ToList().OrderBy(n => n.MaKinh).ToPagedList(pageNumber, pageSize));
        }     

        //Thêm kính
        [HttpGet]
        public ActionResult Themmoikinh()
        {
            ////luu ten file, luu y bo sung thu vien using system.IO
            //var fileName = Path.GetFileName(fileUpload.FileName);
            ////luu duong dan cua file
            //var path = Path.Combine(Server.MapPath("~/img"), fileName);
            ////kiem tra hinh anh ton tai chua
            //if (System.IO.File.Exists(path))
            //{
            //    ViewBag.ThongBao = "Hình Ảnh Đã Tồn Tại!!";
            //}
            //else
            //{
            //    //luu hinh anh vao duong dan
            //    fileUpload.SaveAs(path);
            //}

            ViewBag.MaLoai = new SelectList(db.LoaiKinhs.ToList().OrderBy(n => n.TenLoaiKinh), "MaLoai", "TenLoaiKinh");
            ViewBag.MaHSX = new SelectList(db.HangSanXuats.ToList().OrderBy(n => n.TenHSX), "MaHSX", "TenHSX");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemmoiKinh(Kinh kinh, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaLoai = new SelectList(db.LoaiKinhs.ToList().OrderBy(n => n.TenLoaiKinh), "MaLoai", "TenLoaiKinh");
            ViewBag.MaHSX = new SelectList(db.HangSanXuats.ToList().OrderBy(n => n.TenHSX), "MaHSX", "TenHSX");

            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh ";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                //luu ten file
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/img"), fileName);
                    //kiem tra hinh anh ton tai chua
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        // luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    kinh.AnhBia = fileName;
                    //luu vao CSDL
                    db.Kinhs.InsertOnSubmit(kinh);
                    db.SubmitChanges();
                }


                return RedirectToAction("Kinh");

            }
        }
        //Chi tiet
        public ActionResult ChitietKinh(int id)
        {
            Kinh kinh = db.Kinhs.SingleOrDefault(n => n.MaKinh == id);
            ViewBag.MaKinh = kinh.MaKinh;
            if (kinh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kinh);
        }

        //Xóa kính
        [HttpGet]
        public ActionResult XoaKinh(int id)
        {
            Kinh kinh = db.Kinhs.SingleOrDefault(n => n.MaKinh == id);
            ViewBag.MaKinh = kinh.MaKinh;
            if (kinh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kinh);

        }
        [HttpPost, ActionName("XoaKinh")]
        public ActionResult XacnhanxoaKinh(int id)
        {
            //LẤY RA ĐỐI TƯỢNG SẢN PHẨM THEO MÃ
            Kinh kinh = db.Kinhs.SingleOrDefault(n => n.MaKinh == id);
            ViewBag.MaKinh = kinh.MaKinh;
            if (kinh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Kinhs.DeleteOnSubmit(kinh);
            db.SubmitChanges();
            return RedirectToAction("Kinh");
        }

        //Sửa kính
        [HttpGet]
        public ActionResult SuaKinh(int id)
        {
            //LẤY RA ĐỐI TƯỢNG SẢN PHẨM THEO MÃ
            Kinh kinh = db.Kinhs.SingleOrDefault(n => n.MaKinh == id);
            if (kinh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaLoai = new SelectList(db.LoaiKinhs.ToList().OrderBy(n => n.TenLoaiKinh), "MaLoai", "TenLoaiKinh");
            ViewBag.MaHSX = new SelectList(db.HangSanXuats.ToList().OrderBy(n => n.TenHSX), "MaHSX", "TenHSX");
            return View(kinh);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaKinh(int id, FormCollection collection, HttpPostedFileBase fileUpload)
        {
            var kinh = db.Kinhs.FirstOrDefault(n => n.MaKinh == id);
            kinh.MaKinh = id;
            //Dua du lieu vao dropdownload
            ViewBag.MaLoai = new SelectList(db.LoaiKinhs.ToList().OrderBy(n => n.TenLoaiKinh), "MaLoai", "TenLoaiKinh");
            ViewBag.MaHSX = new SelectList(db.HangSanXuats.ToList().OrderBy(n => n.TenHSX), "MaHSX", "TenHSX");
            //kiem tra duong dan file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn hãng sản xuất";
                return View(kinh);
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //luu ten file
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/img"), fileName);
                    //kiem tra hinh anh ton tai chua
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        // luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    kinh.AnhBia = fileName;
                    kinh.MaKinh = id;
                    //luu vao CSDL
                    UpdateModel(kinh);
                    db.SubmitChanges();
                    return RedirectToAction("Kinh");
                }
                return this.SuaKinh(id);

            }
        }

        //Phụ kiện
        public ActionResult PhuKien(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.PhuKiens.ToList().OrderBy(n => n.MaPK).ToPagedList(pageNumber, pageSize));
        }
        //Thêm phụ kiện
        [HttpGet]
        public ActionResult ThemmoiPhukien()
        {
            ViewBag.MaLPK = new SelectList(db.LoaiPhuKiens.ToList().OrderBy(n => n.TenLoaiPhuKien), "MaLPK", "TenLoaiPhuKien");
            ViewBag.MaHSX = new SelectList(db.HangSanXuats.ToList().OrderBy(n => n.TenHSX), "MaHSX", "TenHSX");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemmoiPhukien(PhuKien phukien, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaLPK = new SelectList(db.LoaiPhuKiens.ToList().OrderBy(n => n.TenLoaiPhuKien), "MaLPK", "TenLoaiPhuKien");
            ViewBag.MaHSX = new SelectList(db.HangSanXuats.ToList().OrderBy(n => n.TenHSX), "MaHSX", "TenHSX");

            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh ";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                //luu ten file
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/img"), fileName);
                    //kiem tra hinh anh ton tai chua
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        // luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    phukien.AnhBiaPK = fileName;
                    //luu vao CSDL
                    db.PhuKiens.InsertOnSubmit(phukien);
                    db.SubmitChanges();
                }


                return RedirectToAction("PhuKien");

            }
        }
        //Chi tiết
        public ActionResult ChitietPK(int id)
        {
            PhuKien phukien = db.PhuKiens.SingleOrDefault(n => n.MaPK == id);
            ViewBag.MaPK = phukien.MaPK;
            if (phukien == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(phukien);
        }
        //Xóa phụ kiện
        [HttpGet]
        public ActionResult XoasPhukien(int id)
        {
            PhuKien phukien = db.PhuKiens.SingleOrDefault(n => n.MaPK == id);
            ViewBag.MaPK = phukien.MaPK;
            if (phukien == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(phukien);

        }
        [HttpPost, ActionName("XoasPhukien")]
        public ActionResult XacnhanxoaPK(int id)
        {
            //LẤY RA ĐỐI TƯỢNG SẢN PHẨM THEO MÃ
            PhuKien phukien = db.PhuKiens.SingleOrDefault(n => n.MaPK == id);
            ViewBag.MaPK = phukien.MaPK;
            if (phukien == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.PhuKiens.DeleteOnSubmit(phukien);
            db.SubmitChanges();
            return RedirectToAction("PhuKien");
        }

        //Sửa phụ kiện
        [HttpGet]
        public ActionResult SuaPhukien(int id)
        {
            //LẤY RA ĐỐI TƯỢNG SẢN PHẨM THEO MÃ
            PhuKien phuKien = db.PhuKiens.SingleOrDefault(n => n.MaPK == id);
            if (phuKien == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaLPK = new SelectList(db.LoaiPhuKiens.ToList().OrderBy(n => n.TenLoaiPhuKien), "MaLPK", "TenLoaiPhuKien");
            ViewBag.HangSanXuat = new SelectList(db.HangSanXuats.ToList().OrderBy(n => n.TenHSX), "MaHSX", "TenHSX");
            return View(phuKien);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaPhukien(int id, FormCollection collection, HttpPostedFileBase fileUpload)
        {
            var phuKien = db.PhuKiens.FirstOrDefault(n => n.MaPK == id);
            phuKien.MaPK = id;
            //Dua du lieu vao dropdownload
            ViewBag.MaLPK = new SelectList(db.LoaiPhuKiens.ToList().OrderBy(n => n.TenLoaiPhuKien), "MaLPK", "TenLoaiPhuKien");
            ViewBag.HangSanXuat = new SelectList(db.HangSanXuats.ToList().OrderBy(n => n.TenHSX), "MaHSX", "TenHSX");
            //kiem tra duong dan file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn hãng sản xuất";
                return View(phuKien);
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //luu ten file
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/img"), fileName);
                    //kiem tra hinh anh ton tai chua
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        // luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                    }
                    phuKien.AnhBiaPK = fileName;
                    phuKien.MaPK = id;
                    //luu vao CSDL
                    UpdateModel(phuKien);
                    db.SubmitChanges();
                    return RedirectToAction("PhuKien");
                }
                return this.SuaPhukien(id);

            }
        }

        //khách hang
        public ActionResult Khach(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.TTKhachHangs.ToList().OrderBy(n => n.MaKH).ToPagedList(pageNumber, pageSize));
        }
        //chi tiet khách hàng
        public ActionResult ChiTietKhach(int id)
        {
            //lấy ra đối tượng sách theo mã
            TTKhachHang kh = db.TTKhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = kh.MaKH;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }
        //xóa khách
        [HttpGet]
        public ActionResult XoaKhach(int id)
        {
            //lấy ra đối tượng khách cần xóa
            TTKhachHang kh = db.TTKhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = kh.MaKH;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }
        [HttpPost, ActionName("XoaKhach")]
        public ActionResult XacNhanXoaKhach(int id)
        {
            //lấy ra đối tượng khách cần xóa
            TTKhachHang kh = db.TTKhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = kh.MaKH;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.TTKhachHangs.DeleteOnSubmit(kh);
            db.SubmitChanges();
            return RedirectToAction("Khach");
        }

        //Tài khoản
        public ActionResult TkAdmin(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.Admins.ToList().OrderBy(n => n.TK).ToPagedList(pageNumber, pageSize));
        }
        //xÓA tài khoản
        [HttpGet]
        public ActionResult XoaAdmin(string id)
        {
            //lấy ra đối tượng khách cần xóa
            Admin ad = db.Admins.SingleOrDefault(n => n.TK == id);
            ViewBag.TK = ad.TK;
            if (ad == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(ad);
        }
        [HttpPost, ActionName("XoaAdmin")]
        public ActionResult XacNhanXoaAdmin(string id)
        {
            //lấy ra đối tượng khách cần xóa
            Admin ad = db.Admins.SingleOrDefault(n => n.TK == id);
            ViewBag.TK = ad.TK;
            if (ad == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Admins.DeleteOnSubmit(ad);
            db.SubmitChanges();
            return RedirectToAction("TkAdmin");
        }
        [HttpGet]

        //Thêm admin
        public ActionResult ThemAdmin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemAdmin(Admin admin)
        {
            //luu vao CSDL
            db.Admins.InsertOnSubmit(admin);
            db.SubmitChanges();
            return RedirectToAction("TkAdmin");
        }
        //sửa admin
        [HttpGet]
        public ActionResult SuaAdmin(string id)
        {
            //LẤY RA ĐỐI TƯỢNG SẢN PHẨM THEO MÃ
            Admin admin = db.Admins.SingleOrDefault(n => n.TK == id);
            if (admin == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            return View(admin);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaAdmin(string id, FormCollection collection, HttpPostedFileBase fileUpload)
        {
            var admin = db.Admins.FirstOrDefault(n => n.TK == id);
            admin.TK = id;
            //Them vao CSDL
            if (ModelState.IsValid)
            {
                admin.TK = id;
                //luu vao CSDL
                UpdateModel(admin);
                db.SubmitChanges();
                return RedirectToAction("TkAdmin");
            }
            return this.SuaAdmin(id);
        }
    }
}
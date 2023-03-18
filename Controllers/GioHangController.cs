using Đồ_Án_Web.Models;
using Đồ_Án_Web.Others;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Đồ_Án_Web.Controllers
{
    public class GioHangController : Controller
    {
        WebBanKinhDataContext data = new WebBanKinhDataContext();
        //Lấy giỏ hàng
        public List<GioHang> Laygiohang()
        {
            List<GioHang> listGiohang = Session["GioHang"] as List<GioHang>;
            if (listGiohang == null)
            {
                listGiohang = new List<GioHang>();
                Session["GioHang"] = listGiohang;
            }
            return listGiohang;
        }
        //thêm kính vào giỏ hàng
        public ActionResult Themgiohang(int makinh, string strURL)
        {
            List<GioHang> listGiohang = Laygiohang();
            GioHang kinh = listGiohang.Find(n => n.makinh == makinh);
            if (kinh == null)
            {
                kinh = new GioHang(makinh);
                listGiohang.Add(kinh);
                return Redirect(strURL);
            }
            else
            {
                kinh.soluong++;
                return Redirect(strURL);         
            }
        }
        //tổng số lượng
        private int TongSoLuong()
        {
            int iTSL = 0;
            List<GioHang> listGiohang = Session["GioHang"] as List<GioHang>;
            if (listGiohang != null)
            {
                iTSL = listGiohang.Sum(n => n.soluong);
            }
            return iTSL;
        }
        //tính tổng tiền
        private double TongTien()
        {
            double iTT = 0;
            List<GioHang> listGiohang = Session["GioHang"] as List<GioHang>;
            if (listGiohang != null)
            {
                iTT = listGiohang.Sum(n => n.thanhtien);
            }
            return iTT;
        }

        public ActionResult ConfirmDH()
        {
            return View();
        }

        public ActionResult GioHang()
        {
            List<GioHang> listGiohang = Laygiohang();
            if (listGiohang.Count == 0)
            {
                return RedirectToAction("TrangChu", "Home");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
           
            return View(listGiohang);
        }

        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }

        public ActionResult XoaGioHang(int MaSP)
        {
            List<GioHang> listGiohang = Laygiohang();
            GioHang kinh = listGiohang.SingleOrDefault(n => n.makinh == MaSP);
            if (kinh != null)
            {
                listGiohang.RemoveAll(n => n.makinh == MaSP);
                return RedirectToAction("GioHang");
            }
            if (listGiohang.Count == 0)
            {
                return RedirectToAction("TrangChu", "Home");
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult UpdateGioHang(int MaSP, FormCollection f)
        {
            List<GioHang> listGiohang = Laygiohang();
            GioHang kinh = listGiohang.SingleOrDefault(n => n.makinh == MaSP);
            if (kinh != null)
            {
                kinh.soluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaAll()
        {
            List<GioHang> listGiohang = Laygiohang();
            listGiohang.Clear();
            return RedirectToAction("TrangChu", "Home");
        }



        [HttpGet]
        public ActionResult DatHangOL()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("TrangChu", "Home");
            }

            List<GioHang> listGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(listGiohang);
        }

        [HttpPost]
        public ActionResult DatHangOL(FormCollection collection)
        {
            DonDatHangg ddh = new DonDatHangg();

            TTKhachHang kh = (TTKhachHang)Session["TaiKhoan"];
            List<GioHang> listGiohang = Laygiohang();
            var HoTen = collection["HoTen"];
            var NgayGiao = collection["NgayGiao"];
            var SDTKH = collection["SDTKH"];
            var Email = collection["Email"];
            var DiaChi = collection["DiaChi"];
            ddh.MaKH = kh.MaKH;
            ddh.HoTen = HoTen;
            ddh.NgayDH = DateTime.Now;
            ddh.NgayGiao = NgayGiao;
            ddh.SDTKH = SDTKH;
            ddh.Email = Email;
            ddh.DiaChi = DiaChi;
            data.DonDatHanggs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            foreach (var item in listGiohang)
            {
                CTDonHang ctdh = new CTDonHang();
                ctdh.MaDH = ddh.MaDH;
                ctdh.MaKinh = item.makinh;
                ctdh.SoLuong = item.soluong;
                ctdh.DonGia = (decimal)item.thanhtien;
                data.CTDonHangs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            //Add code dưới vào trong hàm payment (thanh toán đơn hàng) của các bạn
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];
            double tt = TongTien();
            string totals = (tt * 100).ToString(); //total là tổng của session giỏ hàng

            XuLy pay = new XuLy();

            //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Version", "2.1.0");

            //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_Command", "pay");

            //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_TmnCode", tmnCode);

            //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            //totals là value đã ép kiểu sang kiểu chuỗi ở phía trên
            pay.AddRequestData("vnp_Amount", totals);

            //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_BankCode", "");

            //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));

            //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_CurrCode", "VND");

            //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_IpAddr", ChuyenDoi.GetIpAddress());

            //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_Locale", "vn");

            //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng");

            //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_OrderType", "other");

            //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);

            //mã hóa đơn
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            Session["GioHang"] = null;
            return Redirect(paymentUrl);

        }






        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("TrangChu", "Home");
            }

            List<GioHang> listGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(listGiohang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            DonDatHangg ddh = new DonDatHangg();
           
            TTKhachHang kh = (TTKhachHang)Session["TaiKhoan"];
            List<GioHang> listGiohang = Laygiohang();          
            var HoTen = collection["HoTen"];
            var NgayGiao = collection["NgayGiao"];          
            var SDTKH = collection["SDTKH"];
            var Email = collection["Email"];
            var DiaChi = collection["DiaChi"];
            ddh.MaKH = kh.MaKH;
            ddh.HoTen = HoTen;
            ddh.NgayDH = DateTime.Now;
            ddh.NgayGiao = NgayGiao;
            ddh.SDTKH = SDTKH;
            ddh.Email = Email;
            ddh.DiaChi = DiaChi;
            data.DonDatHanggs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            foreach (var item in listGiohang)
            {
                CTDonHang ctdh = new CTDonHang();
                ctdh.MaDH = ddh.MaDH;
                ctdh.MaKinh = item.makinh;
                ctdh.SoLuong = item.soluong;
                ctdh.DonGia = (decimal)item.thanhtien;
                data.CTDonHangs.InsertOnSubmit(ctdh);
            }       
            data.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("ConfirmDH", "GioHang");
        }


        // GET: VNPay

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                XuLy pay = new XuLy();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }
                //mã hóa đơn
                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef"));

                //mã giao dịch tại hệ thống VNPAY
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo"));

                //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
                //hash của dữ liệu trả về
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                //check chữ ký đúng hay không?
                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret);

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }
    }
}
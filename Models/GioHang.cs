using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Đồ_Án_Web.Models
{
    public class GioHang
    {
        WebBanKinhDataContext data = new WebBanKinhDataContext();
        public int makinh { get; set; }
        //public int mapk { get; set; }

        public string tenkinh { get; set; }
        //public string tenpk { get; set; }

        public string anhbia { get; set; }
        //public string anhbiapk { get; set; }

        public Double giaban { get; set; }
        //public Double giabanpk { get; set; }

        public int soluong { get; set; }
        //public int soluongpk { get; set; }


        public Double thanhtien
        {
            get { return soluong * giaban; }
        }
        //public Double thanhtienpk
        //{
        //    get { return soluongpk * giabanpk; }
        //}
        public GioHang(int MaKinh)
        {
            makinh = MaKinh;
            Kinh kinh = data.Kinhs.Single(n => n.MaKinh == makinh);
            tenkinh = kinh.TenKinh;
            anhbia = kinh.AnhBia;
            giaban = double.Parse(kinh.GiaBan.ToString());
            soluong = 1;

        }

        //public GioHang(int MaPK)
        //{
        //    mapk = MaPK;
        //    PhuKien phuKien = data.PhuKiens.Single(n => n.MaPK == mapk);
        //    tenpk = phuKien.TenPhuKien;
        //    anhbiapk = phuKien.AnhBiaPK;
        //    giabanpk = double.Parse(phuKien.GiaBanPK.ToString());
        //    soluongpk = 1;
        //}
    }
}
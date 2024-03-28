using DoAn_CN.Models;
using Newtonsoft.Json.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_CN.Controllers
{
    public class SaleController : Controller
    {
        dbDoAnCNDataContext data = new dbDoAnCNDataContext();
        private List<SanPham> ProductNew(int count)
        {
            return data.SanPhams.OrderByDescending(a => a.NgayThem).Take(count).ToList();
        }
        public ActionResult Index()
        {
            var SPmoi = ProductNew(12);
            return View(SPmoi);
        }
        public ActionResult AllProduct(int? page)
        {
            int pagesize = 12;
            int pagenum = (page ?? 1);
            var Allproduct = from ALL in data.SanPhams select ALL;
            return View(Allproduct.ToPagedList(pagenum, pagesize));
        }
        public ActionResult SPTheoLoai(int id, int? page)
        {
            int pagesize = 12;
            int pagenum = (page ?? 1);
            var theoloai = from sp in data.SanPhams where sp.idLoai == id select sp;
            return View(theoloai.ToPagedList(pagenum, pagesize));
        }
        public ActionResult chitietSP(int id)
        {
            var CTSP = from CT in data.SanPhams where CT.idSP == id select CT;
            return View(CTSP.Single());
        }
        [HttpPost]
        public ActionResult Search(string keyword)
        {

            var all = data.SanPhams.Where(x => x.TenSP.Contains(keyword));

            return View(all);
        }
        /*=======================================================
        ========================Giỏ hàng=========================
        =======================================================*/
        public List<GioHang> Laygiohang()
        {

            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
     
        public ActionResult Themgiohang(int id, string strURL)
        {
     
            List<GioHang> lstGiohang = Laygiohang();
      
            GioHang SP = lstGiohang.Find(n => n.iidSP == id);
            if (SP == null)
            {
                SP = new GioHang(id);
                lstGiohang.Add(SP);
                return Redirect(strURL);
            }
            else
            {
                SP.iSoluong++;
                return Redirect(strURL);
            }
        }
    
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }
    
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGiohang = Session["GioHang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }
     
        public ActionResult GioHang()
        {
            if (Session["tk"] == null || Session["tk"] == "")
                return RedirectToAction("Index", "NguoiDung");
            else
            {

                List<GioHang> lstGiohang = Laygiohang();
                if (lstGiohang.Count == 0)
                    return RedirectToAction("Index", "Sale");
                else
                {
                    ViewBag.TongSoLuong = TongSoLuong();
                    ViewBag.Tongtien = TongTien();
                    return View(lstGiohang);
                }
            }
        }
     
        public ActionResult GioHangPartial()
        {
            ViewBag.Tongtien = TongTien();
            ViewBag.TongSoLuong = TongSoLuong();
            return PartialView();
        }
       
        public ActionResult Xoa1SP(int id)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iidSP == id);
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.iidSP == id);
                return RedirectToAction("ViewGH");
            }
            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "TDTStore");
            }
            return RedirectToAction("ViewGH");
        }
        //Xóa All Sản phẩm trong giỏ hàng 
        public ActionResult XoaAll()
        {
            List<GioHang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "TDTStore");
        }
        //cập nhật thông tin 1 sản phẩm trong giỏ hàng
        public ActionResult Capnhat(int id, FormCollection collection)
        {
            List<GioHang> lstGiohang = Laygiohang();
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.iidSP == id);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(collection["txtSoluong"].ToString());
            }
            return RedirectToAction("ViewGH");
        }
        public ActionResult ViewGH()
        {
            if (Session["tk"] == null || Session["tk"] == "")
                return RedirectToAction("Index", "NguoiDung");
            else
            {

                List<GioHang> lstGiohang = Laygiohang();
                if (lstGiohang.Count == 0)
                    return RedirectToAction("Index", "Sale");
                else
                {
                    ViewBag.TongSoLuong = TongSoLuong();
                    ViewBag.Tongtien = TongTien();
                    return View(lstGiohang);
                }
            }
        }
        public ActionResult GioHangMomo()
        {
            if (Session["tk"] == null || Session["tk"] == "")
                return RedirectToAction("Index", "NguoiDung");
            else
            {

                List<GioHang> lstGiohang = Laygiohang();
                if (lstGiohang.Count == 0)
                    return RedirectToAction("Index", "Sale");
                else
                {
                    ViewBag.TongSoLuong = TongSoLuong();
                    ViewBag.Tongtien = TongTien();
                    return View(lstGiohang);
                }
            }
        }
        public ActionResult GioHangVNPay()
        {
            if (Session["tk"] == null || Session["tk"] == "")
                return RedirectToAction("Index", "NguoiDung");
            else
            {

                List<GioHang> lstGiohang = Laygiohang();
                if (lstGiohang.Count == 0)
                    return RedirectToAction("Index", "Sale");
                else
                {
                    ViewBag.TongSoLuong = TongSoLuong();
                    ViewBag.Tongtien = TongTien();
                    return View(lstGiohang);
                }
            }
        }
        public ActionResult Dathang()
        {
            DonDatHang ddh = new DonDatHang();
            ThanhVien Cus = (ThanhVien)Session["tk"];
            List<GioHang> gh = Laygiohang();

            ddh.NguoiNhan = Cus.HoTen;
            ddh.SDT = Cus.DienthoaiKH;
            ddh.DiaChiNhan = Cus.DiachiKH;
            ddh.NgayDat = DateTime.Now;
            ddh.NgayGiao = DateTime.Today.AddDays(4);

            ddh.idKH = Cus.id;
            ddh.Datthanhtoan = "Chưa thanh toán";
            ddh.idTrangThai = int.Parse("1");
            data.DonDatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
           
            foreach (var item in gh)
            {
                CT_DDH ctddh = new CT_DDH();
                ctddh.idDDH = ddh.idDDH;
                ctddh.idSP = item.iidSP;
                ctddh.SL = item.iSoluong;
                ctddh.DonGia = item.dThanhtien;
                data.CT_DDHs.InsertOnSubmit(ctddh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Index", "Sale");

        }
        public ActionResult DH_ThanhToanOnl()
        {
            DonDatHang ddh = new DonDatHang();
            ThanhVien Cus = (ThanhVien)Session["tk"];
            List<GioHang> gh = Laygiohang();

            ddh.NguoiNhan = Cus.HoTen;
            ddh.SDT = Cus.DienthoaiKH;
            ddh.DiaChiNhan = Cus.DiachiKH;
            ddh.NgayDat = DateTime.Now;
            ddh.NgayGiao = DateTime.Today.AddDays(4);

            ddh.idKH = Cus.id;
            ddh.Datthanhtoan = "Đã thanh toán";
            ddh.idTrangThai = int.Parse("1");
            data.DonDatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            
            foreach (var item in gh)
            {
                CT_DDH ctddh = new CT_DDH();
                ctddh.idDDH = ddh.idDDH;
                ctddh.idSP = item.iidSP;
                ctddh.SL = item.iSoluong;
                ctddh.DonGia = item.dThanhtien;
                data.CT_DDHs.InsertOnSubmit(ctddh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("ConfirmPaymentClient", "Sale");

        }
        public ActionResult Xacnhan()
        {
            var SPmoi = ProductNew(12);
            return View(SPmoi);
        }
      
        public ActionResult ConfirmPaymentClient()
        {
           
            return View();
        }

        [HttpPost]
        public void SavePayment()
        {
            DonDatHang ddh = new DonDatHang();
            ThanhVien Cus = (ThanhVien)Session["tk"];
            List<GioHang> gh = Laygiohang();

            ddh.NguoiNhan = Cus.HoTen;
            ddh.SDT = Cus.DienthoaiKH;
            ddh.DiaChiNhan = Cus.DiachiKH;
            ddh.NgayDat = DateTime.Now;
            ddh.NgayGiao = DateTime.Today.AddDays(4);

            ddh.idKH = Cus.id;
            ddh.Datthanhtoan = "Đã thanh toán";
            ddh.idTrangThai = int.Parse("1");
            data.DonDatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();

            foreach (var item in gh)
            {
                CT_DDH ctddh = new CT_DDH();
                ctddh.idDDH = ddh.idDDH;
                ctddh.idSP = item.iidSP;
                ctddh.SL = item.iSoluong;
                ctddh.DonGia = item.dThanhtien;
                data.CT_DDHs.InsertOnSubmit(ctddh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
        }
        /*==================================================================================*/
        /*==============================Thanh toán bằng VNPay===============================*/
        
        /*==================================================================================*/
        public ActionResult LichSuMH(int id)
        {
            if (Session["tk"] == null)
                return RedirectToAction("LogIn", "NguoiDung");
            else
            {
                var LS = from ls in data.CT_DDHs.OrderByDescending(n => n.idDDH) where ls.DonDatHang.idKH == id select ls;
                return View(LS);
            }
        }
         
    }
}
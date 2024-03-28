using DoAn_CN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace DoAn_CN.Controllers
{
    public class HomeController : Controller
    {
        dbDoAnCNDataContext data = new dbDoAnCNDataContext();
        public ActionResult Index()
        {
            var BVnew = New(3);
            return View(BVnew);
        }
        [HttpGet]
        /*=========================================Bài viết=======================================*/
        public ActionResult NewsDM(int id, int? page)
        {
            int pagesize = 12;
            int pagenum = (page ?? 1);
            var news = from BA in data.BaiViet_Admins where BA.IdDM == id select BA;
            return View(news.ToPagedList(pagenum, pagesize));
        }
        public ActionResult Baiviet(int id)
        {
            var view = from v in data.BaiViet_Admins where v.id == id select v;
            return View(view.Single());
        }
        private List<BaiViet_Admin> New(int count)
        {
            return data.BaiViet_Admins.OrderByDescending(a => a.NgayThem).Take(count).ToList();
        }
        public ActionResult NewsNew()
        {
            var BVnew = New(4);
            return View(BVnew);
        }

        /*========================================End bài viết ============================================
         
         =========================================Liên Hệ=======================================*/
        public ActionResult LienHe()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LienHe(FormCollection collection, LienHe LH)
        {
            var hoten = collection["HoTen"];
            var sdt = collection["SDT"];
            var email = collection["Email"];
            var noidung = collection["NoiDung"];

            if (string.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Vui lòng nhập đầy đủ họ tên.";
            }
            else if (string.IsNullOrEmpty(email))
            {
                ViewData["Loi2"] = "Vui lòng nhập số điện thoại.";
            }
            else if (string.IsNullOrEmpty(sdt))
            {
                ViewData["Loi3"] = "Vui lòng nhập Email.";
            }
            else if (string.IsNullOrWhiteSpace(noidung))
            {
                ViewData["Loi4"] = "Vui lòng nhập nội dung.";
            }
            else
            {
                //Save về Database
                LH.HoTen = hoten;
                LH.SDT = sdt;
                LH.Email = email;
                LH.NoiDung = noidung;
                data.LienHes.InsertOnSubmit(LH);
                data.SubmitChanges();
                return RedirectToAction("Index", "Home");
            }
            return LienHe();
        }

    }
}
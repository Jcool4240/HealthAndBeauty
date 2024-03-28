using DoAn_CN.Models;
using Facebook;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn_CN.Controllers
{
    public class NguoiDungController : Controller
    {        
        dbDoAnCNDataContext data = new dbDoAnCNDataContext();
        
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppID"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecrect"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppID"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });
            if (result == null)
            {
                return View("LoginFailed");
            }
            var accessToken = result.access_token;
            fb.AccessToken = accessToken;
            if (!string.IsNullOrEmpty(accessToken))
            {
                dynamic me = fb.Get("me?fields = fisrt_name, middle_name, last_name, id");
                string username = me.name;
                string taikhoan = me.id;

                var user = new ThanhVien();
                user.HoTen = username;
                user.Taikhoan = taikhoan;
                user.Matkhau = "123456";
                user.Email = "taikhoan@gmail.com";
                user.DiachiKH = "Công ty";
                user.DienthoaiKH = "0901111111";

                var ngaysinh = String.Format("{0:MM/dd/yyyy}", "01/01/2000");
                user.Ngaysinh = DateTime.Parse(ngaysinh);
               
                var kq = InsertForFacebook(user);
                if (kq > 0)
                {
                    var userSession = new ThanhVien();
                    userSession.Taikhoan = user.HoTen;
                    userSession.id = kq;
                    Session["tk"] = userSession;
                    Session.Add("tendangnhap", userSession.Taikhoan);
                    Session["id"] = userSession.id;
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public int InsertForFacebook(ThanhVien entity)
        {
            var user = data.ThanhViens.SingleOrDefault(x => x.HoTen == entity.HoTen);
            if (user == null)
            {
                data.ThanhViens.InsertOnSubmit(entity);
                data.SubmitChanges();
                return entity.id;
            }
            else
            {
                return user.id;
            }
        }

        [HttpPost]
        public ActionResult Signup(FormCollection collection, ThanhVien TV)
        {
            var TK = collection["TaiKhoan"];
            var MK = collection["Matkhau"];           
            var hoten = collection["HoTen"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["Ngaysinh"]);
            var sdt = collection["DienthoaiKH"];
            var DiaChi = collection["DiachiKH"];
            var email = collection["Email"];

            //Save về Database

            TV.Taikhoan = TK;
            TV.Matkhau = MK;
            TV.HoTen = hoten;
            TV.Ngaysinh = DateTime.Parse(ngaysinh);
            TV.DienthoaiKH = sdt;
            TV.DiachiKH = DiaChi;
            TV.Email = email;
            data.ThanhViens.InsertOnSubmit(TV);
            data.SubmitChanges();
            return RedirectToAction("Index", "NguoiDung");

        }
        [HttpPost]
        public ActionResult LogIn(FormCollection collection)
        {
            var TK = collection["Username"];
            var MK = collection["Password"];

            var TV = data.ThanhViens.SingleOrDefault(k => k.Taikhoan == TK && k.Matkhau == MK);
            if (TV != null)
            {
                Session["tk"] = TV;
                Session["tendangnhap"] = TV.HoTen;
                Session["id"] = TV.id;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Thongbao = "Tài khoản không hợp lệ";
            }

            return View("Index");
        }
        //=============================================================
        public ActionResult SignOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }
        public ActionResult Thongtin(int id)
        {
            var info = from IC in data.ThanhViens where IC.id == id select IC;
            return View(info.Single());
        }
        [HttpPost, ActionName("Thongtin")]
        public ActionResult Sua(int id)
        {
            ThanhVien Cus = data.ThanhViens.SingleOrDefault(n => n.id == id);
            UpdateModel(Cus);
            data.SubmitChanges();
            return RedirectToAction("Thongtin", "NguoiDung");
        }
        //===================
        

    }
}
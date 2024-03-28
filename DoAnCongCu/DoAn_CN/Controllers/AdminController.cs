using DoAn_CN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace DoAn_CN.Controllers
{
    public class AdminController : Controller
    {
        dbDoAnCNDataContext data = new dbDoAnCNDataContext();
        public ActionResult Index()
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                return View();
            }
            
        }
        //=====================================Đăng nhập===============================================
        public ActionResult LogInAdmin()
        {
            return View();
        }
        public ActionResult SignOut()
        {
            Session.Clear();
            return RedirectToAction("Index", "Admin");
        }
        [HttpPost]
        public ActionResult LogInAdmin(FormCollection collection)
        {
            var TK = collection["UserName"];
            var MK = collection["Password"];

            var tk = data.Accounts.SingleOrDefault(k => k.UserName != TK);
            var mk = data.Accounts.SingleOrDefault(k => k.PassWord != MK);

            var Acc = data.Accounts.SingleOrDefault(k => k.UserName == TK && k.PassWord == MK);
            if (Acc != null)
            {
                Session["account"] = Acc;
                Session["tendangnhap"] = Acc.HoTen;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                if(tk != null)
                {
                    ViewBag.Taikhoan = "Tài khoản không hợp lệ";
                }
                else if (mk != null)
                {
                    ViewBag.MatKhau = "Mật khẩu không hợp lệ";
                }
            }
            return this.LogInAdmin();
        }
        //==========================================End đăng nhập==========================================

        //==========================================Thương hiệu============================================
        public ActionResult ViewThuonghieu()
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                return View(data.ThuongHieus.OrderByDescending(n => n.idTH).ToList());
            }

        }
        [HttpPost]
        public ActionResult ThemTH(FormCollection collection, ThuongHieu TH)
        {
            var TenTH = collection["TenThuongHieu"];
            //Lưu Save
            TH.TenTH = TenTH;
            data.ThuongHieus.InsertOnSubmit(TH);
            data.SubmitChanges();
            ViewBag.ThongBao = "Thêm thương hiệu mới thành công";
            return RedirectToAction("ViewThuonghieu", "Admin");
        }
        [HttpGet]
        public ActionResult SuaTH(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var thuonghieu = from TH in data.ThuongHieus where TH.idTH == id select TH;
                return View(thuonghieu.Single());
            }
        }
        [HttpPost, ActionName("SuaTH")]
        public ActionResult Sua(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                ThuongHieu thuonghieu = data.ThuongHieus.SingleOrDefault(n => n.idTH == id);
                UpdateModel(thuonghieu);
                data.SubmitChanges();
                return RedirectToAction("ViewThuonghieu", "Admin");
            }
        }
        [HttpGet]
        public ActionResult XoaTH(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var Thuonghieu = from th in data.ThuongHieus where th.idTH == id select th;
                return View(Thuonghieu.Single());
            }
        }
        [HttpPost, ActionName("XoaTH")]
        public ActionResult Xoa(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                ThuongHieu thuoghieu = data.ThuongHieus.SingleOrDefault(n => n.idTH == id);
                data.ThuongHieus.DeleteOnSubmit(thuoghieu);
                data.SubmitChanges();
                return RedirectToAction("ViewThuonghieu", "Admin");
            }
        }
        //==========================================End thương hiệu========================================

        //==========================================Loại Sản phẩm==========================================
        public ActionResult ViewLoaiSP()
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                return View(data.Loai_SPs.OrderByDescending(n => n.idLoai));
            }
        }
        //==========================================End loại Sản phẩm======================================

        //==========================================Sản phẩm===============================================
        public ActionResult ViewSP(int? page, string keyword)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                
                int pagesize = 12;
               
                int pagenum = (page ?? 1);
                if (!string.IsNullOrEmpty(keyword))
                {
                    TempData["kwd"] = keyword;
                    List<SanPham> lstSanPham = data.SanPhams.Where(n => n.TenSP.ToLower().Contains(keyword.ToLower())).ToList();
                    return View(lstSanPham.OrderByDescending(n => n.idSP).ToPagedList(pagenum, pagesize));
                }
                return View(data.SanPhams.OrderByDescending(n => n.idSP).ToPagedList(pagenum, pagesize));
            }

        }
        [HttpGet]
        public ActionResult ThemSP()
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                ViewBag.idDM = new SelectList(data.DanhMucs.ToList().OrderBy(n => n.TenDM), "idDM", "TenDM");
                ViewBag.idTH = new SelectList(data.ThuongHieus.ToList().OrderBy(n => n.TenTH), "idTH", "TenTH");
                ViewBag.idLoai = new SelectList(data.Loai_SPs.ToList().OrderBy(n => n.TenLoai), "idLoai", "TenLoai");
                return View();
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemSP(SanPham sp, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload1,
            HttpPostedFileBase fileUpload2, HttpPostedFileBase fileUpload3)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var filename = Path.GetFileName(fileUpload.FileName);
                var filename1 = Path.GetFileName(fileUpload1.FileName);
                var filename2 = Path.GetFileName(fileUpload2.FileName);
                var filename3 = Path.GetFileName(fileUpload3.FileName);

                var path = Path.Combine(Server.MapPath("~/Content/Sale/images/"), filename);
                var path1 = Path.Combine(Server.MapPath("~/Content/Sale/images/"), filename1);
                var path2 = Path.Combine(Server.MapPath("~/Content/Sale/images/"), filename2);
                var path3 = Path.Combine(Server.MapPath("~/Content/Sale/images/"), filename3);

                if (System.IO.File.Exists(path))
                {
                    ViewBag.Thongbao = "Hình này đã tồn tại";
                }
                else if (System.IO.File.Exists(path1))
                {
                    ViewBag.Thongbao = "Hình này đã tồn tại";

                }
                else if (System.IO.File.Exists(path2))
                {
                    ViewBag.Thongbao = "Hình này đã tồn tại";

                }
                else if (System.IO.File.Exists(path3))
                {
                    ViewBag.Thongbao = "Hình này đã tồn tại";

                }
                else
                    //úp hình lên server
                fileUpload.SaveAs(path);
                fileUpload1.SaveAs(path1);
                fileUpload2.SaveAs(path2);
                fileUpload3.SaveAs(path3);

                sp.image_1 = filename;
                sp.image_2 = filename1;
                sp.image_3 = filename2;
                sp.image_4 = filename3;
                data.SanPhams.InsertOnSubmit(sp);
                data.SubmitChanges();
                return RedirectToAction("ViewSP", "Admin");
            }
        }
        [HttpGet]
        public ActionResult SuaSP(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                SanPham sanpham = data.SanPhams.SingleOrDefault(n => n.idSP == id);
                ViewBag.idDM = new SelectList(data.DanhMucs.ToList().OrderBy(n => n.TenDM), "idDM", "TenDM", sanpham.idDM);
                ViewBag.idTH = new SelectList(data.ThuongHieus.ToList().OrderBy(n => n.TenTH), "idTH", "TenTH", sanpham.idTH);
                ViewBag.idLoai = new SelectList(data.Loai_SPs.ToList().OrderBy(n => n.TenLoai), "idLoai", "TenLoai", sanpham.idLoai);
                return View(sanpham);
            }
        }
        [HttpPost, ActionName("SuaSP")]
        [ValidateInput(false)]
        public ActionResult Sua(int id, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload1,
            HttpPostedFileBase fileUpload2, HttpPostedFileBase fileUpload3)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                if (ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileUpload.FileName);
                    var filename1 = Path.GetFileName(fileUpload1.FileName);
                    var filename2 = Path.GetFileName(fileUpload2.FileName);
                    var filename3 = Path.GetFileName(fileUpload3.FileName);

                    var path = Path.Combine(Server.MapPath("~/Content/Sale/images/"), filename);
                    var path1 = Path.Combine(Server.MapPath("~/Content/Sale/images/"), filename1);
                    var path2 = Path.Combine(Server.MapPath("~/Content/Sale/images/"), filename2);
                    var path3 = Path.Combine(Server.MapPath("~/Content/Sale/images/"), filename3);

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình này đã tồn tại";
                    }
                    else if (System.IO.File.Exists(path1))
                    {
                        ViewBag.Thongbao = "Hình này đã tồn tại";

                    }
                    else if (System.IO.File.Exists(path2))
                    {
                        ViewBag.Thongbao = "Hình này đã tồn tại";

                    }
                    else if (System.IO.File.Exists(path3))
                    {
                        ViewBag.Thongbao = "Hình này đã tồn tại";

                    }
                    else
                        //úp hình lên server
                        fileUpload.SaveAs(path);
                    fileUpload1.SaveAs(path1);
                    fileUpload2.SaveAs(path2);
                    fileUpload3.SaveAs(path3);

                    SanPham sp = data.SanPhams.SingleOrDefault(n => n.idSP == id);
                    sp.image_1 = filename;
                    sp.image_2 = filename1;
                    sp.image_3 = filename2;
                    sp.image_4 = filename3;

                    UpdateModel(sp);
                    data.SubmitChanges();
                }
                return RedirectToAction("ViewSP", "Admin");
            }
        }
        [HttpGet]
        public ActionResult XoaSP(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var sanpham = from sp in data.SanPhams where sp.idSP == id select sp;
                return View(sanpham.Single());
            }
        }
        [HttpPost, ActionName("XoaSP")]
        public ActionResult Xoasp(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                SanPham sanpham = data.SanPhams.SingleOrDefault(n => n.idSP == id);
                data.SanPhams.DeleteOnSubmit(sanpham);
                data.SubmitChanges();
                return RedirectToAction("ViewSP", "Admin");
            }
        }
        [HttpPost]
        public ActionResult TimKiem(string keyword)
        {
            if (Session["account"] == null)
            {
                return RedirectToAction("LogInAdmin", "Admin");
            }
            else
            {
                int pagesize = 8;
                int pagenum = 1;

                TempData["kwd"] = keyword;
                List<SanPham> lstSanPham = data.SanPhams.Where(n => n.TenSP.ToLower().Contains(keyword.ToLower())).ToList();
                return View("ViewSP", lstSanPham.OrderByDescending(n => n.idSP).ToPagedList(pagenum, pagesize));
            }
        }
        /*==========================================End Sản phẩm===========================================

        ==============================================Đơn hàng===========================================*/
        public ActionResult ViewDDH(int? page, string keyword)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                int pagesize = 10;
                int pagenum = (page ?? 1);
                if (!string.IsNullOrEmpty(keyword))
                {
                    TempData["kwd"] = keyword;
                    List<DonDatHang> lstCus = data.DonDatHangs.Where(n => n.ThanhVien.HoTen.ToLower().Contains(keyword.ToLower())).ToList();
                    return View(lstCus.OrderByDescending(n => n.idDDH).ToPagedList(pagenum, pagesize));
                }
                return View(data.DonDatHangs.OrderByDescending(n => n.idDDH).ToPagedList(pagenum, pagesize));
            }
        }
        public ActionResult Detail(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var chitiet = from sp in data.CT_DDHs where sp.idDDH == id select sp;
                
                return View(chitiet);
            }
        }
        [HttpGet]
        public ActionResult SuaDDH(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                DonDatHang ddh = data.DonDatHangs.SingleOrDefault(n => n.idDDH == id);
                
                ViewBag.idTrangthai = new SelectList(data.TrangThais.ToList().OrderBy(n => n.TrangThai1), "id", "TrangThai1", ddh.idTrangThai);
                return View(ddh);
            }
        }
        [HttpPost, ActionName("SuaDDH")]
        public ActionResult SuaDH(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                DonDatHang ddh = data.DonDatHangs.SingleOrDefault(n => n.idDDH == id);

                UpdateModel(ddh);
                data.SubmitChanges();
            }
            return RedirectToAction("ViewDDH", "Admin");
        }
        [HttpPost]
        public ActionResult TimKiemDDH(string keyword)
        {
            if (Session["account"] == null)
            {
                return RedirectToAction("LogInAdmin", "Admin");
            }
            else
            {
                int pagesize = 8;
                int pagenum = 1;

                TempData["kwd"] = keyword;
                List<DonDatHang> lstCus = data.DonDatHangs.Where(n => n.idDDH.ToString().Contains(keyword.ToLower())).ToList();
                return View("ViewDDH", lstCus.OrderByDescending(n => n.idDDH).ToPagedList(pagenum, pagesize));
            }
        }
        /*==========================================End đơn hàng===========================================
        ===================================================================================================
        ===========================================Bài viết admin========================================*/
        public ActionResult News(int? page, string keyword)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                int pagesize = 10;
                int pagenum = (page ?? 1);
                if (!string.IsNullOrEmpty(keyword))
                {
                    TempData["kwd"] = keyword;
                    List<BaiViet_Admin> lstCus = data.BaiViet_Admins.Where(n => n.TieuDe.ToLower().Contains(keyword.ToLower())).ToList();
                    return View(lstCus.OrderByDescending(n => n.id).ToPagedList(pagenum, pagesize));
                }
                return View(data.BaiViet_Admins.OrderByDescending(n => n.id).ToPagedList(pagenum, pagesize));
            }
        }
        [HttpGet]
        public ActionResult ThemNews()
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                ViewBag.IdDM = new SelectList(data.DanhMucs.ToList().OrderBy(n => n.TenDM), "idDM", "TenDM");
                return View();
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemNews(BaiViet_Admin BA, HttpPostedFileBase fileUpload)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var filename = Path.GetFileName(fileUpload.FileName);
                
                var path = Path.Combine(Server.MapPath("~/Content/Home/images/"), filename);

                if (System.IO.File.Exists(path))
                {
                    ViewBag.Thongbao = "Hình này đã tồn tại";
                }
                    //úp hình lên server
                    fileUpload.SaveAs(path);
                BA.Avatar = filename;
                BA.NgayThem = DateTime.Now;
                data.BaiViet_Admins.InsertOnSubmit(BA);
                data.SubmitChanges();
                return RedirectToAction("News", "Admin");}
            
        }
        [HttpGet]
        public ActionResult SuaBA(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                BaiViet_Admin BVA = data.BaiViet_Admins.SingleOrDefault(n => n.id == id);
                ViewBag.IdDM = new SelectList(data.DanhMucs.ToList().OrderBy(n => n.TenDM), "idDM", "TenDM", BVA.IdDM);
                return View(BVA);
            }
        }
        [HttpPost, ActionName("SuaBA")]
        [ValidateInput(false)]
        public ActionResult SuaAdmin(int id, HttpPostedFileBase fileUpload)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                if (ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Home/images/"), filename);
                    
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình này đã tồn tại";
                    }
                    else 
                        //úp hình lên server
                        fileUpload.SaveAs(path);
                        BaiViet_Admin BVA = data.BaiViet_Admins.SingleOrDefault(n => n.id == id);
                        BVA.Avatar = filename;
                        UpdateModel(BVA);
                        data.SubmitChanges();
                    
                }
                return RedirectToAction("News", "Admin");
            }
        }
        [HttpGet]
        public ActionResult XoaBA(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                var BA = from ba in data.BaiViet_Admins where ba.id == id select ba;
                return View(BA.Single());
            }
        }
        [HttpPost, ActionName("XoaBA")]
        public ActionResult Xoaba(int id)
        {
            if (Session["account"] == null)
                return RedirectToAction("LogInAdmin", "Admin");
            else
            {
                BaiViet_Admin BA = data.BaiViet_Admins.SingleOrDefault(n => n.id == id);
                data.BaiViet_Admins.DeleteOnSubmit(BA);
                data.SubmitChanges();
                return RedirectToAction("News", "Admin");
            }
        }
        [HttpPost]
        public ActionResult TimKiemNew(string keyword)
        {
            if (Session["account"] == null)
            {
                return RedirectToAction("LogInAdmin", "Admin");
            }
            else
            {
                int pagesize = 8;
                int pagenum = 1;

                TempData["kwd"] = keyword;
                List<BaiViet_Admin> lstCus = data.BaiViet_Admins.Where(n => n.TieuDe.ToLower().Contains(keyword.ToLower())).ToList();
                return View("News", lstCus.OrderByDescending(n => n.id).ToPagedList(pagenum, pagesize));
            }
        }
        /*================================================End News================================================*/
    }
}
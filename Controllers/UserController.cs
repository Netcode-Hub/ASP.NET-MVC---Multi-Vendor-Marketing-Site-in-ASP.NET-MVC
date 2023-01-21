using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using _2017VisualStudioMVC.Models;
using System.IO;

namespace _2017VisualStudioMVC.Controllers
{
    public class UserController : Controller
    {
        dbMarketingContext db = new dbMarketingContext();
        // GET: User
        public ActionResult Index(int? page)
        {
            //int pagesize = 8, pageindex = 1;
            //pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            //var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            //IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);
            //return View(stu);
            

            int pagesize = 8, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.ToList().OrderBy(x => x.pro_name);
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);
            tbl_category cate = db.tbl_category();
            return View(stu);
        }

        public ActionResult ViewCategory(int? page)
        {
            int pagesize = 8, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(tbl_user uvm, HttpPostedFileBase imgfile)
        {
            string path = Uploadimg(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded...";
            }
            else
            {
                tbl_user u = new tbl_user();
                u.u_name = uvm.u_name;
                u.u_email = uvm.u_email;
                u.u_password = uvm.u_password;
                u.u_image = path;
                u.u_contact = uvm.u_contact;
                db.tbl_user.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View();
           
        }


        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(tbl_user avm)
        {
            tbl_user ad = db.tbl_user.Where(x => x.u_email == avm.u_email && x.u_password == avm.u_password).SingleOrDefault();

            if (ad != null)
            {
                Session["u_id"] = ad.u_id.ToString();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("error", "Invalid Username or Password");

            }
            return View();
        }
   

        public ActionResult CreateAd()
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categoryList = new SelectList(li, "cat_id", "cat_name");
            return View();
        }


        [HttpPost]
        public ActionResult CreateAd(tbl_product pvm, HttpPostedFileBase imgfile)
        {
            string path = Uploadimg(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded...";
            }
            else
            {
                tbl_product p = new tbl_product();
                p.pro_name = pvm.pro_name;
                p.pro_fk_cat = pvm.pro_fk_cat;
                p.pro_fk_user = Convert.ToInt32(Session["u_id"].ToString());
                p.pro_price = pvm.pro_price;
                p.pro_des = pvm.pro_des;
                p.pro_image = path;
                db.tbl_product.Add(p);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }


        public ActionResult Ads(int?id, int?page)
        {
            int pagesize = 6, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_fk_cat == id).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }

        [HttpPost]
        public ActionResult Ads(int? id, int? page, string search)
        {
            int pagesize = 8, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_name.Contains(search)).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }

        public ActionResult ViewAd(int?id)
        {
            ViewAdModel vam = new ViewAdModel();
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            vam.pro_id = p.pro_id;
            vam.pro_name = p.pro_name;
            vam.pro_price = p.pro_price;
            vam.pro_image = p.pro_image;
            vam.pro_des = p.pro_des;

            tbl_category cat = db.tbl_category.Where(x => x.cat_id == p.pro_fk_cat).SingleOrDefault();
            vam.cat_name = cat.cat_name;

            tbl_user u = db.tbl_user.Where(x => x.u_id == p.pro_fk_user).SingleOrDefault();
            vam.u_name = u.u_name;
            vam.u_image = u.u_image;
            vam.u_contact = u.u_contact;
            vam.u_email = u.u_email;
            vam.pro_fk_user = u.u_id;

            return View(vam);
        }

        public string Uploadimg(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".png") || extension.ToLower().Equals(".jpeg"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Uploads"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Uploads/" + random + Path.GetFileName(file.FileName);
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }

                }
                else
                {
                    Response.Write("<script> alert('Only jgp, jpeg, and png formats are accepted...'); </script>");
                }

            }
            return path;
        }

        public ActionResult SignOut()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        public ActionResult DeleteAd(int?id)
        {
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            db.tbl_product.Remove(p);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
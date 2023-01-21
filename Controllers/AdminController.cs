using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2017VisualStudioMVC.Models;
using System.IO;
using PagedList;

namespace _2017VisualStudioMVC.Controllers
{
    public class AdminController : Controller
    {
        dbMarketingContext db = new dbMarketingContext();

        // GET: Admin
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(tbl_admin avm)
        {
            tbl_admin ad = db.tbl_admin.Where(x => x.ad_username == avm.ad_username && x.ad_password == avm.ad_password).SingleOrDefault();

            if(ad != null)
            {
                Session["ad_id"] = ad.ad_id.ToString();
                return RedirectToAction("ViewCategory");
            }
            else
            {
                ModelState.AddModelError("error", "Invalid Username or Password");
                
            }
            return View();
        }



        // GET: Create Category
        public ActionResult Create()
        {
            if(Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(tbl_category cvm, HttpPostedFileBase imgfile)
        {
            string path = Uploadimg(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded...";
            }
            else
            {
                tbl_category cat = new tbl_category();
                cat.cat_name = cvm.cat_name;
                cat.cat_image = path;
                cat.cat_status = 1;
                cat.cat_fk_ad = Convert.ToInt32(Session["ad_id"].ToString());
                db.tbl_category.Add(cat);
                db.SaveChanges();
                return RedirectToAction("ViewCategory");
            }
            return View();
        }

        public ActionResult ViewCategory(int?page)
        {
            int pagesize = 8, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }

        //upload image
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

        public ActionResult DeleteCategory(int? id)
        {
            tbl_category c = db.tbl_category.Where(x => x.cat_id == id).SingleOrDefault();
            db.tbl_category.Remove(c);
            db.SaveChanges();
            return RedirectToAction("ViewCategory");
        }
    }
}
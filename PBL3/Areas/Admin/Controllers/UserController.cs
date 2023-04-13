using PagedList;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace PBL3.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        // GET: Admin/User
        private pbl3Entities db = new pbl3Entities();
        //  [AdminAuthorize(Role = new string[] { "Manager" })]
        public ActionResult Index(int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            ViewBag.Page = (page - 1) * pageSize + 1;
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var ds = db.Users.OrderByDescending(m => m.UserID).ToPagedList(pageIndex, pageSize);
            return View(ds);
        }
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(User model)
        {
            var account = db.Users.SingleOrDefault(m => m.UserName == model.UserName || m.Email == model.Email);
            if (account != null)
            {
                TempData["error"] = "Tài khoản hoặc Email đã tồn tại!";
                return View();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    model.CreatedDate = DateTime.Now;
                    var passwrHash = Crypto.HashPassword(model.Password);
                    model.Password = passwrHash;
                    //add vao csdl
                    db.Users.Add(model);
                    // luu lai thay doi
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View();

            }
            catch (Exception ex)
            {
                TempData["error"] = "Lưu dữ liệu thất bại. Vui lòng nhập đầy đủ dữ liệu!";
                return View(model);
            }

        }

        public ActionResult Edit(int id)
        {
            var model = db.Users.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(User model)
        {
         
            var update = db.Users.Find(model.UserID);
            update.UserName = model.UserName;
            if (update.Password == model.Password)
            {
                update.Password = model.Password;
            }
            else
            {
                var passwrHash = Crypto.HashPassword(model.Password);
                update.Password = passwrHash;
            }
            update.Email = model.Email;
            update.Role = model.Role;
            update.Status = model.Status;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var del = db.Users.Find(id);
            db.Users.Remove(del);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
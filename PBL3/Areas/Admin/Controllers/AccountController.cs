using PBL3.EF;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace PBL3.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        // GET: Admin/Account
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Register model)
        {
            var account = db.Users.SingleOrDefault(m => m.UserName == model.UserName || m.Email == model.Email);
            if (account != null)
            {
                TempData["error"] = "Tài khoản hoặc Email đã tồn tại!";
                return View();
            }

            if (ModelState.IsValid)
            {
                var user = new User();
                user.UserName = model.UserName;
                var passwrHash = Crypto.HashPassword(model.Password);
                user.Password = passwrHash;
                user.Email = model.Email;
                user.CreatedDate = DateTime.Now;
                user.Role = 2;
                user.Status = true;
                //add vao csdl
                db.Users.Add(user);
                // luu lai thay doi
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("New Error", "Dữ liệu không hợp lệ!");
                return View();
            }
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginV model)
        {
            if (ModelState.IsValid)
            {
                var account = db.Users.FirstOrDefault(m => m.UserName == model.UserName);
                if (account != null && account.Status == false)
                {
                    TempData["error"] = "Tài khoản đã bị khóa!";
                    return View();
                }
                if (account != null && Crypto.VerifyHashedPassword(account.Password, model.Password))
                {
                    // tao session
                    Session["user"] = account;
                    return Redirect("~/Admin");
                    //return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = "Tài khoản hoặc mật khẩu không đúng!";
                    // ModelState.AddModelError("New Error", "Tài khoản hoặc mật khẩu không đúng!");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("New Error", "Dữ liệu không hợp lệ!");
                return View();
            }

        }

        public ActionResult Logout()
        {
            //xoa session
            Session.Remove("user");
            // xoa cookies form authent
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}
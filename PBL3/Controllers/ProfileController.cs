using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace PBL3.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index()
        {
            User nvSession = (User)Session["user"];
            User user = new User();
            if (nvSession != null)
            {
                user = db.Users.Find(nvSession.UserID);
                if (user != null)
                {
                    return View(user);
                }
            }
            return View(user);
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
            if (update.Password == model.Password)
            {
                update.Password = model.Password;
            }
            else
            {
                var passwrHash = Crypto.HashPassword(model.Password);
                update.Password = passwrHash;
            }
            update.UserName = model.UserName;
            update.Email = model.Email;
            update.Address = model.Address;
            update.Phone = model.Phone;
            update.Avatar = model.Avatar;
            db.SaveChanges();
            TempData["type"] = "success";
            TempData["successMessage"] = "Cập nhật thành công!";
            return RedirectToAction("Index");
        }
    }
}
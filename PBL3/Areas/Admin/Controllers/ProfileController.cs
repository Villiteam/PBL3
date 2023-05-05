using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Areas.Admin.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Admin/Profile
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
    }
}
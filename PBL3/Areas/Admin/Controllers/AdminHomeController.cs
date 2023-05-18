using PBL3.App_Start;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Areas.Admin.Controllers
{
    [AdminAuthorize(Role = new string[] { "Admin", "Manager" })]
    public class AdminHomeController : Controller
    {
        // GET: Admin/AdminHome
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ChuyenVeUserHome()
        {
            return Redirect("~/Home");
        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult test()
        {
            return View();
        }
    }
}
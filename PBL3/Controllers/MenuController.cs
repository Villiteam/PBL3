using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Controllers
{

    public class MenuController : Controller
    {

        // GET: Menu
        private pbl3Entities db = new pbl3Entities();
        public ActionResult MenuArrivals()
        {
            var item = db.Categories.Where(m => m.Status == true).ToList();
            return PartialView("_MenuArrivals", item);
        }
        public ActionResult MenuLeft(int? id)
        {
            if(id!= null)
            {
                ViewBag.CatID = id;
            }
            var item = db.Categories.Where(m => m.Status == true).ToList();
            return PartialView("_MenuLeft", item);
        }
    }
}
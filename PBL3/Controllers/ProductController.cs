using Antlr.Runtime.Tree;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index()
        {
            var ds = db.Products.ToList();
            return View(ds);
        }
        public ActionResult Detail(int? id)
        {
            var item = db.Products.Find(id);
            return View(item);
        }

        public ActionResult ProductCategory(int id)
        {
            var ds = db.Products.Where(m=>m.CatID== id).ToList();   
            ViewBag.CatID = id;

            var cat = db.Categories.Find(id).CatName;
            ViewBag.CatName = cat;
            return View(ds);
        }

        public ActionResult Partial_ItemByCatID()
        {
            var item = db.Products.Where(m => m.isHome && m.Status).Take(10).ToList();
            return PartialView("Partial_ItemByCatID",item);
        }
        public ActionResult Partial_BestSellers()
        {
            var item = db.Products.Where(m => m.isSale && m.Status).Take(10).ToList();
            return PartialView("Partial_BestSellers", item);
        }
    }
}
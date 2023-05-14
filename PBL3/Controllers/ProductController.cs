 using Antlr.Runtime.Tree;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PBL3.EF;

namespace PBL3.Controllers
{
    public class ProductController : Controller
    {
        
        // GET: Product
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index(int? id)
        {
            if(id != null)
            {
                var ds = db.Products.Where(m => m.CatID == id && m.Status == true).ToList();
                ViewBag.CatID = id;

                var cat = db.Categories.Find(id).CatName;
                ViewBag.CatName = cat;
                return View(ds);
            }
            else
            {
                ViewBag.CatID = null;
                ViewBag.CatName = null;
                var ds = db.Products.Where(m => m.Status == true).ToList();
                return View(ds);
            }
        }

        public ActionResult Detail(int? id)
        {

            var comments = (from od in db.OrderDetails
                            join c in db.Comments on od.OderDetailID equals c.OrderDetailID
                            where od.ProductID == id
                            select c).ToList();
            ViewBag.Comment = comments;

            var count = comments.Count();
            double rate = (double)comments.Select(m=> m.Rating).Sum()/count;

            ViewBag.Rating = rate;
            ViewBag.Count = count;

            var item = db.Products.Find(id);
            return View(item);
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

        public ActionResult Search(string keyword)
        {
            var data = db.Products.Where(m => m.ProductName.Contains(keyword)).ToList(); 
            return View(data);
        }
 
    }
}
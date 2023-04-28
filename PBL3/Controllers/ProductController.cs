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
        public ActionResult Index()
        {
            var ds = db.Products.ToList();
            return View(ds);
        }
        public ActionResult test()
        {
            return View();
        }
        public ActionResult Detail(int? id)
        {
            var detail = db.OrderDetails.Where(m => m.ProductID == id);
            var join = (from p in detail
                        join q in db.Comments on p.OderDetailID equals q.OrderDetailID
                        select new CommentViewModel
                        {
                            CommentID = q.CommentID,
                            OrderDetailID = q.OrderDetailID,
                            CreateDate = q.CreateDate,
                            UserID = q.UserID,
                            Rating = q.Rating,
                            Comment = q.Comment1
                        }).ToList();

            var count = join.Count();
            double rate = (double)join.Select(m=> m.Rating).Sum()/count;

            ViewBag.Join = join;
            ViewBag.Rating = rate;
            ViewBag.Count = count;
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

        public ActionResult Search(string keyword)
        {
            var data = db.Products.Where(m => m.ProductName.Contains(keyword)).ToList(); 
            return View(data);
        }
 
    }
}
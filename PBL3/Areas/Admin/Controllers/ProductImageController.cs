using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Areas.Admin.Controllers
{
    public class ProductImageController : Controller
    {
        // GET: Admin/ProductImage
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index(int? id)
        {
            ViewBag.ProductID = id;
            var ds = db.ProductImages.Where(m => m.ProductID == id).ToList();
            return View(ds);
        }
        [HttpPost]
        public ActionResult AddImage(int productID, string url)
        {
            db.ProductImages.Add(new ProductImage
            {
                ProductID = productID,
                isDefault = false,
                Image = url
            });
            db.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var del = db.ProductImages.Find(id);
            db.ProductImages.Remove(del);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}
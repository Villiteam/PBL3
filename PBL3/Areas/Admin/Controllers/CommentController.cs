using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Areas.Admin.Controllers
{
    public class CommentController : Controller
    {
        // GET: Admin/Comment
        private pbl3Entities db = new pbl3Entities();
        public ActionResult CommentByProductID(int id)
        {
            var comments = (from od in db.OrderDetails
                            join c in db.Comments on od.OderDetailID equals c.OrderDetailID
                            where od.ProductID == id
                            select c).ToList();
            ViewBag.Comment = comments;

            var count = comments.Count();
            double rate = (double)comments.Select(m => m.Rating).Sum() / count;

            ViewBag.Rating = rate;
            ViewBag.Count = count;

            var item = db.Products.Find(id);
            return View(item);
        }
        public ActionResult Delete(int id)
        {
            var cmm = db.Comments.Find(id);
            db.Comments.Remove(cmm);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}
using PagedList;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace PBL3.Controllers
{
    public class CommentController : Controller
    {
        // GET: Comment
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index(int? page)
        {
   
            User nvSession = (User)Session["user"];
            if (nvSession == null)
            {
                ModelState.AddModelError("New Error", "Vui lòng đăng nhập!");
                return View();
            }

            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            ViewBag.Page = (page - 1) * pageSize + 1;
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            IPagedList<Comment> list = null;
            list = db.Comments.Where(m => m.UserID == nvSession.UserID).OrderByDescending(m => m.CommentID).ToPagedList(pageIndex, pageSize);
            return View(list);
           
        }
        public ActionResult CommentByOrderDetailID(int id)
        {
            var item = db.Comments.Find(id);
            if (item != null)
            {
                //TempData["OrderDetailID"] = id;
                return RedirectToAction("Index");
            }
            ViewBag.OrderDetailID = id;
            return View();
        }
        [HttpPost]
        public ActionResult CommentByOrderDetailID(int detailid, int rating, string comment)
        {
            if (rating == 0)
            {
                ModelState.AddModelError("New Error", "Vui lòng chọn số sao!");
                return View();
            }
            if (string.IsNullOrEmpty(comment))
            {
                ModelState.AddModelError("New Error", "Bình luận không được để trống!");
                return View();
            }

            var orderDetail = db.OrderDetails.Find(detailid);
            db.SaveChanges();
            var order = db.Orders.Find(orderDetail.OrderID);

            var com = new Comment();
            com.UserID = order.UserID;
            com.OrderDetailID = detailid;
            com.Comment1 = comment;
            com.Rating = rating;
            com.CreateDate = DateTime.Now;

            db.Comments.Add(com);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            var item = db.Comments.Find(id);
            return View(item);
        }
        [HttpPost]
        public ActionResult Edit(int id,int rating, string comment)
        {
            if (rating == 0)
            {
                ModelState.AddModelError("New Error", "Vui lòng chọn số sao!");
                return View();
            }
            if (string.IsNullOrEmpty(comment))
            {
                ModelState.AddModelError("New Error", "Bình luận không được để trống!");
                return View();
            }
            var cm = db.Comments.Find(id);
            cm.Comment1 = comment;
            cm.Rating = rating;
            cm.CreateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
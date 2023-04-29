using PagedList;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        // GET: Admin/Order
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index(int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            ViewBag.Page = (page - 1) * pageSize + 1;
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var ds = db.Orders.OrderByDescending(m => m.OrderID).ToPagedList(pageIndex, pageSize);
            return View(ds);
        }
        public ActionResult Detail(int id)
        {
            var order = db.Orders.Find(id);
            return View(order);
        }
        [HttpPost]
        public ActionResult Status(int id)
        {
            var item = db.Orders.Find(id);
            if (item != null)
            {
                item.Status = !item.Status;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.Status });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult Delivered(int id)
        {
            var item = db.Orders.Find(id);
            if (item != null)
            {
                item.Delivered = !item.Delivered;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.Delivered });
            }
            return Json(new { success = false });
        }
        public ActionResult PrintOrder(int id)
        {
            var order = db.Orders.Find(id);
            return View(order);
        }
    }
}
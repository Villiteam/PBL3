using PagedList;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using System.Web.Services.Description;

namespace PBL3.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        // GET: Admin/Order
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index(int? page, string sort, string keyword, string phone)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            ViewBag.Page = (page - 1) * pageSize + 1;
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var list = db.Orders.Include(m => m.User).ToList();

            // Lọc tên theo keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(m => m.User.UserName.Contains(keyword)).ToList();
            }

            ViewBag.Keyword = keyword;

            // Lọc theo số điện thoại  
            if (!string.IsNullOrEmpty(phone))
            {
                list = list.Where(m => m.OrderPhone.Contains(phone)).ToList();
            }
            ViewBag.Phone = phone;


            //Sort
            ViewBag.SortByDate = String.IsNullOrEmpty(sort) ? "date" : "";
            ViewBag.SortByName = (sort == "name_desc") ? "name" : "name_desc";
            ViewBag.SortByTotal = (sort == "total_desc") ? "total" : "total_desc";
            ViewBag.SortByStatus = (sort == "status_desc") ? "status" : "status_desc";
            ViewBag.SortByDelivered = (sort == "delivered_desc") ? "delivered" : "delivered_desc";


            IPagedList<Order> pagedList = null;
            switch (sort)
            {
                case "date":
                    pagedList = list.OrderBy(m => m.OrderDate).ToPagedList(pageIndex, pageSize);
                    break;
                case "name":
                    pagedList = list.OrderBy(m => m.User.UserName).ToPagedList(pageIndex, pageSize);
                    break;
                case "name_desc":
                    pagedList = list.OrderByDescending(m => m.User.UserName).ToPagedList(pageIndex, pageSize);
                    break;
                case "total":
                    pagedList = list.OrderBy(m => m.Total).ToPagedList(pageIndex, pageSize);
                    break;
                case "total_desc":
                    pagedList = list.OrderByDescending(m => m.Total).ToPagedList(pageIndex, pageSize);
                    break;
                case "status":
                    pagedList = list.OrderBy(m => m.Status).ToPagedList(pageIndex, pageSize);
                    break;
                case "status_desc":
                    pagedList = list.OrderByDescending(m => m.Status).ToPagedList(pageIndex, pageSize);
                    break;
                case "delivered":
                    pagedList = list.OrderBy(m => m.Delivered).ToPagedList(pageIndex, pageSize);
                    break;
                case "delivered_desc":
                    pagedList = list.OrderByDescending(m => m.Delivered).ToPagedList(pageIndex, pageSize);
                    break;
                default:
                    pagedList = list.OrderByDescending(m => m.OrderDate).ToPagedList(pageIndex, pageSize);
                    break;
            }

            return View(pagedList);
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
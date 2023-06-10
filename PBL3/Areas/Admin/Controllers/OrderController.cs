using PagedList;
using PBL3.App_Start;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace PBL3.Areas.Admin.Controllers
{
    [AdminAuthorize(Role = new string[] { "Admin", "Accountant" })]
    public class OrderController : Controller
    {
        // GET: Admin/Order
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index(int? page, string sort, string keyword, string phone,string fromdate, string todate)
        {
            var pageSize = 10;
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

            //Lọc theo ngày 
            if (!string.IsNullOrEmpty(fromdate))
            {
                DateTime startDate = DateTime.ParseExact(fromdate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                list = list.Where(x => x.OrderDate >= startDate).ToList();
            }
            if (!string.IsNullOrEmpty(todate))
            {
                DateTime endDate = DateTime.ParseExact(todate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                list = list.Where(x => x.OrderDate <= endDate).ToList();
            }
            ViewBag.FromDate = fromdate;
            ViewBag.ToDate = todate;

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
        public ActionResult Delete(int id)
        {
            try
            {
                var order = db.Orders
                    .Include(o => o.OrderDetails.Select(d => d.Size))
                    .FirstOrDefault(o => o.OrderID == id);

                if (order != null && !order.Status && !order.Delivered)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        detail.Size.Quantity += detail.Quantity;
                    }

                    var productQuantities = order.OrderDetails
                        .GroupBy(d => d.ProductID)
                        .Select(g => new { ProductID = g.Key, Quantity = g.Sum(d => d.Quantity) });

                    foreach (var pq in productQuantities)
                    {
                        var product = db.Products.Find(pq.ProductID);
                        product.Quantity += pq.Quantity;
                    }

                    db.Orders.Remove(order);
                    db.SaveChanges();

                    TempData["error"] = "";
                }
                else
                {
                    TempData["error"] = "Không thể xóa đơn hàng đã giao!";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Đã xảy ra lỗi khi xóa đơn hàng: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

    }
}
using PBL3.App_Start;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Areas.Admin.Controllers
{
    [AdminAuthorize(Role = new string[] { "Admin", "Accountant" })]
    public class StatisticalController : Controller
    {
        // GET: Admin/Statistical
        private pbl3Entities db = new pbl3Entities();
       // [AdminAuthorize(Role = new string[] { "Manager" })]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails
                        on o.OrderID equals od.OrderID
                        join p in db.Products
                        on od.ProductID equals p.ProductID
                        select new
                        {
                            CreateDate = o.OrderDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice
                        };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                query = query.Where(x => x.CreateDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                query = query.Where(x => x.CreateDate <= endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreateDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price)
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy
            });


            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult BangThongKe()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ThongKe(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails
                        on o.OrderID equals od.OrderID
                        join p in db.Products
                        on od.ProductID equals p.ProductID
                        select new
                        {
                            CreateDate = o.OrderDate,
                            ProductID = od.ProductID,
                            ProductName = p.ProductName,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice
                        };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                query = query.Where(x => x.CreateDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                query = query.Where(x => x.CreateDate <= endDate);
            }

            var result = query.GroupBy(x => new { x.ProductID, x.ProductName, CreateDate = DbFunctions.TruncateTime(x.CreateDate) })
                   .Select(x => new
                   {
                       ProductID = x.Key.ProductID,
                       ProductName = x.Key.ProductName,
                       Date = x.Key.CreateDate.Value,
                       TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                       TotalSell = x.Sum(y => y.Quantity * y.Price)
                   }).Select(x => new
                   {
                       ProductID = x.ProductID,
                       ProductName = x.ProductName,
                       Date = x.Date,
                       DoanhThu = x.TotalSell,
                       LoiNhuan = x.TotalSell - x.TotalBuy
                   });

            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult BangThongKe2()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ThongKe2(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails
                        on o.OrderID equals od.OrderID
                        join p in db.Products
                        on od.ProductID equals p.ProductID
                        select new
                        {
                            CreateDate = o.OrderDate,
                            ProductID = od.ProductID,
                            ProductName = p.ProductName,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice
                        };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                query = query.Where(x => x.CreateDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                query = query.Where(x => x.CreateDate <= endDate);
            }

            var result = query.GroupBy(x => new { x.ProductID, x.ProductName, CreateDate = DbFunctions.TruncateTime(x.CreateDate) })
                   .Select(x => new
                   {
                       ProductID = x.Key.ProductID,
                       ProductName = x.Key.ProductName,
                       Date = x.Key.CreateDate.Value,
                       TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                       TotalSell = x.Sum(y => y.Quantity * y.Price)
                   }).Select(x => new
                   {
                       ProductID = x.ProductID,
                       ProductName = x.ProductName,
                       Date = x.Date,
                       DoanhThu = x.TotalSell,
                       LoiNhuan = x.TotalSell - x.TotalBuy
                   });
            var rs = result.GroupBy(x => new { x.ProductID, x.ProductName })
                   .Select(x => new
                   {
                       ProductID = x.Key.ProductID,
                       ProductName = x.Key.ProductName,
                       DoanhThu = x.Sum(y => y.DoanhThu),
                       LoiNhuan = x.Sum(y => y.LoiNhuan)
                   });

            return Json(new { Data = rs }, JsonRequestBehavior.AllowGet);

        }
    }
}

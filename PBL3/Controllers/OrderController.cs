using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using PBL3.EF;
using PagedList;

namespace PBL3.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        private pbl3Entities db = new pbl3Entities();
        private string CartSession = Common.CommonConstants.CartSession;
        public ActionResult Index(int? page)
        {
            User nvSession = (User)Session["user"];
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            ViewBag.Page = (page - 1) * pageSize + 1;
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            IPagedList<Order> list = null;

            if (nvSession != null)
            {
                list = db.Orders.Where(m => m.UserID == nvSession.UserID).OrderByDescending(m => m.OrderID).ToPagedList(pageIndex, pageSize);
            }
            return View(list);
        }

        public ActionResult Detail(int id)
        {
            var order = db.Orders.Find(id);
            return View(order);
        }
        public ActionResult Payment()
        {

            var cart = Session[Common.CommonConstants.CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            // kiểm tra đăng nhập mới cho thanh toán
            User nvSession = (User)Session["user"];
            if (nvSession == null)
            {
                ViewBag.ShowModal = true;
                ViewBag.Error = "Vui lòng đăng nhập để thanh toán";
            }
           
            return View(list);
        }
        [HttpPost]
        public ActionResult Payment(string address, string phone, string PayType,double total)
        {
            var sessionCart = Session[Common.CommonConstants.CartSession];
            var list = new List<CartItem>();
            if (sessionCart != null)
            {
                list = (List<CartItem>)sessionCart;
            }
            var model = new Order();
            // kiểm tra đăng nhập mới cho thanh toán
            User nvSession = (User)Session["user"];
            if (nvSession != null)
            {
                model.UserID = nvSession.UserID;
            }
            else
            {
                ViewBag.ShowModal = true;
                ViewBag.Error = "Vui lòng đăng nhập để thanh toán";
                return View(list);
            }
            // Điền đủ thông tin
            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(PayType))
            {
                ModelState.AddModelError("", "Địa chỉ giao hàng, số điện thoại không được để trống!");
                return View(list);
            }

            //Lưu đơn hàng
            model.OrderAddress = address;
            model.OrderPhone = phone;
            model.PayType = PayType;
            model.OrderDate = DateTime.Now;
            model.Status = false;
            model.Delivered = false;
            model.DeliveryDate = DateTime.Now;
            model.IsPay = false;
            model.Total = total;
            db.Orders.Add(model);
            db.SaveChanges();

            // Lưu chi tiết đơn hàng
            var cart = (List<CartItem>)Session[CartSession];
            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail();
                orderDetail.OrderID = model.OrderID;
                orderDetail.ProductID = item.Product.ProductID;
                orderDetail.Quantity = item.Quantity;
                orderDetail.SizeID = item.SizeID;
                if(item.Product.isSale == true)
                {
                    orderDetail.Price = item.Product.PromotionPrice.Value;

                }
                else
                {
                    orderDetail.Price = item.Product.Price;
                }
                db.OrderDetails.Add(orderDetail);
                db.SaveChanges();
            }
            Session[CartSession] = null;
            return RedirectToAction("Success");
        }
        public ActionResult Success()
        {
            return View();
        }
    }
}
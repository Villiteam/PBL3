using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using PBL3.EF;
using PagedList;
using Newtonsoft.Json;
using ZaloPay.Helper.Crypto;
using ZaloPay.Helper;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Ajax.Utilities;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PBL3.Controllers
{
    public class OrderController : Controller
    {
        static string appid = "553";
        static string key1 = "9phuAOYhan4urywHTh0ndEXiV3pKHr5Q";
        static string key2 = "Iyz2habzyr7AG8SgvoBCbKwKi3UzlLi3";
        static string createOrderUrl = "https://sandbox.zalopay.com.vn/v001/tpe/createorder";
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
                foreach (var item in list)
                {
                    var i = db.Sizes.Find(item.SizeID);
                    if (item.Quantity > i.Quantity)
                    {
                        return RedirectToAction("Index", "Cart");
                    }
                }
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
        public async Task<ActionResult> Payment(string address, string phone, string PayType,double total)
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

                // cập nhật số lượng bảng size;
                var sizeItem = db.Sizes.Find(item.SizeID);
                sizeItem.Quantity = sizeItem.Quantity - item.Quantity;
                db.SaveChanges();

                // cập nhật số lượng tổng của sản phẩm
                var product = db.Products.Find(item.Product.ProductID);
                product.Quantity = product.Quantity - item.Quantity;
                db.SaveChanges();
            }


            Session[CartSession] = null;
            if (model.PayType == "zalopay")
            {
                var transid = Guid.NewGuid().ToString();
                var embeddata = new { orderid = model.OrderID };
                List<object> listitems = new List<object>();

                foreach (var item in cart)
                {
                    listitems.Add(new { itemid = item.Product.ProductID, itemname = item.Product.ProductName, itemprice = item.Product.Price, itemquantity = item.Quantity });
                }
                    var param = new Dictionary<string, string>();

                param.Add("appid", appid);
                param.Add("key1", key1);
                param.Add("key2", key2);
                param.Add("appuser", "demo");
                param.Add("apptime", Utils.GetTimeStamp().ToString());
                param.Add("amount", total.ToString());
                param.Add("apptransid", DateTime.Now.ToString("yyMMdd") + "_" + transid); // mã giao dich có định dạng yyMMdd_xxxx
                param.Add("embeddata", JsonConvert.SerializeObject(embeddata));
                param.Add("item", JsonConvert.SerializeObject(listitems.ToArray()));
                param.Add("description", "ZaloPay demo");
                param.Add("bankcode", "");

                param.Add("redirecturl", "https://localhost:44339/Order/CheckZaloPay");

                var data = appid + "|" + param["apptransid"] + "|" + param["appuser"] + "|" + param["amount"] + "|"
                    + param["apptime"] + "|" + param["embeddata"] + "|" + param["item"];
                param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

                var result = await HttpHelper.PostFormAsync(createOrderUrl, param);
                return RedirectToAction("Success", new { id = model.OrderID, qrurl = result["orderurl"].ToString() });
            }
            return RedirectToAction("Success", new { id = model.OrderID });
        }
        public ActionResult Success(int id, string qrurl)
        {
            if (qrurl != null)
            {
                QRCodeGenerator QrGenerator = new QRCodeGenerator();
                QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(qrurl, QRCodeGenerator.ECCLevel.Q);
                QRCode QrCode = new QRCode(QrCodeInfo);
                Bitmap QrBitmap = QrCode.GetGraphic(60);
                byte[] BitmapArray = QrBitmap.BitmapToByteArray();
                string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
                ViewBag.QrCodeUri = QrUri;
            }
            if (db.Orders.Single(data => data.OrderID == id).PayType == "smartpay")
            {
                return View(new {});
            } else 
            return View();
        }
        public ActionResult Delete(int id)
        {
            var order = db.Orders.Find(id);
            if (order != null)
            {
                if(order.Status == false && order.Delivered == false)
                {
                    db.Orders.Remove(order);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
    }
    public static class BitmapExtension
    {
        public static byte[] BitmapToByteArray(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
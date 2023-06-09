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
using System.Windows.Input;

namespace PBL3.Controllers
{
    public class OrderController : Controller
    {
        static string app_id = "2553";
        static string key1 = "PcY4iZIKFCIdgZvA6ueMcMHHUbRLYjPL";
        static string create_order_url = "https://sb-openapi.zalopay.vn/v2/create";
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
                Random rnd = new Random();
                var app_trans_id = rnd.Next(1000000); // Generate a random order's ID.

                var embeddata = new { orderid = model.OrderID };
                List<object> listitems = new List<object>();

                foreach (var item in cart)
                {
                    listitems.Add(new { itemid = item.Product.ProductID, itemname = item.Product.ProductName, itemprice = item.Product.Price, itemquantity = item.Quantity });
                }
                    var param = new Dictionary<string, string>();

                param.Add("app_id", app_id);
                param.Add("app_user", "user123");
                param.Add("app_time", Utils.GetTimeStamp().ToString());
                param.Add("amount", total.ToString());
                param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
                param.Add("embed_data", JsonConvert.SerializeObject(embeddata));
                param.Add("item", JsonConvert.SerializeObject(listitems.ToArray()));
                param.Add("description", "ZaloPay demo");
                param.Add("bank_code", "zalopayapp");

                param.Add("redirecturl", "https://localhost:44339/Order/CheckZaloPay");

                var data = app_id + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
                param.Add("mac", HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

                var result = await HttpHelper.PostFormAsync(create_order_url, param);
                return Redirect(result["order_url"].ToString());
                //return RedirectToAction("Success", new { id = model.OrderID, qrurl = result["order_url"].ToString() });
            }
            return RedirectToAction("Success", new { id = model.OrderID });
        }

        [HttpPost]
        public ActionResult CheckZaloPay(dynamic cbdata)
        {
            var result = new Dictionary<string, object>();

            try
            {
                var dataStr = Convert.ToString(cbdata["data"]);
                var reqMac = Convert.ToString(cbdata["mac"]);

                var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, dataStr);

                // kiểm tra callback hợp lệ (đến từ ZaloPay server)
                if (!reqMac.Equals(mac))
                {
                    // callback không hợp lệ
                    result["return_code"] = -1;
                    result["return_message"] = "mac not equal";
                }
                else
                {
                    // thanh toán thành công
                    // merchant cập nhật trạng thái cho đơn hàng
                    var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                    Console.WriteLine("update order's status = success where app_trans_id = {0}", dataJson["app_trans_id"]);

                    result["return_code"] = 1;
                    result["return_message"] = "success";
                }
            }
            catch (Exception ex)
            {
                result["return_code"] = 0; // ZaloPay server sẽ callback lại (tối đa 3 lần)
                result["return_message"] = ex.Message;
            }

            // thông báo kết quả cho ZaloPay server
            return Json(result);
        }

        public ActionResult Success(int id)
        {
            //if (qrurl != null)
            //{
            //    QRCodeGenerator QrGenerator = new QRCodeGenerator();
            //    QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(qrurl, QRCodeGenerator.ECCLevel.Q);
            //    QRCode QrCode = new QRCode(QrCodeInfo);
            //    Bitmap QrBitmap = QrCode.GetGraphic(60);
            //    byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            //    string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
            //    ViewBag.QrCodeUri = QrUri;
            //}
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
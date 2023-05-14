using PBL3.EF;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebGrease;
using System.Web.Script.Serialization;
using System.ComponentModel;

namespace PBL3.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        private pbl3Entities db = new pbl3Entities();
        private string CartSession = Common.CommonConstants.CartSession;
        public ActionResult Index()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
                foreach (var item in list)
                {
                    var i = db.Sizes.Find(item.SizeID);
                    if (item.Quantity > i.Quantity)
                    {
                        ViewBag.ShowModal = true;
                        ViewBag.Error = "Số lượng sản phẩm: \"" + item.Product.ProductName + "\", kích cỡ \"" + i.SizeName + "\" chỉ còn " + i.Quantity + " sản phẩm, vui lòng nhập ít hơn!";
                        return View(list);
                    }

                }
            }
            return View(list);
        }
        public JsonResult Delete(int id, int sizeID)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            sessionCart.RemoveAll(x => x.Product.ProductID == id && x.SizeID == sizeID);
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });

        }
        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;
            return Json(new
            {
                status = true
            });

        }
        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            var sessionCart = (List<CartItem>)Session[CartSession];

            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.ProductID == item.Product.ProductID);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                }
            }
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        public ActionResult AddItem(int id, int quantity, int sizeID)
        {
            var product = db.Products.Find(id);
            var cart = Session[CartSession];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(m => m.Product.ProductID == id && m.SizeID == sizeID))
                {
                    foreach (var item in list)
                    {
                        if (item.Product.ProductID == id)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    //tạo mới đối tượng cart item 
                    var item = new CartItem();
                    item.Product = product;
                    item.Quantity = quantity;
                    item.SizeID = sizeID;
                    list.Add(item);
                }
                // Gán vào session
                Session[CartSession] = list;
            }
            else
            {
                //tạo mới đối tượng cart item 
                var item = new CartItem();
                item.Product = product;
                item.Quantity = quantity;
                item.SizeID = sizeID;
                var list = new List<CartItem>();
                list.Add(item);

                // Gán vào session
                Session[CartSession] = list;
            }
            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public PartialViewResult ViewCart()
        {
            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return PartialView(list);
        }

    }
}
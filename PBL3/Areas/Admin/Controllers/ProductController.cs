using PagedList;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        // GET: Admin/Product
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
            var ds = db.Products.OrderByDescending(m => m.ProductID).ToPagedList(pageIndex, pageSize);
            return View(ds);
        }
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Product model, List<string> Images, List<int> rDefault)
        {
            var item = db.Products.SingleOrDefault(m => m.ProductName == model.ProductName);
            if (item != null)
            {
                TempData["error"] = "Tên sản phẩm đã tồn tại!";
                return View();
            }
            if (Images != null && Images.Count > 0)
            {
                for (int i = 0; i < Images.Count; i++)
                {
                    if (i + 1 == rDefault[0])
                    {
                        model.ListImages = Images[i];
                        model.ProductImages.Add(new ProductImage
                        {
                            ProductID = model.ProductID,
                            Image = Images[i],
                            isDefault = true
                        });
                    }
                    else
                    {
                        model.ProductImages.Add(new ProductImage
                        {
                            ProductID = model.ProductID,
                            Image = Images[i],
                            isDefault = false
                        });
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Vui lòng thêm hình ảnh đại diện!");
                return View(model);
            }


            try
            {
                if (ModelState.IsValid)
                {

                    //add vao csdl
                    model.CreateDate = DateTime.Now;
                    db.Products.Add(model);
                    // luu lai thay doi
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lưu dữ liệu thất bại. Vui lòng thử lại!";
                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            var model = db.Products.Find(id);
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Product model)
        {
           
            var update = db.Products.Find(model.ProductID);
            update.ProductName = model.ProductName;
            update.Title = model.Title;
            update.Description = model.Description;
            update.CatID = model.CatID;
            update.Quantity = model.Quantity;
            update.Price = model.Price;
            update.PromotionPrice = model.PromotionPrice;
            update.Status = model.Status;
            update.isHome = model.isHome;
            update.isHot = model.isHot;
            update.isSale = model.isSale;
            update.CreateDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            var del = db.Products.Find(id);
            db.Products.Remove(del);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Status(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.Status = !item.Status;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.Status });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult isHome(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.isHome = !item.isHome;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.isHome });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public ActionResult isSale(int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                item.isSale = !item.isSale;
                db.SaveChanges();
                return Json(new { success = true, isActive = item.isSale });
            }
            return Json(new { success = false });
        }
    }
}
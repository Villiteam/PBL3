﻿using Antlr.Runtime.Tree;
using PagedList;
using PBL3.EF;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        //public ActionResult Add(Product model, List<string> Images, List<int> rDefault, List<int> SizeName)
        public ActionResult Add(ProductV productV, List<string> Images, List<int> rDefault)
        {
            var model = productV.Product;
            var listSize = productV.Sizes;
            if (listSize == null ||listSize.Count() <= 0)
            {
                TempData["error"] = "Sản phẩm chưa có kích cỡ!";
                return View(productV);
            }
            int sl = 0;
            foreach (var i in listSize)
            {
                if (i.SizeName == null || i.Quantity < 1)
                {
                    TempData["error"] = "Tên kích cỡ không được để trống và số lượng ít nhất là 1!";
                    return View(productV);
                }
                sl += i.Quantity;
            }
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
                //add vao csdl
                model.CreateDate = DateTime.Now;
                model.Quantity = sl;
                db.Products.Add(model);
                // luu lai thay doi
                db.SaveChanges();


                // thêm bảng số lượng   
                foreach (var v in listSize)
                {
                    var size = new Size();
                    size.ProductID = model.ProductID;
                    size.SizeName = v.SizeName;
                    size.Quantity = v.Quantity;
                    db.Sizes.Add(size);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lưu dữ liệu thất bại. Vui lòng thử lại!";
                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            var productV = new ProductV();
            productV.Product = db.Products.Find(id);
            productV.Sizes = db.Sizes.Where(m => m.ProductID == id).ToList();
            return View(productV);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ProductV productV)
        {
            var model = productV.Product;
            var listSize = productV.Sizes;

            if (listSize == null || listSize.Count() <= 0)
            {
                TempData["error"] = "Sản phẩm chưa có kích cỡ!";
                return View(productV);
            }
            int sl = 0;
            foreach (var i in listSize)
            {
                if (i.SizeName == null || i.Quantity < 1)
                {
                    TempData["error"] = "Tên kích cỡ không được để trống và số lượng ít nhất là 1!";
                    return View(productV);
                }
                sl += i.Quantity;
            }

            var update = db.Products.Find(model.ProductID);
            update.ProductName = model.ProductName;
            update.Title = model.Title;
            update.Description = model.Description;
            update.CatID = model.CatID;
            update.Quantity = sl;
            update.Price = model.Price;
            update.PromotionPrice = model.PromotionPrice;
            update.Status = model.Status;
            update.isHome = model.isHome;
            update.isHot = model.isHot;
            update.isSale = model.isSale;
            update.CreateDate = DateTime.Now;
            db.SaveChanges();


            // Xóa bảng size cũ của sản phẩm với productid
            var sizesToDelete = db.Sizes.Where(m => m.ProductID == model.ProductID);
            db.Sizes.RemoveRange(sizesToDelete);
            db.SaveChanges();

            // Lưu 1 bảng size mới 
            foreach (var v in listSize)
            {
                var size = new Size();
                size.ProductID = model.ProductID;
                size.SizeName = v.SizeName;
                size.Quantity = v.Quantity;
                db.Sizes.Add(size);
                db.SaveChanges();
            }

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
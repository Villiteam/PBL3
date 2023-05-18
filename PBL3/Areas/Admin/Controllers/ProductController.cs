using Antlr.Runtime.Tree;
using PagedList;
using PBL3.App_Start;
using PBL3.EF;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;

namespace PBL3.Areas.Admin.Controllers
{
    [AdminAuthorize(Role = new string[] { "Admin", "Manager" })]
    public class ProductController : Controller
    {
        // GET: Admin/Product
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index(int? page, string sort, string keyword, int? cate)
        {
            var pageSize = 7;
            if (page == null)
            {
                page = 1;
            }
            ViewBag.Page = (page - 1) * pageSize + 1;
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var list = db.Products.AsQueryable();

            // Lọc tên theo keyword
            ViewBag.Keyword = keyword;

            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(m => m.ProductName.Contains(keyword));
            }

            // Lọc theo danh mục  
            ViewBag.Cate = cate;

            if (cate != null)
            {
                list = list.Where(m => m.CatID == cate);
            }

            //Sort
            ViewBag.SortByDate = String.IsNullOrEmpty(sort) ? "date" : "";
            ViewBag.SortByName = (sort == "name_desc") ? "name" : "name_desc";
            ViewBag.SortByQuantity = (sort == "quantity_desc") ? "quantity" : "quantity_desc";
            ViewBag.SortByPrice = (sort == "price_desc") ? "price" : "price_desc";
            ViewBag.SortByPromotionPrice = (sort == "pprice_desc") ? "pprice" : "pprice_desc";
            ViewBag.SortByHome = (sort == "home_desc") ? "home" : "home_desc";
            ViewBag.SortBySale = (sort == "sale_desc") ? "sale" : "sale_desc";
            ViewBag.SortByStatus = (sort == "status_desc") ? "status" : "status_desc";

            switch (sort)
            {
                case "date":
                    list = list.OrderBy(m => m.CreateDate);
                    break;
                case "name":
                    list = list.OrderBy(m => m.ProductName);
                    break;
                case "name_desc":
                    list = list.OrderByDescending(m => m.ProductName);
                    break;
                case "quantity":
                    list = list.OrderBy(m => m.Quantity);
                    break;
                case "quantity_desc":
                    list = list.OrderByDescending(m => m.Quantity);
                    break;
                case "price":
                    list = list.OrderBy(m => m.Price);
                    break;
                case "price_desc":
                    list = list.OrderByDescending(m => m.Price);
                    break;
                case "pprice":
                    list = list.OrderBy(m => m.PromotionPrice);
                    break;
                case "pprice_desc":
                    list = list.OrderByDescending(m => m.PromotionPrice);
                    break;
                case "home":
                    list = list.OrderBy(m => m.isHome);
                    break;
                case "home_desc":
                    list = list.OrderByDescending(m => m.isHome);
                    break;
                case "sale":
                    list = list.OrderBy(m => m.isSale);
                    break;
                case "sale_desc":
                    list = list.OrderByDescending(m => m.isSale);
                    break;
                case "status":
                    list = list.OrderBy(m => m.Status);
                    break;
                case "status_desc":
                    list = list.OrderByDescending(m => m.Status);
                    break;
                default:
                    list = list.OrderByDescending(m => m.CreateDate);
                    break;
            }



            var pagedResult = list.ToPagedList(pageIndex, pageSize);
            return View(pagedResult);
        }

        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(ProductV productV, List<string> Images, List<int> rDefault)
        {
            var model = productV.Product;
            var listSize = productV.Sizes;
            var item = db.Products.SingleOrDefault(m => m.ProductName == model.ProductName);
            if (model.ProductName == null || model.ProductName == "")
            {
                TempData["error"] = "Vui lòng nhập tên sản phẩm!";
                return View();
            }
            if (item != null)
            {
                TempData["error"] = "Tên sản phẩm đã tồn tại!";
                return View();
            }
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
                return View(productV);
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
                TempData["type"] = "success";
                TempData["successMessage"] = "Thêm mới thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lưu dữ liệu thất bại. Vui lòng thử lại!";
                return View(productV);
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
            update.OriginalPrice = model.OriginalPrice;
            update.Price = model.Price;
            update.PromotionPrice = model.PromotionPrice;
            update.Status = model.Status;
            update.isHome = model.isHome;
            update.isSale = model.isSale;
            update.CreateDate = DateTime.Now;
            db.SaveChanges();


            // Cập nhật bảng size 
            var sizeOld = db.Sizes.Where(m => m.ProductID == model.ProductID).ToList();
            int slNew = listSize.Count();
            int slOld = sizeOld.Count();

            if (slNew < slOld)
            {
                TempData["error"] = "Số lượng kích cỡ không được ít hơn lúc tạo!  Vui lòng nhập ít nhất \"" + slOld + "\" kích cỡ!";
                return View(productV);
            }
            else
            {
                for (int i = 0; i < slNew; i++)
                {
                    if (i < slOld)
                    {
                        int id = sizeOld[i].SizeID;
                        var item = db.Sizes.Find(id);
                        if (item != null)
                        {
                            item.SizeName = listSize[i].SizeName;
                            item.Quantity = listSize[i].Quantity;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var size = new Size();
                        size.ProductID = model.ProductID;
                        size.SizeName = listSize[i].SizeName;
                        size.Quantity = listSize[i].Quantity;
                        db.Sizes.Add(size);
                        db.SaveChanges();
                    }
                }
            }
            TempData["type"] = "success";
            TempData["successMessage"] = "Cập nhật thành công!";
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            var del = db.Products.Find(id);
            db.Products.Remove(del);
            db.SaveChanges();
            TempData["type"] = "success";
            TempData["successMessage"] = "Xóa thành công!";
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
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index()
        {
            var ds = db.Categories.OrderByDescending(m => m.CatID).ToList();
            return View(ds);
        }
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Category model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //add vao csdl
                    db.Categories.Add(model);
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
            var model = db.Categories.Find(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(Category model)
        {
            var up = db.Categories.Find(model.CatID);
            up.CatName = model.CatName;
            up.Status = model.Status;
            up.ParentID = model.ParentID;
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        public ActionResult Delete(int id)
        {
            var del = db.Categories.Find(id);
            db.Categories.Remove(del);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
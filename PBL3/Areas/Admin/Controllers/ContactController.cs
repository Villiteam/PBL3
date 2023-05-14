using PagedList;
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Areas.Admin.Controllers
{
    public class ContactController : Controller
    {
        // GET: Admin/Contact
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index(int? page, string sort, string keyword)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            ViewBag.Page = (page - 1) * pageSize + 1;
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var query = db.Contacts.AsQueryable();

            // Lọc tên theo keyword
            ViewBag.Keyword = keyword;
            // Filter by keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(m => m.Name.Contains(keyword) || m.Email.Contains(keyword));
            }


            // Sort ViewBag
            ViewBag.SortByDate = String.IsNullOrEmpty(sort) ? "date" : "";
            ViewBag.SortByName = (sort == "name_desc") ? "name" : "name_desc";
            ViewBag.SortByEmail = (sort == "email_desc") ? "email" : "email_desc";

            // Sort
            switch (sort)
            {
                case "date":
                    query = query.OrderBy(m => m.CreatedDate);
                    break;
                case "name":
                    query = query.OrderBy(m => m.Name);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(m => m.Name);
                    break;
                case "email":
                    query = query.OrderBy(m => m.Email);
                    break;
                case "email_desc":
                    query = query.OrderByDescending(m => m.Email);
                    break;
         
                default:
                    query = query.OrderByDescending(m => m.CreatedDate);
                    break;
            }

            var list = query.ToPagedList(pageIndex, pageSize);

            return View(list);
        }
        public ActionResult Delete(int id)
        {
            var del = db.Contacts.Find(id);
            db.Contacts.Remove(del);
            db.SaveChanges();
            TempData["successMessage"] = "Xóa thành công!";
            TempData["type"] = "error";

            return RedirectToAction("Index");
        }
    }
}
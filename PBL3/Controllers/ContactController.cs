using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Controllers
{
    public class ContactController : Controller
    {
        // GET: Contact
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string name,string email , string message)
        {
            var contact = new Contact();
            contact.Name = name;
            contact.Email = email;
            contact.Message = message;
            db.Contacts.Add(contact);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
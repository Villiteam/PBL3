
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PBL3.Areas.Admin.Controllers
{
    public class CommentController : Controller
    {
        // GET: Admin/Comment
        private pbl3Entities db = new pbl3Entities();
        public ActionResult Index()
        {
            int productId = 22;
            var comments = (from od in db.OrderDetails
                            join c in db.Comments on od.OderDetailID equals c.OrderDetailID
                            where od.ProductID == productId
                            select c).ToList();
            ViewBag.comment = comments;
            return View();
        }

    }
}
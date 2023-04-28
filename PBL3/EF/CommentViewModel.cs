using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PBL3.EF
{
    public class CommentViewModel
    {
        public int CommentID { get; set; }
        public int OrderDetailID { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
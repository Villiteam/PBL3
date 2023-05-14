using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PBL3.EF
{
    [Serializable]
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public int SizeID { get; set; }
    }
}
using PBL3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PBL3.EF
{
    public class ProductV
    {
        public Product Product { get; set; }
        public List<Size> Sizes { get; set; }
    }
}
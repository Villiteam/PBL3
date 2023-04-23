using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PBL3.EF
{
    public class CustomPagedListRenderOptions : PagedListRenderOptions
    {
        public string CssClass { get; set; }
        public string[] PageClasses { get; set; }
        public string[] LinkClasses { get; set; }
        public string[] ActiveDisabledElementClasses { get; set; }
        public string[] NumericPagerItemContainerClasses { get; set; }
        public string[] NumericPagerItemClasses { get; set; }
    }

}
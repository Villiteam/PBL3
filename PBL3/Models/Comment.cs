//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PBL3.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comment
    {
        public int CommentID { get; set; }
        public int OrderDetailID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int UserID { get; set; }
        public int Rating { get; set; }
        public string Comment1 { get; set; }
    
        public virtual OrderDetail OrderDetail { get; set; }
        public virtual User User { get; set; }
    }
}

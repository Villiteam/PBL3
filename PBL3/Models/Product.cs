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
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.ProductImages = new HashSet<ProductImage>();
            this.Sizes = new HashSet<Size>();
            this.Warehouses = new HashSet<Warehouse>();
        }
    
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Title { get; set; }
        public Nullable<double> OriginalPrice { get; set; }
        public double Price { get; set; }
        public Nullable<double> PromotionPrice { get; set; }
        public string ListImages { get; set; }
        public string Video { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public bool isHome { get; set; }
        public bool isSale { get; set; }
        public bool Status { get; set; }
        public Nullable<int> ViewCount { get; set; }
        public Nullable<int> CatID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string Size { get; set; }
    
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Size> Sizes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Warehouse> Warehouses { get; set; }
    }
}

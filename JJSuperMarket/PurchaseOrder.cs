//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JJSuperMarket
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchaseOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseOrder()
        {
            this.PurchaseOrderDetails = new HashSet<PurchaseOrderDetail>();
        }
    
        public decimal POId { get; set; }
        public string POCode { get; set; }
        public Nullable<System.DateTime> PODate { get; set; }
        public Nullable<decimal> LedgerCode { get; set; }
        public Nullable<decimal> InvoiceNo { get; set; }
        public Nullable<double> Extra { get; set; }
        public Nullable<double> DiscountAmount { get; set; }
        public string Narration { get; set; }
        public Nullable<double> ItemAmount { get; set; }
    
        public virtual Supplier Supplier { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
    }
}

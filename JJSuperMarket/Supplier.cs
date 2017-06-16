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
    
    public partial class Supplier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Supplier()
        {
            this.Purchases = new HashSet<Purchase>();
            this.PurchaseOrders = new HashSet<PurchaseOrder>();
            this.PurchaseReturns = new HashSet<PurchaseReturn>();
        }
    
        public decimal SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string LedgerName { get; set; }
        public string SupplierName { get; set; }
        public string AddressLine { get; set; }
        public string TelePhoneNo { get; set; }
        public string MobileNo { get; set; }
        public string EMailId { get; set; }
        public string TinNo { get; set; }
        public string CST { get; set; }
        public Nullable<decimal> CreditDays { get; set; }
        public Nullable<double> CreditLimits { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Purchase> Purchases { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseReturn> PurchaseReturns { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JJSuperMarket.Reports
{
    /// <summary>
    /// Interaction logic for ReceivableReport.xaml
    /// </summary>
    public partial class ReceivableReport : UserControl
    {
        public ReceivableReport()
        {
            InitializeComponent();
            LoadReport();
        }

        private void LoadReport()
        {
            JJSuperMarketEntities db = new JJSuperMarketEntities();
            try
            {
                List<SupplierDueReport> Suplist = new List<SupplierDueReport>();

                foreach (var sam in db.Suppliers.ToList())
                {
                    foreach (var supl in db.PurchaseReturns.Where(x => x.LedgerCode == sam.SupplierId && x.PRType == "Credit").ToList())
                    {
                        var Pay = db.ReceiptMasters.Where(x => x.SupplierId == supl.Supplier.SupplierId).ToList();

                        SupplierDueReport c1 = new SupplierDueReport();
                        c1.SupplierName = supl.Supplier.SupplierName;
                        // c1.DueDate = String.Format("{0:dd-MM-yyyy}", (cust.Date.Value == null ? DateTime.Today : cust.Date.Value.AddDays(cust.Supplier.CreditDays == null ? 0 : (double)cust.Supplier.CreditDays.Value)));

                        c1.Amount = Convert.ToDecimal(string.Format("{0:N2}", supl.ItemAmount.Value));
                        c1.ReceiptAmount = Pay == null ? 0 : Convert.ToDecimal(string.Format("{0:N2}", Pay.Where(x => x.PurchaseRId == supl.InvoiceNo).Sum(x => x.ReceiptAmount).Value));
                        c1.Balance = Convert.ToDecimal(string.Format("{0:N2}", c1.Amount - c1.ReceiptAmount));

                        c1.PDate = string.Format("{0:dd-MM-yyyy}", supl.PRDate);
                        c1.PInvoiceNo = String.Format("PRINV {0}", supl.InvoiceNo);
                        //c1.IsOverdue = (DateTime.Now - cust.Date.Value.AddDays((double)(cust.Supplier.CreditDays == null ? 0 : cust.Supplier.CreditDays.Value))).TotalDays > 0; ;
                        if (c1.Balance > 0) Suplist.Add(c1);

                    }
                }

                //dgvReceivableCustomer.ItemsSource = ReceivableDetails.toList.Where(x => x.Type == "Customer").Select(x => new { PayName = x.PayName, Amount = x.Amount }).ToList();
                dgvReceivableSupplier.ItemsSource = Suplist;

                List<CustomerDueReport> Cuslist = new List<CustomerDueReport>();

                foreach (var Cus in db.Customers.ToList())
                {
                    foreach (var cust in db.Sales.Where(x => x.LedgerCode == Cus.CustomerId && x.SalesType == "Credit").ToList())
                    {
                        var Pay = db.ReceiptMasters.Where(x => x.CustomerId == cust.Customer.CustomerId).ToList();

                        CustomerDueReport c1 = new CustomerDueReport();
                        c1.CustomerName = cust.Customer.CustomerName;
                        // c1.DueDate = String.Format("{0:dd-MM-yyyy}", (cust.Date.Value == null ? DateTime.Today : cust.Date.Value.AddDays(cust.Supplier.CreditDays == null ? 0 : (double)cust.Supplier.CreditDays.Value)));

                        c1.Amount = Convert.ToDecimal(string.Format("{0:N2}", cust.ItemAmount.Value));
                        c1.ReceiptAmount = Pay == null ? 0 : Convert.ToDecimal(string.Format("{0:N2}", Pay.Where(x => x.SalesId == cust.InvoiceNo).Sum(x => x.ReceiptAmount).Value));
                        c1.Balance = Convert.ToDecimal(string.Format("{0:N2}", c1.Amount - c1.ReceiptAmount));

                        c1.InDate = string.Format("{0:dd-MM-yyyy}", cust.SalesDate);
                        c1.InvoiceNo = String.Format("INV {0}", cust.InvoiceNo);
                        //c1.IsOverdue = (DateTime.Now - cust.Date.Value.AddDays((double)(cust.Supplier.CreditDays == null ? 0 : cust.Supplier.CreditDays.Value))).TotalDays > 0; ;
                        if (c1.Balance > 0) Cuslist.Add(c1);

                    }
                }
                dgvReceivableCustomer.ItemsSource = Cuslist;
            }
            catch (Exception ex)
            {


            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        class CustomerDueReport
        {
            public string InvoiceNo { get; set; }
            public string InDate { get; set; }
            public string CustomerName { get; set; }
            public decimal Amount { get; set; }
            public decimal ReceiptAmount { get; set; }
            public decimal Balance { get; set; }
        }

        class SupplierDueReport
        {
            public string PInvoiceNo { get; set; }
            public string PDate { get; set; }
            public string SupplierName { get; set; }
            public decimal Amount { get; set; }
            public decimal ReceiptAmount { get; set; }
            public decimal Balance { get; set; }
        }
    }
}

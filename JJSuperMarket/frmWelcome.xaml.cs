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

namespace JJSuperMarket
{
    /// <summary>
    /// Interaction logic for frmWelcome.xaml
    /// </summary>
    public partial class frmWelcome : UserControl
    {
       JJSuperMarketEntities db = new JJSuperMarketEntities();
        public frmWelcome()
        {
            InitializeComponent();
            dtpDate.SelectedDate = DateTime.Now;
            dtpDate.DisplayDateEnd = DateTime.Now;
            dtpSTDate.SelectedDate = DateTime.Now;
            dtpPDate.SelectedDate = DateTime.Now;
            lblMonth.Content = string.Format("{0}", DateTime.Now.ToString("MMMM"));
            lblPMonth.Content = string.Format("{0:MMM}", DateTime.Now.ToString("MMMM"));

        }

        private void LoadWindow()
        {
          
             db = new JJSuperMarketEntities();
            //  StockDetails.UpdateStockDetails();
            DateTime dtFrom = DateTime.Now.AddDays(-7);
            DateTime dtTo = DateTime.Now;

            try
            {
                var list1 = db.Sales.GroupBy(x => x.SalesDate).OrderByDescending(x=>x.Key).Take(30).Select(x => new { SalesDate = x.Key, BillAmount = x.Sum(y=>y.ItemAmount ) }).ToList();
                SalesGrid.ItemsSource = list1.Where(x=>x.BillAmount!=null).Select(x => new { Date = string.Format("{0:dd/MM/yyyy}", x.SalesDate), Amount = string.Format("{0:N2}", x.BillAmount) }).ToList();

                //var list2 = db.SalesDetails.Where(x => x.Sale.SalesDate <= DateTime.Now).OrderByDescending(x => x.SDId).Take(50).GroupBy(x => x.Product.ProductName) .Select(x => new { ProductName = x.Key, Amount = x.Sum(y => (y.Quantity * y.DisPer)) }).ToList();
                //StockGrid.ItemsSource = list2.Select(x => new { ProductName = x.ProductName, Amount = string.Format("{0:N2}", x.Amount) }).ToList();


                // var List3 = StockDetails.toList.Select(x => new { ProductName = x.ProductName.ToString(), Quantity = x.ClStock }).OrderBy(x=>x.ProductName).ToList();
                // StockReportGrid.ItemsSource = List3;


                //  var List4 = StockDetails.toList.Where(x => x.ClStock <= x.ReOrderLevel).OrderBy(x=>x.ProductName).ToList();
                //  ReOrderLevelGrid.ItemsSource = List4.Select(x => new { ProductName = x.ProductName, ReOrderLevel = x.ReOrderLevel, CloseingStock = x.ClStock }).ToList();
                //var list2 = db.Sales.Where(x=>x.SalesDate>=dtFrom && x.SalesDate<=dtTo).GroupBy(x => x.SalesDate).OrderBy(x => x.Sum(y => y.ItemAmount)).Select(x => new { SalesDate = x.Key, BillAmount = x.Sum(y => y.ItemAmount) }).ToList();
                //List<KeyValuePair<string, int>> MyValue1 = new List<KeyValuePair<string, int>>();
                //MyValue1 = list2.Where(x => x.BillAmount != null).Select(x => new KeyValuePair<string, int>(string.Format("{0:dd/MM/yyyy}", x.SalesDate), (int)double.Parse(x.BillAmount.ToString()))).ToList();
                //BarChart1.DataContext = MyValue1;

                //List<KeyValuePair<string, int>> MyValue2 = new List<KeyValuePair<string, int>>();
                //MyValue2 = list2.Where(x => x.Amount != null).Select(x => new KeyValuePair<string, int>(x.ProductName.Length > 20 ? x.ProductName.Substring(0, 20) : x.ProductName, (int)double.Parse(x.Amount.ToString()))).ToList();
                //BarChart.DataContext = MyValue2;

                salesTypeReport();
                CalculateNetProfit();

                var salesList = db.SalesDetails.Where(x => x.Sale.SalesDate>=dtFrom &&  x.Sale.SalesDate<=dtTo).GroupBy(x=>x.Product.StockGroup.GroupName).OrderByDescending(x=> (x.Sum(y => y.DisPer * y.Quantity))).ToList();
                dgvCatagorWiseSales.ItemsSource = salesList.Select(x => new {  Category = x.FirstOrDefault().Product.StockGroup.GroupName, Qty = string.Format("{0:N2}", x.Sum(y=>y.Quantity)), Amount= string.Format("{0:N2}", (x.Sum(y=>y.DisPer * y.Quantity)) )}).ToList();

                var count = db.Sales.Where(x => x.SalesDate.Value == dtpDate.SelectedDate.Value).GroupBy(x=>x.Customer.CustomerName).OrderBy(x => x.Sum(y=>y.ItemAmount)).ToList();
                lblName.Content = count.Count().ToString() + " Customer Visited. [Details]";
                dgvSaleInfo.ItemsSource = count.Select(x => new { CustomerName = x.Key, Amount = string.Format("{0:N2}", x.Sum(y => y.ItemAmount)) }).ToList();

                List<CustomerPoints> CusPoint = new List<CustomerPoints>();
                 
                foreach (var Cus in  db.Customers.ToList())
                {
                    CustomerPoints c1 = new CustomerPoints();
                    c1.NoOfVisit = Cus.Sales.Count();
                    c1.CustomerName = Cus.CustomerName;
                    c1.TotalAmount = Convert.ToDecimal(string.Format("{0:N2}", Cus.Sales.Sum(x => x.ItemAmount)));
                    c1.Points = Convert.ToDecimal(string.Format("{0:N2}", Cus.Sales.Sum(x => x.ItemAmount) * 0.01));
                    CusPoint.Add(c1);
                }
                dgvCustomerInfo.ItemsSource = CusPoint.OrderByDescending(x => x.Points).Take(10).ToList();
            }
            catch (Exception ex) { }
        }

        private void salesTypeReport()
        {
            try
            {

                var startDate = new DateTime(dtpSTDate.SelectedDate.Value.Year, dtpSTDate.SelectedDate.Value.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                lblMonth.Content = string.Format("{0}", startDate.ToString("MMMM"));

                var sales1 = db.Sales.Where(x => x.SalesDate >= startDate && x.SalesDate <= endDate).GroupBy(x => x.SalesType).Select(x=>new { SalesType=x.Key, Amount=x.Sum(y=>y.ItemAmount) }).ToList();
               
                dgvSalesTypeWiseReport.ItemsSource =sales1;
            }
            catch(Exception ex)
            { }
        }

        private void PackIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //frmAccountGroups frm = new JJSuperMarket.frmAccountGroups() ;
            //frm.Show();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void dtpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dtpDate.SelectedDate.Value != null)
                {
                    var count = db.Sales.Where(x => x.SalesDate.Value == dtpDate.SelectedDate.Value).GroupBy(x=>x.Customer.CustomerName).ToList();
                    lblName.Content = count.Count().ToString() + " Customer Visited. [Details]";
                    dgvSaleInfo.ItemsSource = count.Select(x => new { CustomerName = x.Key, Amount = string.Format("{0:N2}", x.Sum(y=>y.ItemAmount)) }).OrderBy(x=>x.Amount) .ToList();
                }
            }
            catch (Exception ex)
            {

                 
            }
           
           
        }

        class CustomerPoints
        {
            public int NoOfVisit { get; set; }
            public string CustomerName { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal Points { get; set; }
        }
        class SalesTypeReport
        {
           public string SalesType { get; set; }
            public double Amount { get; set; }
       
        }
        private void dtpCDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //var salesList = db.SalesDetails.Where(x => x.Sale.SalesDate == dtpCDate.SelectedDate).GroupBy(x => x.Product.StockGroup.GroupName).ToList();
                //dgvCatagorWiseSales.ItemsSource = salesList.Select(x => new { Category = x.FirstOrDefault().Product.StockGroup.GroupName, Qty = string.Format("{0:N2}", x.Sum(y => y.Quantity)), Amount = string.Format("{0:N2}", (x.Sum(y => y.DisPer * y.Quantity))) }).OrderByDescending(x => x.Category).ToList();

            }
            catch(Exception ex)
            {

            }


        }

        private void dtpSTDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            salesTypeReport();
        }

        private void dtpPDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateNetProfit();
        }

        private void CalculateNetProfit()
        {
            DateTime startDate = new DateTime(dtpPDate.SelectedDate.Value.Year, dtpPDate.SelectedDate.Value.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);
            TimeSpan ts = startDate.Subtract(endDate);
            int n = Math.Abs(ts.Days);
            NetProfit np = new NetProfit();
            List<NetProfit> listNp = new List<NetProfit>();
            lblPMonth.Content = string.Format("{0}", startDate.ToString("MMMM"));
            for (int i=0;i<=n;i++)
            {
                double? sAmt = 0, pAmt1 = 0,pAmt2=0,TPAmt=0, Total = 0;
                np = new NetProfit();
                np.Date = string.Format("{0:dd/MM/yyyy}", startDate.AddDays(i).Date);
                DateTime dtFrom = startDate.AddDays(i);
                pAmt1 = db.Purchases.Where(x => x.PurchaseDate==dtFrom).Sum(x => x.ItemAmount)==null?0: db.Purchases.Where(x => x.PurchaseDate == dtFrom).Sum(x => x.ItemAmount);
                 pAmt2= db.PurchaseMasters.Where(x => x.PurchaseDate == dtFrom).Sum(x => x.ItemAmount)==null?0: db.PurchaseMasters.Where(x => x.PurchaseDate == dtFrom).Sum(x => x.ItemAmount);                
                TPAmt = pAmt1 + pAmt2;
                np.PurRate = TPAmt == null ? 0 : (double)TPAmt;
                sAmt = db.Sales.Where(x => x.SalesDate == dtFrom).Sum(x => x.ItemAmount);
                np.SelRate = sAmt == null ? 0 : (double)sAmt;
                if (np.PurRate != 0 || np.SelRate != 0)
                {
                  np.Profit =  (np.SelRate)- (np.PurRate);
                    listNp.Add(np);
                }
                
            }
            np = new NetProfit();
            np.Date = "Total";
            np.PurRate =listNp.Sum(x=>x.PurRate);
            np.SelRate = listNp.Sum(x=>x.SelRate);
            np.Profit = listNp.Sum(x=>x.Profit);
            listNp.Add(np);

            dgvProfitReport.ItemsSource = listNp;


        }
        
        class NetProfit
        {
            public string Date { get; set; }
            public double? PurRate { get; set; }
            public double? SelRate { get; set; }
            public double? Profit { get; set; }
        }

    
    }
}

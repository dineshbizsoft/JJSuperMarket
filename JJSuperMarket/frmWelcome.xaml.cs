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
       // JJSuperMarketEntities db = new JJSuperMarketEntities();
        public frmWelcome()
        {
            InitializeComponent();
            
        }
       
        private void LoadWindow()
        {
           // db = new JJSuperMarketEntities();
            JJSuperMarketEntities db = new JJSuperMarketEntities();
            StockDetails.UpdateStockDetails();
            
            try
            {
                var list1 = db.Sales.GroupBy(x => x.SalesDate).OrderByDescending(x => x.Key).Take(30).Select(x => new { SalesDate = x.Key, BillAmount = x.Sum(y=>y.ItemAmount ) }).ToList();
                SalesGrid.ItemsSource = list1.Select(x => new { Date = string.Format("{0:dd/MM/yyyy}", x.SalesDate), Amount = string.Format("{0:N2}", x.BillAmount) }).ToList();

                var list2 = db.SalesDetails.Where(x => x.Sale.SalesDate <= DateTime.Now).OrderByDescending(x => x.SDId).Take(50).GroupBy(x => x.Product.ProductName) .Select(x => new { ProductName = x.Key, Amount = x.Sum(y => (y.Quantity * y.DisPer)) }).ToList();
                StockGrid.ItemsSource = list2.Select(x => new { ProductName = x.ProductName, Amount = string.Format("{0:N2}", x.Amount) }).ToList();

               
                var List3 = StockDetails.toList.Select(x => new { ProductName = x.ProductName.ToString(), Quantity = x.ClStock }).OrderBy(x=>x.ProductName).ToList();
                StockReportGrid.ItemsSource = List3;
               

                var List4 = StockDetails.toList.Where(x => x.ClStock <= x.ReOrderLevel).OrderBy(x=>x.ProductName).ToList();
                ReOrderLevelGrid.ItemsSource = List4.Select(x => new { ProductName = x.ProductName, ReOrderLevel = x.ReOrderLevel, CloseingStock = x.ClStock }).ToList();
                ReOrderLevelGrid.SelectedItem = ReOrderLevelGrid.Items[100];
                ReOrderLevelGrid.ScrollIntoView(ReOrderLevelGrid.Items[100]);

                List<KeyValuePair<string, int>> MyValue1 = new List<KeyValuePair<string, int>>();
                MyValue1 = list1.Where(x => x.BillAmount != null).Select(x => new KeyValuePair<string, int>(string.Format("{0:dd/MM/yyyy}", x.SalesDate), (int)double.Parse(x.BillAmount.ToString()))).ToList();
                BarChart1.DataContext = MyValue1;

                List<KeyValuePair<string, int>> MyValue2 = new List<KeyValuePair<string, int>>();
                MyValue2 = list2.Where(x => x.Amount != null).Select(x => new KeyValuePair<string, int>(x.ProductName.Length > 20 ? x.ProductName.Substring(0, 20) : x.ProductName, (int)double.Parse(x.Amount.ToString()))).ToList();
                BarChart.DataContext = MyValue2;
            }
            catch (Exception ex) { }
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JJSuperMarket.Transaction
{
    /// <summary>
    /// Interaction logic for frmProductDetails.xaml
    /// </summary>
    public partial class frmProductDetails : Window
    {
        List<Product> lstProduct = new List<Product>();
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        public string ProName;
        public frmProductDetails()
        {
            InitializeComponent();
            lstProduct = db.Products.OrderBy(x => x.ProductName).ToList();
        }
        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion
        private void cmbProductSrch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cmbProductSrch.Text))
            {
                dgvProduct.ItemsSource = lstProduct.Where(x => x.ProductName.ToLower().Contains(cmbProductSrch.Text.ToLower())).ToList();
            }
        }

        private void dgvProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Product  p = dgvProduct .SelectedItem as Product ;

               ProName  = p.ProductName;
                this.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtItem.Focus();
            dgvProduct.ItemsSource = lstProduct.ToList();
        }

        private void txtItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                dgvProduct.ItemsSource = db.Products.Where(x => x.ItemCode == txtItem.Text).ToList();
            }
        }

        private void txtItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtItem.Text))
            {
                dgvProduct.ItemsSource = lstProduct.ToList();
            }
        }
    }
}

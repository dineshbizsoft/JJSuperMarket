﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using JJSuperMarket.Domain;
using MaterialDesignThemes.Wpf;
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;

namespace JJSuperMarket.MasterSetup
{
    /// <summary>
    /// Interaction logic for frmProducts.xaml
    /// </summary>
    public partial class frmProducts : UserControl
    {
        JJSuperMarketEntities db = new JJSuperMarketEntities();
        decimal ID = 0;


        public frmProducts()
        {
            InitializeComponent();
            LoadWindow();
            LoadReport();
        }

        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #region Button Events

        private void btnBrowser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog OpenDialogBox = new OpenFileDialog();
                OpenDialogBox.DefaultExt = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";
                OpenDialogBox.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";

                var browsefile = OpenDialogBox.ShowDialog();
                if (browsefile == true)
                {
                    string sFileName = OpenDialogBox.FileName.ToString();
                    if (!string.IsNullOrEmpty(sFileName))
                    {
                        ImageSource imageSource = new BitmapImage(new Uri(sFileName));

                        //    iProductImage.Source = imageSource;
                        //    iProductImage.Tag = AppLib.ReadImageFile(sFileName);
                    }

                }
            }
            catch (Exception ex)
            { }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double open, reorder;
                open = txtOpeningStock.Text == "" ? 0 : Convert.ToDouble(txtOpeningStock.Text);
                reorder = txtReorderLevel.Text == "" ? 0 : Convert.ToDouble(txtReorderLevel.Text);

                if (txtProductName.Text == "")
                {

                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Product Name.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtProductName.Focus();
                }
                else if (txtBarcode.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Item Code.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtBarcode.Focus();
                }
                else if (cmbUOM.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter UOM.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtBarcode.Focus();
                }
                else if (cmbUnder.Text == "")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter Under category.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtBarcode.Focus();
                }
                else if (txtOpeningStock.Text == "" || txtOpeningStock.Text == "0")
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Enter the Opening Stock.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtOpeningStock.Focus();
                }
                //else if (txtReorderLevel.Text == "" || txtReorderLevel.Text == "0")
                //{
                //    var sampleMessageDialog = new SampleMessageDialog
                //    {
                //        Message = { Text = "Enter Reorder Level.." }
                //    };

                //    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                //    txtReorderLevel.Focus();
                //}
                else if (open < reorder)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "Opening and Reorderlevel Mismatch.." }
                    };
                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtReorderLevel.Focus();
                }
                else if (ID != 0)
                {
                    bool r = true;
                    string name = db.Products.Where(x => x.ProductId == ID).Select(x => x.ProductName).FirstOrDefault().ToString();
                    string name1 = db.Products.Where(x => x.ProductId == ID).Select(x => x.ItemCode).FirstOrDefault().ToString();
                    if (txtProductName.Text != name)

                    {
                        if (await validation() == false)
                        {
                            r = false;
                        }

                    }
                    if (txtBarcode.Text != name1)
                    {
                        if (await validation() == false)
                        {
                            r = false;
                        }
                    }

                    if (r == true)
                    {
                        Product c = db.Products.Where(x => x.ProductId == ID).FirstOrDefault();

                        c.ProductName = txtProductName.Text;
                        c.PurchaseRate = (txtPurchaseRate.Text.ToString() == "" ? 0 : Convert.ToDouble(txtPurchaseRate.Text.ToString()));
                        c.SellingRate = (txtSellingRate.Text.ToString() == "" ? 0 : Convert.ToDouble(txtSellingRate.Text.ToString()));
                        c.UOMCode = (cmbUOM.Text == "" ? 0 : Convert.ToDecimal(cmbUOM.SelectedValue));
                        c.GroupCode = cmbUnder.Text == "" ? 0 : (Convert.ToDecimal(cmbUnder.SelectedValue));
                        c.ItemCode = txtBarcode.Text;
                        c.MRP = txtMRP.Text == "" ? 0 : Convert.ToDouble(txtMRP.Text);
                        c.OpQty = (txtOpeningStock.Text.ToString() == "" ? 0 : Convert.ToDouble(txtOpeningStock.Text.ToString()));
                        c.ReOrderLevel = (txtReorderLevel.Text.ToString() == "" ? 0 : Convert.ToDouble(txtReorderLevel.Text.ToString()));
                        // c.ProductImage = (iProductImage.Tag == null ? null : (byte[])iProductImage.Tag);

                        db.SaveChanges();
                        var sampleMessageDialog = new SampleMessageDialog
                        {
                            Message = { Text = "Updated Successfully.." }
                        };

                        await DialogHost.Show(sampleMessageDialog, "RootDialog");
                        FormClear();
                        LoadWindow();
                        LoadReport();

                    }


                }
                else
                {

                    if (await validation() == true)
                    {
                        Product c = new Product();
                        c.ProductName = txtProductName.Text;
                        c.PurchaseRate = (txtPurchaseRate.Text.ToString() == "" ? 0 : Convert.ToDouble(txtPurchaseRate.Text.ToString()));
                        c.SellingRate = (txtSellingRate.Text.ToString() == "" ? 0 : Convert.ToDouble(txtSellingRate.Text.ToString()));
                        c.UOMCode = (cmbUOM.SelectedValue.ToString() == "" ? 0 : Convert.ToDecimal(cmbUOM.SelectedValue));
                        c.GroupCode = cmbUnder.Text == "" ? 0 : (Convert.ToDecimal(cmbUnder.SelectedValue));
                        c.ItemCode = txtBarcode.Text;
                        c.MRP = txtMRP.Text == "" ? 0 : Convert.ToDouble(txtMRP.Text);
                        c.OpQty = (txtOpeningStock.Text.ToString() == "" ? 0 : Convert.ToDouble(txtOpeningStock.Text.ToString()));
                        c.ReOrderLevel = (txtReorderLevel.Text.ToString() == "" ? 0 : Convert.ToDouble(txtReorderLevel.Text.ToString()));
                        //  c.ProductImage = (iProductImage.Tag == null ? null : (byte[])iProductImage.Tag);


                        db.Products.Add(c);
                        db.SaveChanges();
                        var sampleMessageDialog = new SampleMessageDialog
                        {
                            Message = { Text = "Saved successfully.." }
                        };

                        await DialogHost.Show(sampleMessageDialog, "RootDialog");
                        FormClear();
                        LoadWindow();
                        LoadReport();
                    }
                }

            }
            catch (Exception ex)
            {
                //todo
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            FormClear();
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ID == 0)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "No Records to Delete.." }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                }
                else
                {
                    var sampleDialog = new SampleDialog
                    {
                        Message = { Text = "Do you want to delete this record?.." }
                    };

                    var result = Convert.ToBoolean(await DialogHost.Show(sampleDialog, "RootDialog"));
                    if (result == true)
                    {
                        Product c = db.Products.Where(x => x.ProductId == ID).FirstOrDefault();
                        db.Products.Remove(c);
                        db.SaveChanges();
                        var sampleMessageDialog = new SampleMessageDialog
                        {
                            Message = { Text = "Deleted Successfully.." }
                        };

                        await DialogHost.Show(sampleMessageDialog, "RootDialog");
                        FormClear();
                        LoadWindow();
                        LoadReport();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(" Can't Delete Contact Admin..");
                db = new JJSuperMarketEntities();
                FormClear();
                LoadWindow();
                LoadReport();
            }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadWindow();
        }

        private void btnSearchReport_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void dgvProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Product c = dgvProduct.SelectedItem as Product;
                ID = c.ProductId;
                txtProductName.Text = c.ProductName;
                txtPurchaseRate.Text = c.PurchaseRate.ToString();
                txtSellingRate.Text = c.SellingRate.ToString();
                cmbUOM.Text = c.UnitsOfMeasurement.UOMSymbol;
                cmbUnder.Text = c.StockGroup.GroupName;
                txtBarcode.Text = c.ItemCode;
                txtMRP.Text = c.MRP.ToString();
                txtOpeningStock.Text = c.OpQty.ToString();
                txtReorderLevel.Text = c.ReOrderLevel.ToString();
                //  iProductImage.Source = AppLib.ViewImage(c.ProductImage);
                // iProductImage.Tag = c.ProductImage;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region User Define
        private async Task<bool> validation()
        {


            var item = db.Products.Where(x => x.ProductName == txtProductName.Text).FirstOrDefault();
            if (item != null)
            {
                if (item.ProductId != ID)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "" + txtProductName.Text + ", Already Exist.Enter New One " }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtProductName.Focus();

                    return false;
                }
            }

            item = db.Products.Where(x => x.ItemCode == txtBarcode.Text).FirstOrDefault();
            if (item != null)
            {
                if (item.ProductId != ID)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = { Text = "This Barcode assign to " + item.ProductName + ". Enter New Bar Code " }
                    };

                    await DialogHost.Show(sampleMessageDialog, "RootDialog");
                    txtBarcode.Focus();

                    return false;
                }
            }

            return true;
        }
        private void FormClear()
        {
            txtProductId.Clear();
            txtProductName.Clear();
            txtSellingRate.Clear();
            txtPurchaseRate.Clear();
            txtMRP.Clear();
            txtBarcode.Clear();
            txtItem.Clear();
            cmbProductSrch.Clear();
            cmbUOM.Text = "";
            cmbUnder.Text = "";
            txtOpeningStock.Clear();
            txtReorderLevel.Clear();
            // iProductImage.Source = null;
            ID = 0;
        }
        private void LoadWindow()
        {
            try
            {
                var c1 = db.UnitsOfMeasurements.ToList();
                cmbUOM.ItemsSource = c1;
                cmbUOM.SelectedValuePath = "UOMId";
                cmbUOM.DisplayMemberPath = "UOMSymbol";

                if (cmbProductSrch.Text != "")
                {
                    var p = db.Products.Where(x => x.ProductName == cmbProductSrch.Text).OrderByDescending(x => x.ProductId).ToList();
                    foreach (var p1 in p)
                    {

                    }
                }
                else
                {
                    dgvProduct.ItemsSource = db.Products.OrderByDescending(x => x.ProductId).ToList();
                }
            }
            catch (Exception ex)
            { }
        }
        private void LoadReport()
        {
            try
            {
                Reportviewer.Reset();
                DataTable dt = getData();
                ReportDataSource Data = new ReportDataSource("Product", dt);

                Reportviewer.LocalReport.DataSources.Add(Data);
                Reportviewer.LocalReport.ReportEmbeddedResource = "JJSuperMarket.Reports.Master.rptProductReport.rdlc";

                Reportviewer.RefreshReport();
            }
            catch (Exception ex)
            {

            }

        }
        private DataTable getData()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(AppLib.conStr))
            {
                SqlCommand cmd;

                string qry = string.Format(" select p.ProductName,p.ItemCode,p.PurchaseRate,p.SellingRate,p.mrp  From Products as P left join StockGroups as s on p.GroupCode=s.GroupCode order by ProductName");
                cmd = new SqlCommand(qry, con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);


            }
            return dt;
        }

        private void cmbUnder_DropDownOpened(object sender, EventArgs e)
        {
            var U = db.StockGroups.OrderBy(x => x.GroupName).ToList();
            cmbUnder.ItemsSource = U;
            cmbUnder.SelectedValuePath = "StockGroupId";
            cmbUnder.DisplayMemberPath = "GroupName";
        }


        #endregion



        private void txtItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                dgvProduct.ItemsSource = db.Products.Where(x => x.ItemCode == txtItem.Text).ToList();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //  lstProduct = db.Products.OrderBy(x => x.ProductName).ToList();
            LoadReport();

        }

        private void cmbProductSrch_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtItem.Clear();
            if (!string.IsNullOrWhiteSpace(cmbProductSrch.Text))
            {
                dgvProduct.ItemsSource = db.Products.Where(x => x.ProductName.ToLower().Contains(cmbProductSrch.Text.ToLower())).OrderBy(x => x.ProductName).ToList();
            }
            else
            {
                dgvProduct.ItemsSource = db.Products.ToList();
            }

        }

        private void txtItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            cmbProductSrch.Clear();
            if (string.IsNullOrEmpty(txtItem.Text))
            {
                dgvProduct.ItemsSource = db.Products.ToList().OrderBy(x => x.ProductName);
            }

        }
    }
}
public class ProductDetails
{
    public string SNo { get; set; }
    public string ProductName { get; set; }
    public string Under { get; set; }
}
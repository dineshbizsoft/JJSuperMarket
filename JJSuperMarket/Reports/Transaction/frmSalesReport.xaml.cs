﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;
using JJSuperMarket.Domain;
using Microsoft.Reporting.WinForms;
using System.Text.RegularExpressions;

namespace JJSuperMarket.Reports.Transaction
{
    /// <summary>
    /// Interaction logic for frmSalesReport.xaml
    /// </summary>
    public partial class frmSalesReport : UserControl 
    {
        string qry = "";
        JJSuperMarketEntities db = new JJSuperMarketEntities();

        public frmSalesReport()
        {
            InitializeComponent();
            LoadReport();
            LoadWindow();

        }
        #region Numeric Only
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
       
        #endregion
        private void LoadWindow()
        {
            dtpFromDate.SelectedDate = DateTime.Today.AddDays(-30) ;
            dtpToDate.SelectedDate = DateTime.Today;
            txtBillAmtFrom.Text = "0";
            txtBillAmtTo.Text = "1000";
            var v = db.Customers.ToList();
            cmbCustomer.ItemsSource = v;
            cmbCustomer.DisplayMemberPath = "CustomerName";
            cmbCustomer.SelectedValuePath = "CustomerName";
           
        }

        private void LoadReport()
        {
            try
            {
                SalesReport.Reset();
                DataTable dt = getData();
                ReportDataSource Data = new ReportDataSource("Sales", dt);

                SalesReport.LocalReport.DataSources.Add(Data);
                SalesReport.LocalReport.ReportEmbeddedResource = "JJSuperMarketReports.Transaction.rptSales.rdlc";
                SalesReport.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(PurchaseDetails);

                SalesReport.RefreshReport();
            }
            catch (Exception ex)
            {

            }
        }

        private void PurchaseDetails(object sender, SubreportProcessingEventArgs e)
        {
            int c = int.Parse(e.Parameters["SalesId"].Values[0]);
            DataTable dt = GetDetails(c);
            ReportDataSource rs = new ReportDataSource("SalesDetails", dt);
            e.DataSources.Add(rs);
        }

        private DataTable GetDetails(int c)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppLib.conStr))
            {
                string qry = String.Format("select P.ProductName as ProductCode, POD.Quantity,U.UOMSymbol as UOM, POD.Rate,POD.TaxPer, POD.DisPer from SalesDetails as POD left join Products as P on POD.ProductCode = p.ProductId left join UnitsOfMeasurement as U on POD.UOM=U.UOMId where POD.SalesId='{0}'", c);
                SqlCommand cmd = new SqlCommand(qry, conn);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            return dt;
        }

        private DataTable getData()
        {
            Wqry();
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(AppLib.conStr))
            {
                SqlCommand cmd;
                string qry1 = string.Format("select   PO.SalesId,s.CustomerName as LedgerCode,PO.SalesDate, PO.InvoiceNo,PO.DiscountAmount,PO.Extra,PO.ItemAmount,po.salestype from Sales as PO left join Customer as s on PO.LedgerCode = s.CustomerId where {0}", qry);
                cmd = new SqlCommand(qry1, con);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            return dt;

        }

        public string Wqry()
        {
            DateTime fromDate = Convert.ToDateTime(dtpFromDate.SelectedDate);
            DateTime toDate = Convert.ToDateTime(dtpToDate.SelectedDate);
            Double billFrom = Convert.ToDouble(txtBillAmtFrom.Text);
            Double billTo = Convert.ToDouble(txtBillAmtTo.Text);
            qry = String.Format("PO.SalesDate>='{0:yyyy-MM-dd}' and PO.SalesDate<='{1:yyyy-MM-dd}' and PO.ItemAmount>='{2}' and PO.ItemAmount<='{3}'", fromDate, toDate, billFrom, billTo);
            if (cmbCustomer.Text != "")
            {

                qry = qry + "and S.CustomerName='" + cmbCustomer.Text + "'";

            }
            if (txtInvoiceNo.Text != "")
            {
                qry = qry + "and PO.InvoiceNo='" + txtInvoiceNo.Text + "'";

            }
            if (cmbSalesType.Text != "")
            {
                qry=qry+"and po.salestype='"+cmbSalesType.Text+"'";
            }
            return qry;
        }

        

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }
    }
}

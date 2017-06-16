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

namespace JJSuperMarket.Reports.Transaction
{
    /// <summary>
    /// Interaction logic for frmPurchaseReturnReport.xaml
    /// </summary>
    public partial class frmPurchaseReturnReport : Window
    {
        string qry = "";
        JJSuperMarketEntities db = new JJSuperMarketEntities();

        public frmPurchaseReturnReport()
        {
            InitializeComponent();
            LoadReport();
            LoadWindow();

        }

        private void LoadWindow()
        {
            dtpFromDate.SelectedDate = DateTime.Today;
            dtpToDate.SelectedDate = DateTime.Today;
            txtBillAmtFrom.Text = "0";
            txtBillAmtTo.Text = "99999999";
            var v = db.Suppliers.ToList();
            cmbSupplier.ItemsSource = v;
            cmbSupplier.DisplayMemberPath = "SupplierName";
            cmbSupplier.SelectedValuePath = "SupplierName";
        }

        private void LoadReport()
        {
            try
            {
                PurchaseReturnReport.Reset();
                DataTable dt = getData();
                ReportDataSource Data = new ReportDataSource("PurchaseReturn", dt);

                PurchaseReturnReport.LocalReport.DataSources.Add(Data);
                PurchaseReturnReport.LocalReport.ReportEmbeddedResource = "JJSuperMarketReports.Transaction.rptPurchaseReturn.rdlc";
                PurchaseReturnReport.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(PurchaseDetails);

                PurchaseReturnReport.RefreshReport();
            }
            catch (Exception ex)
            {

            }
        }

        private void PurchaseDetails(object sender, SubreportProcessingEventArgs e)
        {
            int c = int.Parse(e.Parameters["PRId"].Values[0]);
            DataTable dt = GetDetails(c);
            ReportDataSource rs = new ReportDataSource("PurchaseReturnDetails", dt);
            e.DataSources.Add(rs);
        }

        private DataTable GetDetails(int c)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(AppLib.conStr))
            {
                string qry = String.Format("select P.ProductName as ProductCode, POD.Quantity,U.UOMSymbol as UOM, POD.Rate,POD.TaxPer, POD.DisPer from PurchaseReturnDetails as POD left join Products as P on POD.ProductCode = p.ProductId left join UnitsOfMeasurement as U on POD.UOM=U.UOMId where POD.PRId='{0}'", c);
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
                string qry1 = string.Format("select   PO.PRId,s.SupplierName as LedgerCode,PO.PRDate, PO.InvoiceNo,PO.DiscountAmount,PO.Extra,PO.ItemAmount from PurchaseReturn as PO left join Supplier as s on PO.LedgerCode = s.SupplierId where {0}", qry);
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
            qry = String.Format("PO.PRDate>='{0:yyyy-MM-dd}' and PO.PRDate<='{1:yyyy-MM-dd}' and PO.ItemAmount>='{2}' and PO.ItemAmount<='{3}'", fromDate, toDate, billFrom, billTo);
            if (cmbSupplier.Text != "")
            {

                qry = qry + "and S.SupplierName='" + cmbSupplier.Text + "'";

            }
            if (txtInvoiceNo.Text != "")
            {
                qry = qry + "and PO.InvoiceNo='" + txtInvoiceNo.Text + "'";

            }

            return qry;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }
    }
}

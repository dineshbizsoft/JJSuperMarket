using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JJSuperMarket
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmLogin : Window
    {
        frmHome frm = new frmHome();
        bool isShowCloseConform = true;
        
        public frmLogin()
        {
            App.LogWriter("Login_Init01");
            InitializeComponent();
            txtUserId.Focus();
            App.LogWriter("Login_Init02");
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtUserId.Text = "";
            txtPassword.Password = "";
            txtUserId.Focus();
        }



        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            JJSuperMarketEntities db = new JJSuperMarketEntities();

            if (txtUserId.Text == "")
            {
                MessageBox.Show("Enter Username");
            }
            else if (txtPassword.Password == "")
            {

                MessageBox.Show("Enter Password");
            }
            var Un =db.CompanyDetails.FirstOrDefault ();
            if (txtUserId.Text == Un.UserName  && txtPassword.Password == Un.PassWord )
            {
                frm.Show();
                isShowCloseConform = false;    
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid Account");
                btnClear_Click(sender, e);
            }

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isShowCloseConform == true)
            {
                if (MessageBox.Show(this, "Are you sure to exit?", "Exit Confirmation", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    frm.Close();
                    e.Cancel = false;
                }
            }
            
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                this.Close();
        }

    }
}

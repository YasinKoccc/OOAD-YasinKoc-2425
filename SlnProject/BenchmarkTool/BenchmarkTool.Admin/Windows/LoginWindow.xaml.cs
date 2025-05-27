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
using System.Windows.Shapes;
using BenchmarkTool.Library.Services;

namespace BenchmarkTool.Admin.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim().ToLower();
            string password = pwdPassword.Password.Trim();

            // ✅ Fix: Hardcode admin credentials
            if (email == "pieter.s@technova.be" && password == "admin")
            {
                // Grant admin access
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                txtStatus.Text = "Onjuiste admin gegevens.";
            }
        }
    }
}

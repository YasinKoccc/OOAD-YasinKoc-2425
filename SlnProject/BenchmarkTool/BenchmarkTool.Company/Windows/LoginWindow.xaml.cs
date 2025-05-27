using System.Windows;
using BenchmarkTool.Library.Models;
using BenchmarkTool.Library.Services;

namespace BenchmarkTool.Company.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Trim both email and password to avoid hidden whitespace issues
            string email = txtEmail.Text.Trim().ToLower(); // Enforce lowercase for case-insensitive matching
            string password = pwdPassword.Password.Trim(); // Trim password input

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                txtStatus.Text = "Vul e-mail en wachtwoord in.";
                return;
            }

            // Debug output (remove in production)
#if DEBUG
            string hash = CompanyService.HashPassword(password);
            MessageBox.Show(
                $"Email: '{email}'\nHash: '{hash}'", // Never show raw password
                "Login Debug"
            );
#endif

            // Attempt login
            BenchmarkTool.Library.Models.Company bedrijf = CompanyService.Login(email, password);

            if (bedrijf != null)
            {
                new MainWindow(bedrijf).Show();
                this.Close();
            }
            else
            {
                txtStatus.Text = "Ongeldige combinatie van e-mail en wachtwoord.";
                pwdPassword.Clear(); // Clear password field on failure
            }
        }
    }
}
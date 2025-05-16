using System.Windows;
using BenchmarkTool.Library.Models;
using BenchmarkTool.Library.Services;

namespace BenchmarkTool.Company.Windows
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
            string email = txtEmail.Text.Trim();
            string password = pwdPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                txtStatus.Text = "Vul e-mail en/of wachtwoord in.";
                return;
            }

            BenchmarkTool.Library.Models.Company bedrijf = CompanyService.Login(email, password);


            if (bedrijf != null)
            {
                // Login gelukt
                MainWindow mainWindow = new MainWindow(bedrijf);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                txtStatus.Text = "Onjuiste gebruikersnaam en/of wachtwoord.";
            }
        }
    }
}

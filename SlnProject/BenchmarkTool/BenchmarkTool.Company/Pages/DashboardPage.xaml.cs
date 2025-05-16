using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BenchmarkTool.Company.Windows;
using BenchmarkTool.Company.Pages;


namespace BenchmarkTool.Company.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        private BenchmarkTool.Library.Models.Company _company;

        public DashboardPage(BenchmarkTool.Library.Models.Company company)
        {
            InitializeComponent();
            _company = company;

            txtBedrijfsnaam.Text = _company.Name;
            txtEmail.Text = _company.Email;
            txtStatus.Text = _company.Status;

            if (_company.Logo != null && _company.Logo.Length > 0)
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(_company.Logo))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                }
                imgLogo.Source = bitmap;
            }
        }

        private void BtnJaarrapporten_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new YearReportsPage(_company));
        }

        private void BtnProfielWijzigen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hier komt de pagina om profiel te wijzigen.");
            // TODO: Navigeren naar profiel wijzigen pagina
        }

        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            // Terug naar login window
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            // Sluit MainWindow
            Window.GetWindow(this).Close();
        }

        private void BtnBenchmark_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new BenchmarkPage());
        }
    }
}

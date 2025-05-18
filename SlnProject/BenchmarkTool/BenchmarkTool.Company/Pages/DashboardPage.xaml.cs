using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BenchmarkTool.Company.Windows;
using BenchmarkTool.Company.Pages;
using CompanyModel = BenchmarkTool.Library.Models.Company;



namespace BenchmarkTool.Company.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        private CompanyModel _bedrijf;

        public DashboardPage(CompanyModel company)
        {
            InitializeComponent();
            _bedrijf = company;


            txtBedrijfsnaam.Text = _bedrijf.Name;
            txtEmail.Text = _bedrijf.Email;
            txtStatus.Text = _bedrijf.Status;

            if (_bedrijf.Logo != null && _bedrijf.Logo.Length > 0)
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(_bedrijf.Logo))
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
            NavigationService?.Navigate(new YearReportsPage(_bedrijf));
        }

        private void BtnProfielWijzigen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ProfileEditPage(_bedrijf));
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

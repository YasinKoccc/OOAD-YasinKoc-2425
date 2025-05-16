using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using BenchmarkTool.Library.Models;
using BenchmarkTool.Library.Services;

namespace BenchmarkTool.Admin.Pages
{
    public partial class BedrijvenBeherenPage : Page
    {
        public BedrijvenBeherenPage()
        {
            InitializeComponent();
            LaadCompanies();
        }

        public void LaadCompanies()
        {
            var companies = CompanyService.GetAllCompanies();

            BedrijvenPanel.Children.Clear();

            foreach (Company company in companies)
            {
                StackPanel card = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5)
                };

                if (company.Logo != null)
                {
                    BitmapImage img = new BitmapImage();
                    using (var ms = new MemoryStream(company.Logo))
                    {
                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        img.StreamSource = ms;
                        img.EndInit();
                    }

                    card.Children.Add(new Image
                    {
                        Source = img,
                        Width = 50,
                        Height = 50,
                        Margin = new Thickness(0, 0, 10, 0)
                    });
                }

                var tekst = new TextBlock
                {
                    Text = $"{company.Name}\n{company.Email}",
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 200
                };
                card.Children.Add(tekst);

                Button btnWijzigen = new Button
                {
                    Content = "Wijzigen",
                    Tag = company.Id,
                    Margin = new Thickness(5, 0, 5, 0),
                    Width = 75,
                };
                btnWijzigen.Click += BtnWijzigen_Click;
                card.Children.Add(btnWijzigen);

                Button btnVerwijderen = new Button
                {
                    Content = "Verwijderen",
                    Tag = company.Id,
                    Margin = new Thickness(5, 0, 5, 0),
                    Width = 75,
                    Background = Brushes.Red,
                    Foreground = Brushes.White
                };
                btnVerwijderen.Click += BtnVerwijderen_Click;
                card.Children.Add(btnVerwijderen);

                BedrijvenPanel.Children.Add(card);
            }
        }

        private void BtnWijzigen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int companyId)
            {
                NavigationService.Navigate(new BedrijfWijzigenPage(companyId, this));
            }
        }

        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int companyId)
            {
                var result = MessageBox.Show("Weet je zeker dat je dit bedrijf wilt verwijderen?", "Verwijderen", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        CompanyService.DeleteCompany(companyId);
                        LaadCompanies();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Fout bij verwijderen: " + ex.Message);
                    }
                }
            }
        }
    }
}

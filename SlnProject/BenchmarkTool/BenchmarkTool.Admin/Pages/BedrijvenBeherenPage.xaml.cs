using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using BenchmarkTool.Library.Models;
using BenchmarkTool.Library.Services;
using System.Collections.Generic; // Added for List<string> for ComboBox items

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

                // Logo
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

                // Bedrijfsinfo
                var tekst = new TextBlock
                {
                    Text = $"{company.Name}\n{company.Email}\nStatus: {company.Status}",
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 200
                };
                card.Children.Add(tekst);

                // Wijzig knop
                Button btnWijzigen = new Button
                {
                    Content = "Wijzigen",
                    Tag = company.Id,
                    Margin = new Thickness(5, 0, 5, 0),
                    Width = 75,
                };
                btnWijzigen.Click += BtnWijzigen_Click;
                card.Children.Add(btnWijzigen);

                // Verwijder knop
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

                // Goedkeuren / Afkeuren knoppen bij "pending"
                if (company.Status == "pending")
                {
                    // ✅ Goedkeuren knop
                    Button btnGoedkeuren = new Button
                    {
                        Content = "✅",
                        FontSize = 16,
                        Tag = company.Id,
                        Margin = new Thickness(5, 0, 5, 0),
                        Width = 40,
                        Height = 30,
                        ToolTip = "Goedkeuren",
                        Background = Brushes.LightGreen
                    };
                    btnGoedkeuren.Click += BtnGoedkeuren_Click;

                    // ❌ Afkeuren knop
                    Button btnAfkeuren = new Button
                    {
                        Content = "❌",
                        FontSize = 16,
                        Tag = company.Id,
                        Margin = new Thickness(5, 0, 5, 0),
                        Width = 40,
                        Height = 30,
                        ToolTip = "Afkeuren",
                        Background = Brushes.IndianRed,
                        Foreground = Brushes.White
                    };
                    btnAfkeuren.Click += BtnAfkeuren_Click;

                    // Voeg toe aan card
                    card.Children.Add(btnGoedkeuren);
                    card.Children.Add(btnAfkeuren);
                }


                // Add the ComboBox here, next to the "Verwijderen" button
                ComboBox actionComboBox = new ComboBox
                {
                    Tag = company.Id, // Associate the ComboBox with the company ID
                    Width = 120, // Adjust width as needed
                    Margin = new Thickness(5, 0, 0, 0), // Add some margin to separate it
                    VerticalAlignment = VerticalAlignment.Center // Align vertically with other elements
                };

                // Populate the ComboBox with desired options
                actionComboBox.Items.Add("Selecteer Actie"); // Default/placeholder item
                actionComboBox.Items.Add("✅ Approve");
                actionComboBox.Items.Add("⏳ Hold");
                actionComboBox.Items.Add("❌ Reject");

                // You can add more options as needed

                // Set the default selected item
                actionComboBox.SelectedIndex = 0;

                // Attach a SelectionChanged event handler
                actionComboBox.SelectionChanged += ActionComboBox_SelectionChanged;

                card.Children.Add(actionComboBox);

                BedrijvenPanel.Children.Add(card);
            }
        }

        private void BtnWijzigen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int companyId)
            {
                // NavigationService.Navigate(new BedrijfWijzigenPage(companyId, this)); // Re-enable if you have this page
                MessageBox.Show($"Wijzigen button clicked for Company ID: {companyId}");
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

        private void BtnGoedkeuren_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            CompanyService.UpdateStatus(id, "active");
            LaadCompanies();
        }

        private void BtnAfkeuren_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            CompanyService.UpdateStatus(id, "rejected");
            LaadCompanies();
        }

        // New event handler for the ComboBox selection change
        private void ActionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.Tag is int companyId)
            {
                string selectedAction = comboBox.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(selectedAction))
                    return;

                switch (selectedAction)
                {
                    case "✅ Approve":
                        CompanyService.UpdateStatus(companyId, "active");
                        break;
                    case "⏳ Hold":
                        CompanyService.UpdateStatus(companyId, "pending");
                        break;
                    case "❌ Reject":
                        CompanyService.UpdateStatus(companyId, "rejected");
                        break;
                    default:
                        return;
                }

                // Refresh the list to reflect the new status
                LaadCompanies();
            }
        }

    }
}
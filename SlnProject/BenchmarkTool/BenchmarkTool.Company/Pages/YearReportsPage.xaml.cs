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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BenchmarkTool.Library.Models;
using BenchmarkTool.Library.Services;

namespace BenchmarkTool.Company.Pages
{
    /// <summary>
    /// Interaction logic for YearReportsPage.xaml
    /// </summary>
    public partial class YearReportsPage : Page
    {
        private BenchmarkTool.Library.Models.Company _company;

        public YearReportsPage(BenchmarkTool.Library.Models.Company company)
        {
            InitializeComponent();
            _company = company;

            this.Loaded += YearReportsPage_Loaded;
        }

        private void YearReportsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReports();
        }


        public void LoadReports()
        {
            List<YearReport> reports = YearReportService.GetReportsByCompanyId(_company.Id);
            ReportsPanel.Children.Clear();

            foreach (YearReport report in reports)
            {
                // Benchmark: get average FTE for this year  
                double avgFte = YearReportService.GetAverageFteByYear(report.Year);

                // Add a benchmark TextBlock  
                var benchmarkText = new TextBlock
                {
                    Text = $"Gemiddelde FTE in {report.Year}: {avgFte:F2}",
                    FontSize = 12,
                    Foreground = Brushes.DarkBlue,
                    Margin = new Thickness(10, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Info-tekst  
                var infoText = new TextBlock
                {
                    Text = $"Jaar: {report.Year} - FTE: {report.Fte}",
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Bewerken knop  
                Button btnEdit = new Button
                {
                    Content = "Bewerk",
                    Tag = report,
                    Margin = new Thickness(10, 0, 5, 0),
                    Width = 80
                };
                btnEdit.Click += BtnBewerkRapport_Click;

                // Verwijderen knop  
                Button btnDelete = new Button
                {
                    Content = "Verwijder",
                    Tag = report,
                    Margin = new Thickness(5, 0, 0, 0),
                    Width = 80,
                    Background = Brushes.Red,
                    Foreground = Brushes.White
                };
                btnDelete.Click += BtnVerwijderRapport_Click;

                // Horizontale rij maken  
                StackPanel row = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 0)
                };
                row.Children.Add(infoText);
                row.Children.Add(btnEdit);
                row.Children.Add(btnDelete);
                row.Children.Add(benchmarkText); // Moved this line after 'row' declaration  

                // Border rond rij  
                var border = new Border
                {
                    Background = Brushes.LightGray,
                    BorderBrush = Brushes.DarkGray,
                    BorderThickness = new Thickness(1.5),
                    CornerRadius = new CornerRadius(8),
                    Margin = new Thickness(10, 5, 10, 5),
                    Padding = new Thickness(12),
                    Child = row
                };

                ReportsPanel.Children.Add(border); // ✅ Correct naamgebruik  
            }
        }




        private void BtnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new YearReportEditPage(_company));
        }
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            // Navigeer terug naar het dashboard
            this.NavigationService?.Navigate(new DashboardPage(_company));
        }


        private void BtnBewerkRapport_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is YearReport report)
            {
                NavigationService?.Navigate(new YearReportEditPage(_company, report));
            }
        }

        private void BtnVerwijderRapport_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is YearReport report)
            {
                var result = MessageBox.Show("Weet je zeker dat je dit rapport wilt verwijderen?", "Bevestigen", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        YearReportService.DeleteYearReport(report.Id);
                        LoadReports();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fout bij verwijderen: " + ex.Message);
                    }
                }
            }
        }

        private void BtnToonGrafiek_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new BenchmarkChartPage(_company));
        }
    }
}

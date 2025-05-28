using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BenchmarkTool.Library.Models;
using BenchmarkTool.Library.Services;
using System.Linq;
using System.Collections.Generic;

namespace BenchmarkTool.Company.Pages
{
    public partial class BenchmarkChartPage : Page
    {
        private BenchmarkTool.Library.Models.Company _company;
        private List<YearReport> _reports;

        public BenchmarkChartPage(BenchmarkTool.Library.Models.Company company)
        {
            InitializeComponent();
            _company = company;

            // Vul sectoren
            var sectors = CompanyService.GetAllCompanies()
                .Select(c => c.Sector)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .OrderBy(s => s)
                .ToList();
            sectors.Insert(0, "Alle sectoren");
            cmbSector.ItemsSource = sectors;
            cmbSector.SelectedIndex = 0;

            // Vul jaren
            var years = YearReportService.GetAllYearReports()
                .Select(r => r.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();
            years.Insert(0, 0); // 0 = alle jaren
            cmbYear.ItemsSource = years;
            cmbYear.SelectedIndex = 0;

            DrawChart();
        }
        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            DrawChart();
        }


        private void DrawChart()
        {
            string selectedSector = cmbSector.SelectedItem as string;
            if (selectedSector == "Alle sectoren") selectedSector = null;

            int selectedYear = (cmbYear.SelectedItem is int y && y != 0) ? y : (int?)null ?? 0;

            var benchmarkRows = YearReportService.GetFilteredYearReports(selectedSector, selectedYear == 0 ? (int?)null : selectedYear);

            if (benchmarkRows == null || benchmarkRows.Count == 0) return;

            // Only show your own company's reports from the filtered list
            var myReports = benchmarkRows.Where(br => br.Company.Id == _company.Id).Select(br => br.Report).ToList();
            if (myReports.Count == 0) return;

            double maxFte = myReports.Max(r => (double)r.Fte);
            double maxAvg = myReports.Max(r => YearReportService.GetAverageFteByYear(r.Year));
            double max = System.Math.Max(maxFte, maxAvg);

            double barWidth = 60;
            double spacing = 40;
            double left = 40;
            double baseLine = 220;

            ChartCanvas.Children.Clear();

            int i = 0;
            foreach (var report in myReports)
            {
                double avg = YearReportService.GetAverageFteByYear(report.Year);

                // Your company FTE bar
                var rect = new Rectangle
                {
                    Width = barWidth,
                    Height = (max > 0) ? (report.Fte / max) * 150 : 0,
                    Fill = Brushes.SteelBlue
                };
                Canvas.SetLeft(rect, left + i * (barWidth + spacing));
                Canvas.SetTop(rect, baseLine - rect.Height);
                ChartCanvas.Children.Add(rect);

                // Average FTE bar
                var rectAvg = new Rectangle
                {
                    Width = barWidth,
                    Height = (max > 0) ? (avg / max) * 150 : 0,
                    Fill = Brushes.Orange
                };
                Canvas.SetLeft(rectAvg, left + i * (barWidth + spacing) + barWidth + 5);
                Canvas.SetTop(rectAvg, baseLine - rectAvg.Height);
                ChartCanvas.Children.Add(rectAvg);

                // Year label
                var yearLabel = new TextBlock
                {
                    Text = report.Year.ToString(),
                    Width = barWidth * 2,
                    TextAlignment = TextAlignment.Center
                };
                Canvas.SetLeft(yearLabel, left + i * (barWidth + spacing));
                Canvas.SetTop(yearLabel, baseLine + 5);
                ChartCanvas.Children.Add(yearLabel);

                i++;
            }

            // Legend
            var legendYour = new TextBlock
            {
                Text = _company.Name, // <-- Gebruik de bedrijfsnaam
                Foreground = Brushes.SteelBlue,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(legendYour, 10);
            Canvas.SetTop(legendYour, 10);
            ChartCanvas.Children.Add(legendYour);

            var legendAvg = new TextBlock
            {
                Text = "Gemiddelde",
                Foreground = Brushes.Orange,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(legendAvg, 10);
            Canvas.SetTop(legendAvg, 30);
            ChartCanvas.Children.Add(legendAvg);

        }


        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new YearReportsPage(_company));
        }

    }
}

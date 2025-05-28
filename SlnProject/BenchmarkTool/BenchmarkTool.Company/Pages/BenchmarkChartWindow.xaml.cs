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
            _reports = YearReportService.GetReportsByCompanyId(_company.Id);
            DrawChart();
        }

        private void DrawChart()
        {
            if (_reports.Count == 0) return;

            double maxFte = _reports.Max(r => (double)r.Fte);
            double maxAvg = _reports.Max(r => YearReportService.GetAverageFteByYear(r.Year));
            double max = System.Math.Max(maxFte, maxAvg);

            double barWidth = 60;
            double spacing = 40;
            double left = 40;
            double baseLine = 220;

            ChartCanvas.Children.Clear();

            int i = 0;
            foreach (var report in _reports)
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
                Text = "Jouw bedrijf",
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

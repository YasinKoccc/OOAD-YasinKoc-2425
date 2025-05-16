using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using BenchmarkTool.Library.Models;
using BenchmarkTool.Library.Services;

namespace BenchmarkTool.Company.Pages
{
    /// <summary>
    /// Interaction logic for BenchmarkPage.xaml
    /// </summary>
    public partial class BenchmarkPage : Page
    {
        private List<(YearReport Report, BenchmarkTool.Library.Models.Company Company)> _allData;

        public BenchmarkPage()
        {
            InitializeComponent();
            LoadFilters();
            LoadData();
        }

        private void LoadFilters()
        {
            var sectors = CompanyService.GetAllCompanies()
                                        .Select(c => c.Sector)
                                        .Where(s => !string.IsNullOrEmpty(s))
                                        .Distinct()
                                        .OrderBy(s => s)
                                        .ToList();
            sectors.Insert(0, "Alle");
            cmbSector.ItemsSource = sectors;
            cmbSector.SelectedIndex = 0;

            var years = GetAllYears();
            years.Insert(0, 0); // 0 betekent "Alle"
            cmbYear.ItemsSource = years;
            cmbYear.SelectedIndex = 0;
        }

        private void LoadData()
        {
            string sectorFilter = cmbSector.SelectedItem as string;
            if (sectorFilter == "Alle")
                sectorFilter = null;

            int? yearFilter = null;
            if (cmbYear.SelectedItem is int y && y != 0)
                yearFilter = y;

            _allData = GetFilteredYearReports(sectorFilter, yearFilter);
            lvBenchmark.ItemsSource = _allData;
        }

        private void Filters_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        // Helper method to get all years from YearReportService
        private List<int> GetAllYears()
        {
            return YearReportService.GetAllYearReports()
                .Select(r => r.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();
        }

        // Helper method to get filtered year reports
        private List<(YearReport Report, BenchmarkTool.Library.Models.Company Company)> GetFilteredYearReports(string sector, int? year)
        {
            var companies = CompanyService.GetAllCompanies();
            var reports = YearReportService.GetAllYearReports();

            var query = from report in reports
                        join company in companies on report.CompanyId equals company.Id
                        where (sector == null || company.Sector == sector)
                              && (!year.HasValue || report.Year == year.Value)
                        select (Report: report, Company: company);

            return query.ToList();
        }
    }

}

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
            lvReports.ItemsSource = reports;
        }

        private void BtnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new YearReportEditPage(_company.Id));
        }

        private void BtnBewerken_Click(object sender, RoutedEventArgs e)
        {
            if (lvReports.SelectedItem is YearReport selectedReport)
            {
                NavigationService?.Navigate(new YearReportEditPage(_company.Id, selectedReport));
            }
            else
            {
                MessageBox.Show("Selecteer een rapport om te bewerken.");
            }
        }

        private void BtnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            if (lvReports.SelectedItem is YearReport selectedReport)
            {
                var result = MessageBox.Show("Weet je zeker dat je dit rapport wilt verwijderen?", "Bevestigen", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        YearReportService.DeleteYearReport(selectedReport.Id);
                        LoadReports();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fout bij verwijderen: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecteer een rapport om te verwijderen.");
            }
        }

    }
}

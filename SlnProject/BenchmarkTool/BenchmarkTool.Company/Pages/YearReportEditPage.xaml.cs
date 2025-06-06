﻿using System;
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
    /// Interaction logic for YearReportEditPage.xaml
    /// </summary>
    public partial class YearReportEditPage : Page
    {
        private YearReport _yearReport;
        private int _companyId;
        private BenchmarkTool.Library.Models.Company _company;

        public YearReportEditPage(BenchmarkTool.Library.Models.Company company, YearReport yearReport = null)
        {
            InitializeComponent();
            _company = company;
            _yearReport = yearReport;

            if (_yearReport != null)
            {
                txtYear.Text = _yearReport.Year.ToString();
                txtFte.Text = _yearReport.Fte.ToString();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtYear.Text, out int year))
            {
                MessageBox.Show("Voer een geldig jaartal in.");
                return;
            }
            if (!int.TryParse(txtFte.Text, out int fte))
            {
                MessageBox.Show("Voer een geldig FTE in.");
                return;
            }

            if (_yearReport == null)
                _yearReport = new YearReport();

            _yearReport.Year = year;
            _yearReport.Fte = fte;
            _yearReport.CompanyId = _companyId;

            try
            {
                if (_yearReport.Id == 0)
                    YearReportService.AddYearReport(_yearReport);
                else
                    YearReportService.UpdateYearReport(_yearReport);

                MessageBox.Show("Jaarrapport succesvol opgeslagen.");

                // Navigeer handmatig terug naar de vorige pagina
                var parentWindow = Window.GetWindow(this) as MainWindow;
                if (parentWindow != null)
                {
                    parentWindow.MainFrame.Navigate(new YearReportsPage(new BenchmarkTool.Library.Models.Company { Id = _companyId }));

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij opslaan: " + ex.Message);
            }
        }
        private void BtnTerug_Click(object sender, RoutedEventArgs e)
        {
            // After save or on back button:
            NavigationService?.Navigate(new YearReportsPage(_company));

        }

    }
}

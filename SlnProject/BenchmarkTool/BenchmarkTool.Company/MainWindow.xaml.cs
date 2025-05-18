using System.Windows;
using BenchmarkTool.Company.Pages;
using System.IO;
using System.Windows.Media.Imaging;
using BenchmarkTool.Library.Models;
using CompanyModel = BenchmarkTool.Library.Models.Company;



namespace BenchmarkTool.Company
{
    /// <summary>  
    /// Interaction logic for MainWindow.xaml  
    /// </summary>  
    public partial class MainWindow : Window
    {
        private BenchmarkTool.Library.Models.Company _bedrijf;

        public MainWindow(BenchmarkTool.Library.Models.Company bedrijf)
        {
            InitializeComponent();
            _bedrijf = bedrijf;

            // Direct dashboard tonen
            MainFrame.Navigate(new DashboardPage(_bedrijf));
        }
    }
}

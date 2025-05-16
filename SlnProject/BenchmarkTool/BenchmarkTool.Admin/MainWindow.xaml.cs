using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using BenchmarkTool.Admin.Pages;


namespace BenchmarkTool.Admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenBedrijvenPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BedrijvenBeherenPage());
        }

        private void OpenBedrijfToevoegen_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.BedrijfToevoegenPage());
        }
    }
}

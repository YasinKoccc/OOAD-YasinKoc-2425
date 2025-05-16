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
using BenchmarkTool.Library.Services;

namespace BenchmarkTool.Company.Pages
{
    /// <summary>
    /// Interaction logic for ProfileEditPage.xaml
    /// </summary>
    public partial class ProfileEditPage : Page
    {
        private BenchmarkTool.Library.Models.Company _company;

        public ProfileEditPage(BenchmarkTool.Library.Models.Company company)
        {
            InitializeComponent();
            _company = company;

            txtName.Text = _company.Name;
            txtContact.Text = _company.Contact;
            txtEmail.Text = _company.Email;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _company.Name = txtName.Text.Trim();
            _company.Contact = txtContact.Text.Trim();
            _company.Email = txtEmail.Text.Trim();

            try
            {
                CompanyService.UpdateCompany(_company);
                MessageBox.Show("Profiel succesvol bijgewerkt.");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Fout bij opslaan: " + ex.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BenchmarkTool.Library.Models;
using BenchmarkTool.Library.Services;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;


namespace BenchmarkTool.Admin.Pages
{
    public partial class BedrijfWijzigenPage : Page
    {
        private int _companyId;
        private BedrijvenBeherenPage _parentPage;
        private Company _company;

        public BedrijfWijzigenPage(int companyId, BedrijvenBeherenPage parentPage)
        {
            InitializeComponent();
            _companyId = companyId;
            _parentPage = parentPage;

            LoadNacecodes();
            LoadCompanyData();
        }

        private void LoadNacecodes()
        {
            List<string> codes = CompanyService.GetNacecodes();
            cmbNacecode.ItemsSource = codes;
        }

        private void LoadCompanyData()
        {
            _company = CompanyService.GetCompanyById(_companyId);
            if (_company != null)
            {
                txtName.Text = _company.Name;
                txtContact.Text = _company.Contact;
                txtEmail.Text = _company.Email;
                txtLogin.Text = _company.Login;
                cmbNacecode.SelectedItem = _company.Nacecode_Code;
            }
        }

        private void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtContact.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtLogin.Text) ||
                cmbNacecode.SelectedItem == null)
            {
                MessageBox.Show("Vul alle velden in.");
                return;
            }

            _company.Name = txtName.Text.Trim();
            _company.Contact = txtContact.Text.Trim();
            _company.Email = txtEmail.Text.Trim();
            _company.Login = txtLogin.Text.Trim();
            _company.Nacecode_Code = cmbNacecode.SelectedItem.ToString();

            if (!string.IsNullOrWhiteSpace(pwdPassword.Password))
            {
                _company.Password = HashPassword(pwdPassword.Password);
            }

            try
            {
                CompanyService.UpdateCompany(_company);
                _parentPage.LaadCompanies();
                this.NavigationService.GoBack();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij opslaan: " + ex.Message);
            }
        }

        private void KiesLogo_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Afbeeldingen|*.png;*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                byte[] imageBytes = File.ReadAllBytes(dialog.FileName);
                _company.Logo = imageBytes;

                // Preview tonen
                BitmapImage img = new BitmapImage();
                using (var ms = new MemoryStream(imageBytes))
                {
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.StreamSource = ms;
                    img.EndInit();
                }
                imgLogoPreview.Source = img;
            }
        }

        private void Annuleren_Click(object sender, RoutedEventArgs e)
        {
            _parentPage.NavigationService.GoBack();
        }

        private string HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var sb = new System.Text.StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}

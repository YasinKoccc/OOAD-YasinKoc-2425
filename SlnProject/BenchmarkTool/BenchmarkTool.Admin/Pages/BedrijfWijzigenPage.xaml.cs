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
            cmbNacecode.Items.Clear();
            foreach (var code in codes)
            {
                cmbNacecode.Items.Add(code);
            }

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
                // Ga handmatig terug naar de vorige pagina zonder GoBack
                Window parentWindow = Window.GetWindow(this);
                if (parentWindow != null)
                {
                    var frame = parentWindow.FindName("MainFrame") as Frame;
                    if (frame != null)
                    {
                        frame.Content = _parentPage; // dit is BedrijvenBeherenPage
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij opslaan: " + ex.Message);
            }
        }

        private void KiesLogo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            dialog.Filter = "Afbeeldingen|*.png;*.jpg;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                string chosenFileName = dialog.FileName;

                imgLogoPreview.Source = new BitmapImage(new Uri(chosenFileName));
            }
        }



        private void Annuleren_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as BenchmarkTool.Admin.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(new BedrijvenBeherenPage());
            }
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BenchmarkTool.Library.Services;
using CompanyModel = BenchmarkTool.Library.Models.Company;

namespace BenchmarkTool.Company.Pages
{
    /// <summary>
    /// Interaction logic for ProfileEditPage.xaml
    /// </summary>
    public partial class ProfileEditPage : Page
    {
        private CompanyModel _bedrijf;

        public ProfileEditPage(CompanyModel bedrijf)
        {
            InitializeComponent();
            _bedrijf = bedrijf;
            LaadGegevens();
        }

        private void LaadGegevens()
        {
            var nacecodes = CompanyService.GetNacecodes();
            cmbNacecode.ItemsSource = nacecodes;
            cmbNacecode.SelectedItem = _bedrijf.Nacecode_Code;

            txtNaam.Text = _bedrijf.Name;
            txtContact.Text = _bedrijf.Contact;
            txtEmail.Text = _bedrijf.Email;
            txtLogin.Text = _bedrijf.Login;
            txtSector.Text = _bedrijf.Sector;
        }

        private void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            txtStatus.Text = "";

            if (string.IsNullOrWhiteSpace(txtNaam.Text) ||
                string.IsNullOrWhiteSpace(txtContact.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                txtStatus.Text = "Vul alle verplichte velden in.";
                return;
            }

            // Als een nieuw wachtwoord is ingevuld, controleer het oude wachtwoord
            if (!string.IsNullOrWhiteSpace(pwdNieuw.Password))
            {
                if (string.IsNullOrWhiteSpace(pwdOud.Password))
                {
                    txtStatus.Text = "Geef het huidige wachtwoord in.";
                    return;
                }

                string ingegevenHash = CompanyService.HashPassword(pwdOud.Password);

                if (_bedrijf.Password != ingegevenHash)
                {
                    txtStatus.Text = "Huidig wachtwoord is onjuist.";
                    return;
                }

                _bedrijf.Password = CompanyService.HashPassword(pwdNieuw.Password); // Nieuw wachtwoord opslaan
            }

            // Update overige gegevens
            _bedrijf.Name = txtNaam.Text.Trim();
            _bedrijf.Contact = txtContact.Text.Trim();
            _bedrijf.Email = txtEmail.Text.Trim();
            _bedrijf.Login = txtLogin.Text.Trim();
            _bedrijf.LastModified = DateTime.Now;

            try
            {
                _bedrijf.Address = _bedrijf.Address ?? "";
                _bedrijf.Zip = _bedrijf.Zip ?? "";
                _bedrijf.City = _bedrijf.City ?? "";
                _bedrijf.Country = _bedrijf.Country ?? "";
                _bedrijf.Phone = _bedrijf.Phone ?? "";
                _bedrijf.Btw = _bedrijf.Btw ?? "";
                _bedrijf.Status = _bedrijf.Status ?? "Actief"; // of iets anders
                _bedrijf.Language = _bedrijf.Language ?? "NL"; // of "EN", "FR"
                _bedrijf.Nacecode_Code = _bedrijf.Nacecode_Code ?? "99999";
                CompanyService.UpdateCompany(_bedrijf);
                txtStatus.Foreground = System.Windows.Media.Brushes.Green;
                txtStatus.Text = "Profiel succesvol bijgewerkt.";
            }
            catch (Exception ex)
            {
                txtStatus.Foreground = System.Windows.Media.Brushes.Red;
                txtStatus.Text = "Fout bij opslaan: " + ex.Message;
            }
        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            // Navigeer terug naar DashboardPage en geef het bedrijf mee
            this.NavigationService?.Navigate(new DashboardPage(_bedrijf));
        }
    }
}

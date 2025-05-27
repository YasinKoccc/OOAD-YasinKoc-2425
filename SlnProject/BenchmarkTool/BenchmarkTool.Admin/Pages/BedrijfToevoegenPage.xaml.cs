using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BenchmarkTool.Library.Models;
using BenchmarkTool.Library.Services;

namespace BenchmarkTool.Admin.Pages
{
    /// <summary>
    /// Interaction logic for BedrijfToevoegenPage.xaml
    /// </summary>
    public partial class BedrijfToevoegenPage : Page
    {
        public BedrijfToevoegenPage()
        {
            InitializeComponent();
        }

        private void Toevoegen_Click(object sender, RoutedEventArgs e)
        {
            // Basisvalidatie: alle velden moeten ingevuld zijn
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtContact.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(pwdPassword.Password))
            {
                txtStatus.Foreground = Brushes.Red;
                txtStatus.Text = "Vul alle velden in.";
                return;
            }

            // Hash het wachtwoord
            string hashedPassword = CompanyService.HashPassword(pwdPassword.Password);


            Company nieuwBedrijf = new Company
            {
                Name = txtName.Text.Trim(),
                Contact = txtContact.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Login = txtLogin.Text.Trim(),
                Password = hashedPassword,
                RegDate = DateTime.Now,
                Status = "Nieuw",
                Nacecode_Code = "99999" // ✅ <-- HIER
            };



            try
            {
                // Voeg toe via service
                CompanyService.AddCompany(nieuwBedrijf);

                // Bevestiging tonen
                txtStatus.Foreground = Brushes.Green;
                txtStatus.Text = "Bedrijf succesvol toegevoegd.";

                // Formulier leegmaken
                ClearForm();
            }
            catch (Exception ex)
            {
                // Foutmelding tonen
                txtStatus.Foreground = Brushes.Red;
                txtStatus.Text = "Fout bij toevoegen: " + ex.Message;
            }
        }

        private void ClearForm()
        {
            txtName.Text = "";
            txtContact.Text = "";
            txtEmail.Text = "";
            pwdPassword.Password = "";
            txtLogin.Text = "";
        }
    }
}

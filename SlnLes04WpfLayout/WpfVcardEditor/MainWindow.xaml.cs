using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace WpfVcardEditor
{
    public partial class MainWindow : Window
    {
        private bool isDirty = false;
        private string currentFilePath = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.Owner = this;
            about.ShowDialog();
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Ben je zeker dat je de applicatie wil afsluiten?",
                "Toepassing sluiten",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                DefaultExt = ".vcf",
                Filter = "vCard Files (*.vcf)|*.vcf"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    string[] lines = File.ReadAllLines(dlg.FileName);

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("N", StringComparison.OrdinalIgnoreCase) && line.Contains(":"))
                        {
                            int colonIndex = line.IndexOf(':');
                            if (colonIndex > -1)
                            {
                                string nameData = line.Substring(colonIndex + 1);
                                string[] parts = nameData.Split(';');
                                if (parts.Length > 0)
                                    txtLastname.Text = parts[0].Trim();
                                if (parts.Length > 1)
                                    txtFirstname.Text = parts[1].Trim();
                            }
                        }
                        else if (line.StartsWith("FN:", StringComparison.OrdinalIgnoreCase))
                        {
                            string fullName = line.Substring(3).Trim();
                            string[] parts = fullName.Split(' ');
                            if (parts.Length > 0)
                                txtFirstname.Text = parts[0];
                            if (parts.Length > 1)
                                txtLastname.Text = string.Join(" ", parts, 1, parts.Length - 1);
                        }
                        else if (line.StartsWith("BDAY:", StringComparison.OrdinalIgnoreCase))
                        {
                            string bdayStr = line.Substring(5).Trim();
                            DateTime bday;
                            if (DateTime.TryParseExact(bdayStr, "yyyyMMdd", null,
                                    System.Globalization.DateTimeStyles.None, out bday))
                            {
                                datBirthday.SelectedDate = bday;
                            }
                        }
                        else if (line.StartsWith("GENDER:", StringComparison.OrdinalIgnoreCase))
                        {
                            string gender = line.Substring(7).Trim().ToUpper();
                            if (gender == "M")
                                rbMale.IsChecked = true;
                            else if (gender == "F")
                                rbFemale.IsChecked = true;
                            else
                                rbUnknown.IsChecked = true;
                        }
                        else if (line.StartsWith("EMAIL", StringComparison.OrdinalIgnoreCase) && line.Contains("HOME"))
                        {
                            int index = line.IndexOf(':');
                            if (index > -1 && index < line.Length - 1)
                                txtPrivateEmail.Text = line.Substring(index + 1).Trim();
                        }
                        else if (line.StartsWith("TEL", StringComparison.OrdinalIgnoreCase) && line.Contains("HOME"))
                        {
                            int index = line.IndexOf(':');
                            if (index > -1 && index < line.Length - 1)
                                txtPrivatePhone.Text = line.Substring(index + 1).Trim();
                        }
                        else if (line.StartsWith("EMAIL", StringComparison.OrdinalIgnoreCase) &&
                                 line.IndexOf("WORK", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            int colonIndex = line.IndexOf(':');
                            if (colonIndex > -1 && colonIndex < line.Length - 1)
                            {
                                txtWorkEmail.Text = line.Substring(colonIndex + 1).Trim();
                            }
                        }
                        else if (line.StartsWith("TEL", StringComparison.OrdinalIgnoreCase) &&
                                 line.IndexOf("WORK", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            int colonIndex = line.IndexOf(':');
                            if (colonIndex > -1 && colonIndex < line.Length - 1)
                            {
                                txtWorkPhone.Text = line.Substring(colonIndex + 1).Trim();
                            }
                        }
                        else if (line.StartsWith("TITLE", StringComparison.OrdinalIgnoreCase))
                        {
                            int colonIndex = line.IndexOf(':');
                            if (colonIndex > -1 && colonIndex < line.Length - 1)
                            {
                                txtJobTitle.Text = line.Substring(colonIndex + 1).Trim();
                            }
                        }
                        else if (line.StartsWith("ORG", StringComparison.OrdinalIgnoreCase))
                        {
                            int colonIndex = line.IndexOf(':');
                            if (colonIndex > -1 && colonIndex < line.Length - 1)
                            {
                                txtCompany.Text = line.Substring(colonIndex + 1).Trim();
                            }
                        }
                        else if (line.StartsWith("X-SOCIALPROFILE;TYPE=linkedin:", StringComparison.OrdinalIgnoreCase))
                        {
                            txtLinkedIn.Text = line.Substring("X-SOCIALPROFILE;TYPE=linkedin:".Length).Trim();
                        }
                        else if (line.StartsWith("X-SOCIALPROFILE;TYPE=facebook:", StringComparison.OrdinalIgnoreCase))
                        {
                            txtFacebook.Text = line.Substring("X-SOCIALPROFILE;TYPE=facebook:".Length).Trim();
                        }
                        else if (line.StartsWith("X-SOCIALPROFILE;TYPE=instagram:", StringComparison.OrdinalIgnoreCase))
                        {
                            txtInstagram.Text = line.Substring("X-SOCIALPROFILE;TYPE=instagram:".Length).Trim();
                        }
                        else if (line.StartsWith("X-SOCIALPROFILE;TYPE=youtube:", StringComparison.OrdinalIgnoreCase))
                        {
                            txtYouTube.Text = line.Substring("X-SOCIALPROFILE;TYPE=youtube:".Length).Trim();
                        }
                        else if (line.StartsWith("PHOTO:", StringComparison.OrdinalIgnoreCase))
                        {
                            ProcessPhotoLine(line);
                        }
                    }

                    currentFilePath = dlg.FileName;
                    menuSave.IsEnabled = true;
                    UpdateStatusBar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fout bij het inlezen van de vCard: " + ex.Message,
                        "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void menuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                DefaultExt = ".vcf",
                Filter = "vCard Files (*.vcf)|*.vcf"
            };

            if (dlg.ShowDialog() == true)
            {
                currentFilePath = dlg.FileName;
                SaveVCard(currentFilePath);
            }
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                SaveVCard(currentFilePath);
                MessageBox.Show("vCard is opgeslagen.", "Bevestiging", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveVCard(string filePath)
        {
            List<string> lines = new List<string>();
            lines.Add("BEGIN:VCARD");
            lines.Add("VERSION:3.0");

            if (!string.IsNullOrWhiteSpace(txtFirstname.Text) || !string.IsNullOrWhiteSpace(txtLastname.Text))
            {
                string fullName = (txtFirstname.Text.Trim() + " " + txtLastname.Text.Trim()).Trim();
                lines.Add("FN:" + fullName);
            }
            if (datBirthday.SelectedDate.HasValue)
            {
                lines.Add("BDAY:" + datBirthday.SelectedDate.Value.ToString("yyyyMMdd"));
            }
            if (rbMale.IsChecked == true)
                lines.Add("GENDER:M");
            else if (rbFemale.IsChecked == true)
                lines.Add("GENDER:F");
            else if (rbUnknown.IsChecked == true)
                lines.Add("GENDER:O");

            if (!string.IsNullOrWhiteSpace(txtPrivateEmail.Text))
                lines.Add("EMAIL;TYPE=HOME:" + txtPrivateEmail.Text.Trim());
            if (!string.IsNullOrWhiteSpace(txtPrivatePhone.Text))
                lines.Add("TEL;TYPE=HOME:" + txtPrivatePhone.Text.Trim());

            if (!string.IsNullOrWhiteSpace(txtCompany.Text))
                lines.Add("ORG:" + txtCompany.Text.Trim());
            if (!string.IsNullOrWhiteSpace(txtJobTitle.Text))
                lines.Add("TITLE:" + txtJobTitle.Text.Trim());
            if (!string.IsNullOrWhiteSpace(txtWorkEmail.Text))
                lines.Add("EMAIL;TYPE=WORK:" + txtWorkEmail.Text.Trim());
            if (!string.IsNullOrWhiteSpace(txtWorkPhone.Text))
                lines.Add("TEL;TYPE=WORK:" + txtWorkPhone.Text.Trim());

            if (!string.IsNullOrWhiteSpace(txtLinkedIn.Text))
                lines.Add("X-SOCIALPROFILE;TYPE=linkedin:" + txtLinkedIn.Text.Trim());
            if (!string.IsNullOrWhiteSpace(txtFacebook.Text))
                lines.Add("X-SOCIALPROFILE;TYPE=facebook:" + txtFacebook.Text.Trim());
            if (!string.IsNullOrWhiteSpace(txtInstagram.Text))
                lines.Add("X-SOCIALPROFILE;TYPE=instagram:" + txtInstagram.Text.Trim());
            if (!string.IsNullOrWhiteSpace(txtYouTube.Text))
                lines.Add("X-SOCIALPROFILE;TYPE=youtube:" + txtYouTube.Text.Trim());

            if (imgPhoto != null && imgPhoto.Source != null)
            {
                string base64Photo = GetImageBase64(imgPhoto.Source);
                if (!string.IsNullOrWhiteSpace(base64Photo))
                    lines.Add("PHOTO:" + base64Photo);
            }

            lines.Add("END:VCARD");
            try
            {
                File.WriteAllLines(filePath, lines);
                menuSave.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fout bij het opslaan van de vCard: " + ex.Message,
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Card_Changed(object sender, EventArgs e)
        {
            isDirty = true;
        }

        private void menuNew_Click(object sender, RoutedEventArgs e)
        {
            if (isDirty)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Er zijn onopgeslagen wijzigingen. Weet u zeker dat u een nieuwe kaart wilt starten?",
                    "Bevestiging",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            try
            {
                ClearForm();
                currentFilePath = null;
                menuSave.IsEnabled = false;
                isDirty = false;
                UpdateStatusBar();
            }
            catch (IOException ioEx)
            {
                MessageBox.Show("I/O fout tijdens het legen van het formulier: " + ioEx.Message,
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (UnauthorizedAccessException uaEx)
            {
                MessageBox.Show("Toegang geweigerd: " + uaEx.Message,
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Onverwachte fout: " + ex.Message,
                    "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            txtFirstname.Text = "";
            txtLastname.Text = "";
            datBirthday.SelectedDate = null;
            rbMale.IsChecked = false;
            rbFemale.IsChecked = false;
            rbUnknown.IsChecked = false;
            txtPrivateEmail.Text = "";
            txtPrivatePhone.Text = "";

            if (imgPhoto != null)
                imgPhoto.Source = null;

            txtCompany.Text = "";
            txtJobTitle.Text = "";
            txtWorkEmail.Text = "";
            txtWorkPhone.Text = "";
            txtLinkedIn.Text = "";
            txtFacebook.Text = "";
            txtInstagram.Text = "";
            txtYouTube.Text = "";
        }

        private void UpdateStatusBar()
        {
            string fileName = string.IsNullOrEmpty(currentFilePath) ? "(geen geopend)" : System.IO.Path.GetFileName(currentFilePath);
            statusCard.Content = "huidige kaart: " + fileName;
        }

        private void ProcessPhotoLine(string line)
        {
            string base64Data = line.Substring("PHOTO:".Length).Trim();
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64Data);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    imgPhoto.Source = bitmap;
                }
            }
            catch (FormatException fEx)
            {
                MessageBox.Show("Ongeldige base64-gegevens in de foto-regel:\n" + fEx.Message,
                    "Foto inlezen Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Onverwachte fout bij het laden van de foto:\n" + ex.Message,
                    "Foto inlezen Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                Filter = "JPEG Files (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG Files (*.png)|*.png|All Files (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                txtSelectedPhoto.Content = System.IO.Path.GetFileName(dlg.FileName);
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(dlg.FileName, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    imgPhoto.Source = bitmap;
                    isDirty = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fout bij het laden van de foto: " + ex.Message,
                        "Foto laden Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string GetImageBase64(System.Windows.Media.ImageSource imageSource)
        {
            if (imageSource is BitmapImage bitmapImage)
            {
                try
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fout bij het converteren van de foto naar base64:\n" + ex.Message,
                        "Foto wegschrijven Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return string.Empty;
        }
    }
}

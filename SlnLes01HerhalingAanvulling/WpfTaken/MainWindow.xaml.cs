using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfTaken
{
    public partial class MainWindow : Window
    {
        private Stack<ListBoxItem> verwijderdeTaken = new Stack<ListBoxItem>();

        public MainWindow()
        {
            InitializeComponent();
            InitializePrioriteiten();
            btnToevoegen.Click += btnToevoegen_Click;
            btnVerwijderen.Click += btnVerwijderen_Click;
            btnTerugzetten.Click += btnTerugzetten_Click;
            txttaak.TextChanged += txttaak_TextChanged;
            cmbPrioriteit.SelectionChanged += cmbPrioriteit_SelectionChanged;
            DateDeadline.SelectedDateChanged += DateDeadline_SelectedDateChanged;
            btnVerwijderen.IsEnabled = false;
            btnTerugzetten.IsEnabled = false;
        }

        private void InitializePrioriteiten()
        {
            cmbPrioriteit.Items.Add(new ComboBoxItem { Content = "Selecteer...", Tag = "lightBlue" });
            cmbPrioriteit.Items.Add(new ComboBoxItem { Content = "Laag", Tag = "LightGreen" });
            cmbPrioriteit.Items.Add(new ComboBoxItem { Content = "Normaal", Tag = "LightYellow" });
            cmbPrioriteit.Items.Add(new ComboBoxItem { Content = "Hoog", Tag = "LightCoral" });

            cmbPrioriteit.SelectedIndex = 0;
        }

        private void btnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForm())
            {
                string taak = $"{txttaak.Text} (Deadline: {DateDeadline.SelectedDate.Value.ToShortDateString()}, Door: {GetGeselecteerdePersoon()})";

                ListBoxItem item = new ListBoxItem();
                item.Content = taak;

                if (cmbPrioriteit.SelectedItem is ComboBoxItem selectedPrioriteit)
                {
                    item.Background = (selectedPrioriteit.Tag.ToString()) switch
                    {
                        "LightGreen" => System.Windows.Media.Brushes.LightGreen,
                        "LightYellow" => System.Windows.Media.Brushes.LightYellow,
                        "LightCoral" => System.Windows.Media.Brushes.LightCoral,
                        _ => System.Windows.Media.Brushes.LightBlue
                    };
                }
                else
                {
                    item.Background = System.Windows.Media.Brushes.LightBlue;
                }

                lstTaken.Items.Add(item);

                ClearForm();
                lblerror.Content = "";

                btnVerwijderen.IsEnabled = true;
            }
        }

        private void btnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            if (lstTaken.SelectedItem != null)
            {
                verwijderdeTaken.Push((ListBoxItem)lstTaken.SelectedItem);
                lstTaken.Items.Remove(lstTaken.SelectedItem);

                btnTerugzetten.IsEnabled = true;

                btnVerwijderen.IsEnabled = lstTaken.Items.Count > 0;
            }
        }

        private void btnTerugzetten_Click(object sender, RoutedEventArgs e)
        {
            if (verwijderdeTaken.Count > 0)
            {
                lstTaken.Items.Add(verwijderdeTaken.Pop());

                btnTerugzetten.IsEnabled = verwijderdeTaken.Count > 0;

                btnVerwijderen.IsEnabled = true;
            }
        }

        private bool CheckForm()
        {
            if (string.IsNullOrWhiteSpace(txttaak.Text))
            {
                lblerror.Content = "Taak beschrijving is vereist.";
                return false;
            }

            if (cmbPrioriteit.SelectedItem == null)
            {
                lblerror.Content = "Selecteer een prioriteit.";
                return false;
            }

            if (DateDeadline.SelectedDate == null)
            {
                lblerror.Content = "Selecteer een deadline.";
                return false;
            }

            if (!rdbAdam.IsChecked.Value && !rdbBilal.IsChecked.Value && !rdbChelsey.IsChecked.Value)
            {
                lblerror.Content = "Selecteer een persoon.";
                return false;
            }

            lblerror.Content = "";
            return true;
        }

        private void ClearForm()
        {
            txttaak.Clear();
            cmbPrioriteit.SelectedIndex = -1;
            DateDeadline.SelectedDate = null;
            rdbAdam.IsChecked = false;
            rdbBilal.IsChecked = false;
            rdbChelsey.IsChecked = false;
        }

        private string GetGeselecteerdePersoon()
        {
            if (rdbAdam.IsChecked.Value) return "Adam";
            if (rdbBilal.IsChecked.Value) return "Bilal";
            if (rdbChelsey.IsChecked.Value) return "Chelsey";
            return "";
        }

        private void txttaak_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (btnToevoegen.IsEnabled)
            {
                CheckForm();
            }
        }

        private void cmbPrioriteit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (btnToevoegen.IsEnabled)
            {
                CheckForm();
            }
        }

        private void DateDeadline_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (btnToevoegen.IsEnabled)
            {
                CheckForm();
            }
        }
    }
}
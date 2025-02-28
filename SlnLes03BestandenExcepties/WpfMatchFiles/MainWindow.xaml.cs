using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace WpfMatchFiles
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

        private void btnKiesBestand1_Click(object sender, RoutedEventArgs e)
        {
            string bestandPad = OpenBestand();
            if (!string.IsNullOrEmpty(bestandPad))
            {
                txtBestand1.Text = bestandPad;
            }
        }

        private void btnKiesBestand2_Click(object sender, RoutedEventArgs e)
        {
            string bestandPad = OpenBestand();
            if (!string.IsNullOrEmpty(bestandPad))
            {
                txtBestand2.Text = bestandPad;
            }
        }

        private void btnVergelijk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBestand1.Text) || string.IsNullOrEmpty(txtBestand2.Text))
            {
                MessageBox.Show("Selecteer eerst twee bestanden.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<string> triplets1;
            List<string> triplets2;

            try
            {
                triplets1 = LeesTriplets(txtBestand1.Text);
                triplets2 = LeesTriplets(txtBestand2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Er is een fout opgetreden: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double overeenkomst = BerekenOvereenkomst(triplets1, triplets2);
            txtResultaat.Text = $"overeenkomst: {overeenkomst:F0}%";
        }

        private string OpenBestand()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "Tekstbestanden (*.txt)|*.txt|Alle bestanden (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }

        private List<string> LeesTriplets(string path)
        {
            string tekst;
            try
            {
                tekst = File.ReadAllText(path);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Fout bij het lezen van het bestand: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<string>();
            }

            tekst = Regex.Replace(tekst, "[^a-zA-Z]", " ");
            tekst = Regex.Replace(tekst, "\\s+", " ").Trim();
            string[] woorden = tekst.Split(' ');

            HashSet<string> triplets = new HashSet<string>();
            for (int i = 0; i < woorden.Length - 2; i++)
            {
                string triplet = $"{woorden[i]} {woorden[i + 1]} {woorden[i + 2]}";
                triplets.Add(triplet);
            }

            return triplets.ToList();
        }

        private double BerekenOvereenkomst(List<string> lijst1, List<string> lijst2)
        {
            lijst1.Sort();
            lijst2.Sort();

            int overeenkomst = 0;
            int totaal = 0;

            int i = 0, j = 0;

            while (i < lijst1.Count && j < lijst2.Count)
            {
                int vergelijking = lijst1[i].CompareTo(lijst2[j]);

                if (vergelijking == 0)
                {
                    overeenkomst++;
                    totaal++;
                    i++;
                    j++;
                }
                else if (vergelijking < 0)
                {
                    totaal++;
                    i++;
                }
                else
                {
                    totaal++;
                    j++;
                }
            }

            totaal += lijst1.Count - i;
            totaal += lijst2.Count - j;

            return totaal == 0 ? 0 : (double)overeenkomst / totaal * 100;
        }
    }
}
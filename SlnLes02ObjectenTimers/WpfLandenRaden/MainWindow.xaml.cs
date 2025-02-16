using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfLandenRaden
{
    public partial class MainWindow : Window
    {
        private const int Interval = 3000; // 3 seconden
        private List<string> alleLanden = new List<string> { "argentina", "brazelie", "canada", "finland", "india", "japan", "marokko", "mexico", "nederland", "nieuwzeeland", "romania", "senegal", "zuidafrika" };
        private List<string> huidigeLanden = new List<string>();
        private List<double> antwoordTijden = new List<double>();
        private DispatcherTimer timer;
        private Stopwatch stopwatch;
        private int correctCount = 0;
        private int currentLandIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(Interval);
            timer.Tick += Timer_Tick;

            stopwatch = new Stopwatch();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            correctCount = 0;
            antwoordTijden.Clear();
            huidigeLanden = alleLanden.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
            stackPanel.Children.Clear();

            foreach (var land in huidigeLanden)
            {
                var imagePath = $@"afbeeldingen/{land}.png";
                if (File.Exists(imagePath))
                {
                    var image = new Image
                    {
                        Source = new BitmapImage(new Uri(imagePath, UriKind.Relative)),
                        Tag = land,
                        Width = 100,
                        Height = 100,
                        Margin = new Thickness(10)
                    };
                    image.MouseUp += Image_MouseUp;
                    stackPanel.Children.Add(image);
                }
                else
                {
                    MessageBox.Show($"Afbeelding voor {land} niet gevonden.");
                }
            }

            currentLandIndex = -1;
            NextLand();
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            NextLand();
        }

        private void NextLand()
        {
            if (currentLandIndex >= 0 && stopwatch.IsRunning)
            {
                antwoordTijden.Add(stopwatch.Elapsed.TotalMilliseconds);
                stopwatch.Reset();
            }

            currentLandIndex = (currentLandIndex + 1) % huidigeLanden.Count;
            txtLand.Text = huidigeLanden[currentLandIndex];
            stopwatch.Start();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            var clickedLand = image.Tag as string;

            if (clickedLand == txtLand.Text)
            {
                correctCount++;
                var blurEffect = new System.Windows.Media.Effects.BlurEffect { Radius = 10 };
                image.Effect = blurEffect;
                MessageBox.Show("Juist!");
                PlaySound("geluiden/right.wav");
            }
            else
            {
                MessageBox.Show("Fout!");
                PlaySound("geluiden/wrong.wav");
                antwoordTijden.Add(0); // Voeg nul toe bij fout antwoord
            }

            if (antwoordTijden.Count == huidigeLanden.Count)
            {
                EndGame();
            }
        }

        private void PlaySound(string soundFile)
        {
            var player = new SoundPlayer(soundFile);
            player.Play();
        }

        private void EndGame()
        {
            timer.Stop();
            stopwatch.Stop();

            double gemiddeldeTijd = antwoordTijden.Where(t => t > 0).Average();
            MessageBox.Show($"Aantal juiste: {correctCount}\nGemiddelde antwoordtijd: {gemiddeldeTijd} ms");
        }
    }
}

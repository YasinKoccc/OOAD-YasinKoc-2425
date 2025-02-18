using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfRaadLand
{
    public partial class MainWindow : Window
    {
        private List<TimeSpan> antwoordtijden = new();
        private string correctAnswer;
        private readonly Stopwatch stopwatch = new();
        private readonly DispatcherTimer timer;
        private int vraagNummer = 1;
        private readonly List<string> landen = new() { "Nieuw-Zeeland", "Finland", "Argentinië", "Marokko", "Japan" };
        private List<string> gevraagdeLanden = new();
        private const int MaxTime = 30;
        private int correctCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            img1.Source = new BitmapImage(new Uri("/WpfRaadLand materiaal/nieuwzeeland.png", UriKind.Relative));
            img2.Source = new BitmapImage(new Uri("/WpfRaadLand materiaal/finland.png", UriKind.Relative));
            img3.Source = new BitmapImage(new Uri("/WpfRaadLand materiaal/argentina.png", UriKind.Relative));
            img4.Source = new BitmapImage(new Uri("/WpfRaadLand materiaal/marokko.png", UriKind.Relative));
            img5.Source = new BitmapImage(new Uri("/WpfRaadLand materiaal/japan.png", UriKind.Relative));

            img1.Tag = "Nieuw-Zeeland";
            img2.Tag = "Finland";
            img3.Tag = "Argentinië";
            img4.Tag = "Marokko";
            img5.Tag = "Japan";

            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) }; // Interval
            timer.Tick += Timer_Tick;

            // Placeholder tekst
            txtResult.Text = "Kies het juiste land aan! Ben je klaar?";
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;
            StartNextQuestion();
        }

        private void StartNextQuestion()
        {
            if (gevraagdeLanden.Count == 0)
            {
                // Reset spelstatus
                vraagNummer = 1;
                correctCount = 0;
                antwoordtijden.Clear();
                gevraagdeLanden = new List<string>(landen);
            }

            stopwatch.Restart();
            timer.Start();

            // Kies een willekeurig land
            Random random = new();
            int index = random.Next(gevraagdeLanden.Count);
            correctAnswer = gevraagdeLanden[index];
            gevraagdeLanden.RemoveAt(index);

            // Toon de vraag
            txtResult.Text = $"Duid {correctAnswer} aan!";
            progressBar.Value = 100;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            double timeLeft = MaxTime - stopwatch.Elapsed.TotalSeconds;
            progressBar.Value = Math.Max(0, (timeLeft / MaxTime) * 100);

            if (timeLeft <= 0)
            {
                timer.Stop();
                CheckAnswer(null);
            }
        }

        private void CheckAnswer(object? sender)
        {
            stopwatch.Stop();
            timer.Stop();

            // Controleer antwoord
            string selectedAnswer = sender is Image img ? img.Tag?.ToString() : "";
            bool isCorrect = selectedAnswer == correctAnswer;
            if (isCorrect) correctCount++;
            antwoordtijden.Add(stopwatch.Elapsed);

            // Gemiddelde tijd berekenen
            double averageTime = antwoordtijden.Count > 0 ? antwoordtijden.Where(t => t.TotalSeconds > 0).Average(t => t.TotalSeconds) : 0;

            // Toon feedback
            txtResult.Text = isCorrect ? "Correct! " : "Fout! ";
            txtResult.Text += $"Je hebt {correctCount}/{vraagNummer} juist in gemiddeld {averageTime:F1}s";

            btnStart.IsEnabled = true;

            vraagNummer++;
            if (vraagNummer > 5)
            {
                vraagNummer = 1;
                correctCount = 0;
                antwoordtijden.Clear();
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CheckAnswer(sender);
        }
    }
}
